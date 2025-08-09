using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mayka.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<MProduct> Products { get; set; }

        public DbSet<MClient> Clients { get; set; }
        public DbSet<MPurchase> Purchases { get; set; }

        public DbSet<MConstructor> Constructors { get; set; }
        public DbSet<MPhoto> Photos { get; set; }

        public DbSet<MCategory> Categories { get; set; }
        public DbSet<MComment> Comments { get; set; }

        public DbSet<MPurchaseProduct> PurchaseProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";

            string adminEmail = "admin@mail.ru";
            string adminPassword = "romazan061";

            // добавляем роли
            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            User adminUser = new User { Id = 1, Email = adminEmail, Password = adminPassword, RoleId = adminRole.Id };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });

            // добавляем категорию
            MCategory notCategory = new MCategory { Id = 1, Title = "Без категории" };
            modelBuilder.Entity<MCategory>().HasData(new MCategory[] { notCategory });
            base.OnModelCreating(modelBuilder);
        }
    }
}
