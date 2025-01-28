using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        public string ImageName { get; set; } // Base64

        public string PredictedText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Key PharmacistModel
        [ForeignKey("Pharmacist")]
        public int PharmacistId { get; set; }

        // Navigation Property
        public PharmacistModel Pharmacist { get; set; }
    }
}
