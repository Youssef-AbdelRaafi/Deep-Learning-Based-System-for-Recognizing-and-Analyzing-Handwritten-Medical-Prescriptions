using Microsoft.EntityFrameworkCore;
using Backend;

namespace BackEnd;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PharmacistModel> _pharmacistModels { get; set; }
}

