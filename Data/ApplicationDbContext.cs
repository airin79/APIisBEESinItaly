using APIisBEESinItaly.Models;
using Microsoft.EntityFrameworkCore;

namespace APIisBEESinItaly.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Pet> Pets { get; set; }  // Esta propiedad representa la tabla de la base de datos
    }
}