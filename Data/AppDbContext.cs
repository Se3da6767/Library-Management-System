using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Programming", Description = "Software development and programming books", IsActive = true },
                new Category { Id = 2, Name = "Business", Description = "Business, management and entrepreneurship", IsActive = true },
                new Category { Id = 3, Name = "Science", Description = "General and applied science books", IsActive = true },
                new Category { Id = 4, Name = "Literature", Description = "Novels, stories and literary works", IsActive = true }
            );

            modelBuilder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "O'Reilly Media", Country = "USA", Email = "contact@oreilly.com", PhoneNo = "01000000001" },
                new Publisher { Id = 2, Name = "Pearson", Country = "UK", Email = "contact@pearson.com", PhoneNo = "01000000002" },
                new Publisher { Id = 3, Name = "Packt", Country = "UK", Email = "contact@packt.com", PhoneNo = "01000000003" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Clean Code", AuthorName = "Robert C Martin", ISBN = "9780132350884", Price = 950m, PublicationDate = new DateTime(2008, 8, 1), Pages = 464, Language = "English", IsAvailable = true, Description = "A practical guide to writing clean maintainable code.", ImagePath = "\\images\\No_Image.png", CategoryId = 1, PublisherId = 2 },
                new Book { Id = 2, Title = "C# in Depth", AuthorName = "Jon Skeet", ISBN = "9781617294532", Price = 1200m, PublicationDate = new DateTime(2019, 3, 23), Pages = 528, Language = "English", IsAvailable = true, Description = "Deep coverage of modern C# language features.", ImagePath = "\\images\\No_Image.png", CategoryId = 1, PublisherId = 1 },
                new Book { Id = 3, Title = "The Lean Startup", AuthorName = "Eric Ries", ISBN = "9780307887894", Price = 700m, PublicationDate = new DateTime(2011, 9, 13), Pages = 336, Language = "English", IsAvailable = true, Description = "Entrepreneurship and product development principles.", ImagePath = "\\images\\No_Image.png", CategoryId = 2, PublisherId = 2 }
            );

            modelBuilder.Entity<Member>().HasData(
                new Member { Id = 1, FullName = "Ahmed Mohamed Hassan", NationalId = "29805151234567", Email = "ahmed.member@gmail.com", PhoneNo = "01012345678", Address = "Cairo", BirthDate = new DateTime(1998, 5, 15), JoinDate = new DateTime(2026, 1, 15), IsActive = true },
                new Member { Id = 2, FullName = "Mariam Adel Mostafa", NationalId = "29907121234568", Email = "mariam.member@gmail.com", PhoneNo = "01123456789", Address = "Giza", BirthDate = new DateTime(1999, 7, 12), JoinDate = new DateTime(2026, 2, 10), IsActive = true }
            );
        }
    }
}
