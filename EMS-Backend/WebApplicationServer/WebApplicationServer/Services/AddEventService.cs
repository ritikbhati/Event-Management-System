﻿using Azure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplicationServer.Data;
using WebApplicationServer.Email;
using WebApplicationServer.Models;
using WebApplicationServer.Models.ResponseModels;
using WebApplicationServer.Models.ViewModels;
using WebApplicationServer.Services.IService;

namespace WebApplicationServer.Services
{
    public class AddEventService : IAddEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ISendRegisterSuccessMailService _sendRegisterSuccessMailService;


        public AddEventService(ApplicationDbContext context, CloudinaryService cloudinaryService, ISendRegisterSuccessMailService sendRegisterSuccessMailService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _sendRegisterSuccessMailService = sendRegisterSuccessMailService;
        }

        public async Task<List<EventViewModel>> GetAllEvents()
        {
            var events = await _context.Events
                .Include(e => e.Organizer)
                .Select(e => new EventViewModel
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventCategory = e.EventCategory,
                    Description = e.Description,
                    ChiefGuest = e.ChiefGuest,
                    EventDate = e.EventDate,
                    EventTime = e.Event_Time,
                    EventLocation = e.EventLocation,
                    TicketPrice = e.TicketPrice,
                    Capacity = e.Capacity,
                    BannerImage = e.BannerImage,
                    EventOrganizerId = e.EventOrganizerId,
                    OrganizerFirstName = e.Organizer.FirstName,
                    OrganizerLastName = e.Organizer.LastName
                })
                .ToListAsync();

            return events;
        }

        public async Task<GetEVentByIdResposeViewModel> GetEventById(int EventId)
        {
            GetEVentByIdResposeViewModel response = new GetEVentByIdResposeViewModel();
            response.Status = 200;
            response.Message = "All Events Fetched";
            response.GetEventById = await _context.Events.FindAsync(EventId);
            return response;
        }



        public async Task<ResponseViewModel> AddEvent(AddEventViewModel addEvent, string Id, IFormFile bannerImage)
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(bannerImage);

            Event eventToBeAdded = new Event
            {
                EventName = addEvent.EventName,
                EventCategory = addEvent.EventCategory,
                EventLocation = addEvent.EventLocation,
                Event_Time = addEvent.Event_Time,
                EventDate = addEvent.EventDate,
                ChiefGuest = addEvent.ChiefGuest,
                Description = addEvent.Description,
                Capacity = addEvent.Capacity,
                TicketPrice = addEvent.TicketPrice,
                BannerImage = imageUrl,
                EventOrganizerId = Id
            };

            await _context.Events.AddAsync(eventToBeAdded);
            await _context.SaveChangesAsync();


            ResponseViewModel response = new ResponseViewModel();
            if (eventToBeAdded == null)
            {
                response.Status = 500;
                response.Message = "Unable to add event, please try again.";
                return response;
            }

            response.Status = 200;
            response.Message = "Event Successfully added.";
            return response;
        }

        public async Task<ResponseViewModel> DeleteEvent(int id)
        {
            ResponseViewModel response = new ResponseViewModel();

            try
            {
                var eventToDelete = await _context.Events.FindAsync(id);
                if (eventToDelete == null)
                {
                    response.Status = 404;
                    response.Message = "Event not found";
                }
                else
                {
                    _context.Events.Remove(eventToDelete);
                    await _context.SaveChangesAsync();
                    response.Status = 200;
                    response.Message = "Event deleted successfully";
                }
            }
            catch (Exception ex)
            {
                response.Status = 500;
                response.Message = $"Error deleting event: {ex.Message}";
            }
            return response;
        }


        public async Task<ResponseViewModel> UpdateEvent(int id, UpdateEventViewModel updateEvent, string userId)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                var eventToUpdate = await _context.Events.FindAsync(id);
                if (eventToUpdate == null)
                {
                    response.Status = 404;
                    response.Message = "Event not found";
                    return response;
                }

                if (eventToUpdate.EventOrganizerId != userId)
                {
                    response.Status = 403;
                    response.Message = "You are not authorized to update this event";
                    return response;
                }

                eventToUpdate.EventName = updateEvent.EventName;
                eventToUpdate.EventCategory = updateEvent.EventCategory;
                eventToUpdate.Description = updateEvent.Description;
                eventToUpdate.ChiefGuest = updateEvent.ChiefGuest;
                eventToUpdate.EventDate = updateEvent.EventDate;
                eventToUpdate.Event_Time = updateEvent.Event_Time;
                eventToUpdate.EventLocation = updateEvent.EventLocation;
                eventToUpdate.TicketPrice = updateEvent.TicketPrice;
                eventToUpdate.Capacity = updateEvent.Capacity;
                eventToUpdate.BannerImage = eventToUpdate.BannerImage;

                await _context.SaveChangesAsync();

                var bookedUsers = await _context.BookedEvents
                .Where(b => b.EventId == id)
                .Include(b => b.User) 
                .ToListAsync();


                foreach (var bookedUser in bookedUsers)
                {
                    if (bookedUser.User != null)
                    {
                        string email = bookedUser.User.Email;
                        string subject = "Event Updated: " + eventToUpdate.EventName;                        
                        string message = UpdateEventEmail.EmailBody(
                            eventToUpdate.EventName,
                            bookedUser.User.FirstName,
                            bookedUser.User.LastName,
                            eventToUpdate.EventDate.ToString(),
                            eventToUpdate.Event_Time.ToString(),
                            eventToUpdate.EventLocation,
                            eventToUpdate.Description);

                        bool emailSent = await _sendRegisterSuccessMailService.SendRegisterSuccessMailAsync(email, subject, message);

                        if (!emailSent)
                        {
                            response.Status = 500;
                            response.Message = "Failed to send update email to some users.";
                            return response;
                        }
                    }
                }

                response.Status = 200;
                response.Message = "Event successfully updated";
            }
            catch (Exception ex)
            {
                response.Status = 500;
                response.Message = $"Error updating event: {ex.Message}";
            }

            return response;
        }

        public async Task<GetEventByAppliedFilterResponseViewModel> GetEventsByCategory(string category)
        {

            GetEventByAppliedFilterResponseViewModel response = new GetEventByAppliedFilterResponseViewModel();
            response.Status = 200;
            response.Message = "All Events Fetched that matches the Category";
            response.CategoryEvents = await _context.Events.Where(e => e.EventCategory == category).ToListAsync();
            return response;
        }

        public async Task<GetEventByAppliedFilterResponseViewModel> GetEventsByLocation(string location)
        {

            GetEventByAppliedFilterResponseViewModel response = new GetEventByAppliedFilterResponseViewModel();
            response.Status = 200;
            response.Message = "All Events Fetched that matches the Location";
            response.CategoryEvents = await _context.Events.Where(e => e.EventLocation == location).ToListAsync();
            return response;
        }

        public async Task<List<OrganizerCreatedEventViewModel>> GetOrganizerCreatedEvents(string organizerId)
        {
            var events = await _context.Events
                .Where(e => e.EventOrganizerId == organizerId)
                .Select(e => new OrganizerCreatedEventViewModel
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventCategory = e.EventCategory,
                    EventDescription = e.Description,
                    ChiefGuest = e.ChiefGuest,
                    EventDate = e.EventDate,
                    Event_Time = e.Event_Time,
                    EventLocation = e.EventLocation,
                    TicketPrice = e.TicketPrice,
                    Capacity = e.Capacity,
                    BannerImage = e.BannerImage
                })
                .ToListAsync();

            return events;
        }

        public async Task<List<TicketDetailsViewModel>> GetTicketDetailsForOrganizer(int eventId, string organizerId)
        {
            var eventDetails = await _context.Events
            .FirstOrDefaultAsync(e => e.EventId == eventId && e.EventOrganizerId == organizerId);

            if (eventDetails == null)
            {
                return null;
            }

            var bookedEvents = await _context.BookedEvents
            .Include(be => be.User)
            .Where(be => be.EventId == eventId)
            .ToListAsync();


            var ticketDetails = bookedEvents
            .Select(be => new TicketDetailsViewModel
            {
                UserName = be.User.FirstName + " " + be.User.LastName,
                TotalTickets = 1,
                TotalAmountReceived = eventDetails.TicketPrice,
                EventName = eventDetails.EventName,
                EventDate = eventDetails.EventDate,
                EventLocation = eventDetails.EventLocation,
                TotalTicketsAvailable = eventDetails.Capacity
            }).ToList();

            return ticketDetails;
        }

        public async Task<List<string>> GetUniqueEventCategories()
        {

            List<string> eventCategories = await _context.Events.Select(e => e.EventCategory).Distinct().ToListAsync();
            return eventCategories;

        }

        public async Task<List<string>> GetUniqueEventLocation()
        {
            List<string> eventLocations = await _context.Events.Select(e => e.EventLocation).Distinct().ToListAsync();
            return eventLocations;

        }
        public async Task<IEnumerable<EventViewModel>> GetPastEvents()
        {
            var currentDate = DateTime.Today;
            var upcomingEvents = await _context.Events
           .Where(e => e.EventDate < currentDate)
           .Select(e => new EventViewModel
           {
               EventId = e.EventId,
               EventName = e.EventName,
               EventCategory = e.EventCategory,
               Description = e.Description,
               ChiefGuest = e.ChiefGuest,
               EventDate = e.EventDate,
               EventTime = e.Event_Time,
               EventLocation = e.EventLocation,
               TicketPrice = e.TicketPrice,
               Capacity = e.Capacity,
               BannerImage = e.BannerImage,
               EventOrganizerId = e.EventOrganizerId,
               OrganizerFirstName = e.Organizer.FirstName,
               OrganizerLastName = e.Organizer.LastName
           })
           .ToListAsync();

            return upcomingEvents;
        }

        public async Task<IEnumerable<EventViewModel>> GetUpcomingEvents()
        {
            var currentDate = DateTime.Today;
            var upcomingEvents = await _context.Events
                .Where(e => e.EventDate >= currentDate)
                .Select(e => new EventViewModel
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventCategory = e.EventCategory,
                    Description = e.Description,
                    ChiefGuest = e.ChiefGuest,
                    EventDate = e.EventDate,
                    EventTime = e.Event_Time,
                    EventLocation = e.EventLocation,
                    TicketPrice = e.TicketPrice,
                    Capacity = e.Capacity,
                    BannerImage = e.BannerImage,
                    EventOrganizerId = e.EventOrganizerId,
                    OrganizerFirstName = e.Organizer.FirstName,
                    OrganizerLastName = e.Organizer.LastName
                })
                .ToListAsync();
            return upcomingEvents;
        }

        public async Task<EventDetailsWithUserViewModel> GetEventDetails(int eventId)
        {
            var eventDetails = await _context.Events.FindAsync(eventId);
            if (eventDetails == null)
            {
                return null;
            }
            var bookedUsers = await _context.BookedEvents
              .Include(be => be.User)
              .Where(be => be.EventId == eventId)
              .Select(be => new BookedUserViewModel
              {
                  UserId = be.UserId,
                  FirstName = be.User.FirstName,
                  LastName = be.User.LastName,
                  Email = be.User.Email,
                  PhoneNumber = be.User.PhoneNumber,
                  NumberOfTickets = be.NumberOfTickets,
                  TotalPrice = be.NumberOfTickets * eventDetails.TicketPrice
              })
              .ToListAsync();
            var EventDetailsWithUserViewModel = new EventDetailsWithUserViewModel
            {
                EventId = eventDetails.EventId,
                EventName = eventDetails.EventName,
                EventCategory = eventDetails.EventCategory,
                Description = eventDetails.Description,
                ChiefGuest = eventDetails.ChiefGuest,
                EventDate = eventDetails.EventDate,
                EventTime = eventDetails.Event_Time,
                EventLocation = eventDetails.EventLocation,
                TicketPrice = eventDetails.TicketPrice,
                Capacity = eventDetails.Capacity,
                BannerImage = eventDetails.BannerImage,
                BookedUsers = bookedUsers
            };
            return EventDetailsWithUserViewModel;
        }

    }
}


