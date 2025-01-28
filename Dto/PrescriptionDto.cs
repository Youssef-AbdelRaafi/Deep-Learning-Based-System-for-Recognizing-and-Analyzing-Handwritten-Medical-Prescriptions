namespace BackEnd.Dto
{
    public class PrescriptionDto
    {
        public int Id { get; set; } // Primary Key
        public string ImageName { get; set; }
        public string PredictedText { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PharmacistId { get; set; } // Foreign Key
    }
}
