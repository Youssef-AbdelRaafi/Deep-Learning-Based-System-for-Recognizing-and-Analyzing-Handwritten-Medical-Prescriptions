using BackEnd.Dto;
using System.Globalization;
using System.Text.Json;
using AutoMapper;
using BackEnd;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace BackEnd.Controllers;

using Backend;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class HandlerController : ControllerBase
{

    private readonly AppDbContext _context;
   

    public HandlerController(AppDbContext context)
    {
        _context = context;
  
    }

    [HttpPost("/RegisterPharmacist")]
    public async Task<IActionResult> RegisterPharmacist(PharmacistDto pharmacistDto)
    {
        var isExist = await _context._pharmacistModels
        .FirstOrDefaultAsync(ph => ph.Email == pharmacistDto.Email);

        if (isExist is not null)
        {
            return BadRequest("this is email already exist");
        }
        else
        {
            var pharmacist = new PharmacistModel
            {
                Email = pharmacistDto.Email,
                BirthOfDate = pharmacistDto.BirthOfDate,
                FullName = pharmacistDto.FullName,
                Hight = pharmacistDto.Hight,
                Password = pharmacistDto.Password,
                Weight = pharmacistDto.Weight,
            };

            await _context._pharmacistModels.AddAsync(pharmacist);
            _context.SaveChanges();

            return Ok("Add new Pharmacist successfully");
        }
    }
    [HttpPost("LoginPharmacist")]
    public async Task<IActionResult> LoginPharmacist([FromBody] LoginDto loginDto)
    {
        var pharmacist = await _context._pharmacistModels
            .FirstOrDefaultAsync(ph => ph.Email == loginDto.Email && ph.Password == loginDto.Password);

        if (pharmacist == null)
        {
            return BadRequest("Invalid email or password.");
        }

        return Ok(new
        {
            pharmacistId = pharmacist.Id,
            fullName = pharmacist.FullName,
            email = pharmacist.Email
        });
    }

    // Add Prescription
    [HttpPost("AddPrescription")]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionDto dto)
    {
        try
        {
            var pharmacist = await _context._pharmacistModels.FindAsync(dto.PharmacistId);
            if (pharmacist == null)
                return NotFound($"Pharmacist with ID {dto.PharmacistId} not found.");

            var prescription = new Prescription
            {
                ImageName = dto.ImageName,
                PredictedText = dto.PredictedText,
                CreatedAt = DateTime.Now,
                PharmacistId = dto.PharmacistId
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Prescription saved successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // Get Prescriptions for Pharmacist
    [HttpGet("GetPrescriptions/{pharmacistId}")]
    public async Task<IActionResult> GetPrescriptions(int pharmacistId)
    {
        try
        {
            var prescriptions = await _context.Prescriptions
                .Where(p => p.PharmacistId == pharmacistId)
                .ToListAsync();

            if (!prescriptions.Any())
            {
                return Ok(new { message = "No prescriptions found for this pharmacist.", prescriptions = new List<PrescriptionDto>() });
            }

            var prescriptionDtos = prescriptions.Select(p => new PrescriptionDto
            {
                Id = p.Id,
                ImageName = p.ImageName,
                PredictedText = p.PredictedText,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(prescriptionDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    // Delete Prescription
    [HttpDelete("DeletePrescription/{id}")]
    public async Task<IActionResult> DeletePrescription(int id)
    {
        var prescription = await _context.Prescriptions.FindAsync(id);
        if (prescription == null)
        {
            return NotFound("Prescription not found.");
        }

        _context.Prescriptions.Remove(prescription);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Prescription deleted successfully!" });
    }

    [HttpPut("/UpdateProfile")]
    public async Task<IActionResult> UpdateProfileAsync(EditProfileDto editProfileDto)
    {
        var isExist = await _context._pharmacistModels
        .FirstOrDefaultAsync(ph => ph.Email == editProfileDto.Email);

        if (isExist is null)
            return BadRequest("You can not change email or this is information missing ");

        isExist.BirthOfDate = editProfileDto.BirthOfDate;
        isExist.FullName = editProfileDto.FullName;
        isExist.Hight = editProfileDto.Hight;
        isExist.Weight = editProfileDto.Weight;
        isExist.Email = editProfileDto.Email;


            _context._pharmacistModels.Update(isExist);
            _context.SaveChanges();

            return Ok("Update new Pharmacist successfully");
    }

}
