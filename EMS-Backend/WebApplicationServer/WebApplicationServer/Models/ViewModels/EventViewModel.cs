namespace WebApplicationServer.Models.ViewModels
{
    public class EventViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventCategory { get; set; }
        public string Description { get; set; }
        public string ChiefGuest { get; set; }
        public DateTime EventDate { get; set; }
        public string EventTime { get; set; }
        public string EventLocation { get; set; }
        public decimal TicketPrice { get; set; }
        public int Capacity { get; set; }
        public string BannerImage { get; set; }
        public string EventOrganizerId { get; set; }
        public string OrganizerFirstName { get; set; }
        public string OrganizerLastName { get; set; }
    }
}
