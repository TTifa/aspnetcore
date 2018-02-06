using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class TtifaContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<Article> articles { get; set; }
        public DbSet<WechatAccount> wechataccounts { get; set; }
        public DbSet<Category> categorys { get; set; }
        public DbSet<Goods> goods { get; set; }
        public DbSet<Evaluate> evaluates { get; set; }
        public DbSet<Ad> ads { get; set; }
        public DbSet<DeliveryAddress> deliveryaddresses { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderGoods> ordergoods { get; set; }
        public DbSet<Area> areas { get; set; }
        public DbSet<WechatFans> wechatfans { get; set; }

        public TtifaContext(DbContextOptions<TtifaContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().ToTable("users");
        }
    }
}
