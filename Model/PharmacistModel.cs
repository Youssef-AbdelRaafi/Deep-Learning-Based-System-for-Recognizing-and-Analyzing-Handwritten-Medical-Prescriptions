using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Backend;

namespace BackEnd.Model
{
    [Table("PharmacistModel")]
    public class PharmacistModel
    {
        [Key]
        public int Id { get; set; }

        [EmailAddress, Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        public decimal Hight { get; set; }

        [Required]
        public decimal Weight { get; set; }

        [Required]
        public DateTime BirthOfDate { get; set; }

        // Navigation Property
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
