namespace BackEnd.Dto;

public class EditProfileDto
{
    [EmailAddress, Required] public string? Email { get; set; }
    [Required] public string? FullName { get; set; }
    [Required] public decimal Hight { get; set; }
    [Required] public decimal Weight { get; set; }
    public DateTime BirthOfDate { get; set; }
}
