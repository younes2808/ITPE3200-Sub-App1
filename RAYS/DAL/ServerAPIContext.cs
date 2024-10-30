using Microsoft.EntityFrameworkCore;
using RAYS.Models;

namespace RAYS.DAL
{
    public class ServerAPIContext : DbContext
    {
        public ServerAPIContext(DbContextOptions<ServerAPIContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for Like (UserId + PostId)
            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.UserId, l.PostId });

            // Define relationship between Like and User
            modelBuilder.Entity<Like>()
                .HasOne<User>()  // Use User entity directly
                .WithMany()  // No navigation property to Likes in User
                .HasForeignKey(l => l.UserId)  // UserId is the foreign key
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if user is removed

            // Define relationship between Like and Post
            modelBuilder.Entity<Like>()
                .HasOne<Post>()  // Use Post entity directly
                .WithMany()  // No navigation property to Likes in Post
                .HasForeignKey(l => l.PostId)  // PostId is the foreign key
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if post is removed

            // Define relationship between Post and User
            modelBuilder.Entity<Post>()
                .HasOne<User>()  // Use User entity directly
                .WithMany()  // No navigation property to Posts in User
                .HasForeignKey(p => p.UserId)  // UserId is the foreign key
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if user is removed

            // Define relationship between Comment and User
            modelBuilder.Entity<Comment>()
                .HasOne<User>()  // Use User entity directly
                .WithMany()  // No navigation property to Comments in User
                .HasForeignKey(c => c.UserId)  // UserId is the foreign key
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if user is removed

            // Define relationship between Comment and Post
            modelBuilder.Entity<Comment>()
                .HasOne<Post>()  // Use Post entity directly
                .WithMany()  // No navigation property to Comments in Post
                .HasForeignKey(c => c.PostId)  // PostId is the foreign key
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if post is removed

            // Define relationship between Friend and User (Sender)
            modelBuilder.Entity<Friend>()
                .HasOne<User>()  // Use User entity directly for Sender
                .WithMany()  // No navigation property to Friends in User
                .HasForeignKey(f => f.SenderId)  // SenderId is the foreign key
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if user is removed

            // Define relationship between Friend and User (Receiver)
            modelBuilder.Entity<Friend>()
                .HasOne<User>()  // Use User entity directly for Receiver
                .WithMany()  // No navigation property to Friends in User
                .HasForeignKey(f => f.ReceiverId)  // ReceiverId is the foreign key
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if user is removed

            // Define relationship between Message and User (Sender)
            modelBuilder.Entity<Message>()
                .HasOne<User>()  // Use User entity directly for Sender
                .WithMany()  // No navigation property to Messages in User
                .HasForeignKey(m => m.SenderId)  // SenderId is the foreign key
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if user is removed

            // Define relationship between Message and User (Receiver)
            modelBuilder.Entity<Message>()
                .HasOne<User>()  // Use User entity directly for Receiver
                .WithMany()  // No navigation property to Messages in User
                .HasForeignKey(m => m.ReceiverId)  // ReceiverId is the foreign key
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if user is removed
        }
    }
}
