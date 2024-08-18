namespace UFSQQFacilities.Models
{
    public class Notification
    {
        public int NotificationId { get; set; } 
        public string Message { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;

        public string UserEmail{ get; set; }
    }
}
