using DeskBooker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace DeskBooker.DataAccess
{
    public class DeskBookerContext : DbContext
    {
        public DeskBookerContext(DbContextOptions<DeskBookerContext> options) : base(options)
        {
        }

        public DbSet<DeskBooking> DeskBooking { get; set; }

        public DbSet<Desk> Desk { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Desk>().HasData(
                new Desk { Id = Guid.NewGuid(), Description = "Desk 1" },
                new Desk { Id = Guid.NewGuid(), Description = "Desk 2" }
            );
        }
    }
}
