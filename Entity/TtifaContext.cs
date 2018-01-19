using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class TtifaContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<Article> articles { get; set; }

        public TtifaContext(DbContextOptions<TtifaContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().ToTable("users");
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

    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Author { get; set; }
        public int AuthorUid { get; set; }
    }
}
