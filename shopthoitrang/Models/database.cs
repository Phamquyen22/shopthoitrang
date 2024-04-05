namespace shopthoitrang.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class database : DbContext
    {
        public database()
            : base("name=database")
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<best_pro> best_pro { get; set; }
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<comment> comment { get; set; }
        public virtual DbSet<Discount_code> Discount_code { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Order_detail> Order_detail { get; set; }
        public virtual DbSet<producer> producer { get; set; }
        public virtual DbSet<product> product { get; set; }
        public virtual DbSet<product_type> product_type { get; set; }
        public virtual DbSet<rate> rate { get; set; }
        public virtual DbSet<User_info> User_info { get; set; }
        public virtual DbSet<Voucher_user> Voucher_user { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(e => e.product_type)
                .WithOptional(e => e.Category)
                .WillCascadeOnDelete();

            modelBuilder.Entity<comment>()
                .Property(e => e.date_post)
                .IsFixedLength();

            modelBuilder.Entity<comment>()
                .HasMany(e => e.rate)
                .WithOptional(e => e.comment)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Discount_code>()
                .Property(e => e.hide)
                .IsFixedLength();

            modelBuilder.Entity<Discount_code>()
                .HasMany(e => e.Voucher_user)
                .WithOptional(e => e.Discount_code)
                .WillCascadeOnDelete();

            modelBuilder.Entity<product>()
                .HasMany(e => e.best_pro)
                .WithOptional(e => e.product)
                .HasForeignKey(e => e.id_pro)
                .WillCascadeOnDelete();

            modelBuilder.Entity<product>()
                .HasMany(e => e.rate)
                .WithOptional(e => e.product)
                .WillCascadeOnDelete();

            modelBuilder.Entity<User_info>()
                .HasMany(e => e.Account)
                .WithRequired(e => e.User_info)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User_info>()
                .HasMany(e => e.Voucher_user)
                .WithOptional(e => e.User_info)
                .WillCascadeOnDelete();
        }
    }
}
