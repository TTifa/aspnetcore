using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class TtifaContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public TtifaContext(DbContextOptions<TtifaContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().ToTable("users");//默认：Users
        }
    }

    public class User
    {
        [Key]
        public int Uid { get; set; }
        public string Username { get; set; }
        public string Pwd { get; set; }
        public int Status { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string LastLoginIP { get; set; }
    }
}
