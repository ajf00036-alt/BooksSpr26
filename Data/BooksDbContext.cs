using BooksSpr2026.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BooksSpr2026.Data
{
    public class BooksDbContext : IdentityDbContext
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options)
        {

        }


        public DbSet<Category> Categories { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Travel", Description= "Books about going places"},
                new Category { CategoryId = 2, Name= "Fiction", Description= "Books that aren't real"},
                new Category { CategoryId = 3, Name = "Technology", Description = "This is the description for the technology category"}
                );

            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, BookTitle = "Harry Potter and the Prisoner of Azkaban", Author = "J.K. Rowling", Description = "When the Knight Bus crashes through the darkness and screeches to a halt in front of him, it's the start of another far from ordinary year at Hogwarts for Harry Potter", Price = 22.91m, CategoryId = 2, ImgUrl = ""},
                new Book { BookId = 2, BookTitle = "USA National Parks: Lands of Wonder", Author = "DK Travel", Description = "The USA's National Parks truly are blah blah blah.", Price = 16.80m, CategoryId = 1, ImgUrl = ""},
                new Book { BookId = 3, BookTitle = "OCP Oracle Certified Professional Java SE 17 Developer Study Guide: Exam 1Z0-829", Author = "Scott Selikoff", Description = "In the OCP Oracle Certified Professional etc.", Price = 36.99m, CategoryId = 3, ImgUrl = "" }
                );

        }

    }

}
