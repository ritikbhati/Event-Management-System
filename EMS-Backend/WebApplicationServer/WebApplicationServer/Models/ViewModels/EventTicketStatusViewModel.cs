﻿namespace WebApplicationServer.Models.ViewModels
{
    public class EventTicketStatusViewModel
    {
        public List<EventStatus> Events { get; set; } = new List<EventStatus>();

        public class EventStatus
        {
            public int eventId { get; set; }
            public string EventName { get; set; }
            public int TotalTicketsSold { get; set; }
            public int TotalTicketsLeft { get; set; }
            public decimal TicketPrice {  get; set; }
            public List<UserTicket> UserTickets { get; set; } = new List<UserTicket>();
        }

        public class UserTicket
        {
            public string Username { get; set; }
            public int TotalTicketsBooked { get; set; }
            public decimal TotalPayable { get; set; }
        }


    }
}
