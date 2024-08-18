namespace UFSQQFacilities.Models
{
    public class FacilityImage
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }

        public int FacilityId { get; set; }
    }
}
