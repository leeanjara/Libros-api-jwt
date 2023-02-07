using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Library.API.Models;

namespace Library.API.Data
{
    public class LibrosContext : IdentityDbContext
    {
        public LibrosContext (DbContextOptions<LibrosContext> options)
            : base(options)
        {
        }

        public DbSet<Libro> Libro { get; set; } = default!;
    }
}
