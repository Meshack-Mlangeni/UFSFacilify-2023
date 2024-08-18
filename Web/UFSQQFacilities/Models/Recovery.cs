namespace UFSQQFacilities.Models
{
    public class Recovery
    {
        public int RecoveryId { get; set; }
        public string UserEmail { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
    }
}
