﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using WebApplicationServer.Data;
using WebApplicationServer.Models;
using WebApplicationServer.Services.IService;

namespace WebApplicationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendETicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SendETicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("generateticket/{bookingId}")]
        public async Task<IActionResult> GenerateETicket(int bookingId)
        {
            var bookedEventDetails = await _context.BookedEvents
            .Where(be => be.BookingId == bookingId)
            .Include(be => be.Event)
            .ThenInclude(e => e.Organizer)
            .Include(be => be.User)
            .FirstOrDefaultAsync();

            if (bookedEventDetails == null)
            {
                return NotFound();
            }

            string htmlContent = $@"
                <h1>Booking ID: {bookingId}</h1>
                <h1>E-Ticket for {bookedEventDetails.Event.EventName}</h1>
                <p><strong>Event Name:</strong> {bookedEventDetails.Event.EventName}</p>
                <p><strong>Event Date:</strong> {bookedEventDetails.Event.EventDate}</p>
                <p><strong>Event Location:</strong> {bookedEventDetails.Event.EventLocation}</p>
                <p><strong>Booking Date:</strong> {bookedEventDetails.BookingDate}</p>
                <p><strong>User Name:</strong> {bookedEventDetails.User?.FirstName} {bookedEventDetails.User?.LastName}</p>
                <p><strong>Ticket Price:</strong> {bookedEventDetails.Event.TicketPrice}</p>
                <p><strong>Event Time:</strong> {bookedEventDetails.Event.Event_Time}</p>
                <p><strong>Total Tickets Booked:</strong> {bookedEventDetails.NumberOfTickets}</p>
                <p><strong>Total Price:</strong> {bookedEventDetails.NumberOfTickets * bookedEventDetails.Event.TicketPrice}</p>";

            var document = new PdfDocument();
            PdfGenerator.AddPdfPages(document, htmlContent, PageSize.A4);
            byte[]? response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }
            string filename = $"ETicket_{bookingId}.pdf";
            return File(response, "application/pdf", filename);
        }
    }
}