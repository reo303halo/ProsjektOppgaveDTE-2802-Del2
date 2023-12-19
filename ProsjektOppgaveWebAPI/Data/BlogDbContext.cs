using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProsjektOppgaveWebAPI.Models;


namespace ProsjektOppgaveWebAPI.Data;

public class BlogDbContext : IdentityDbContext<IdentityUser>
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Blog>? Blog { get; set; }
    public DbSet<Post>? Post { get; set; }
    public DbSet<Comment>? Comment { get; set; }
    public DbSet<IdentityUser>? User { get; set; }
    public DbSet<Tag>? Tag { get; set; }
    public DbSet<BlogTagRelations> BlogTagRelations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<BlogTagRelations>()
            .HasKey(bt => new { bt.BlogId, bt.TagId });
        
        // SEEDING PREPARATION
        var hasher = new PasswordHasher<IdentityUser>();
        
        var adminUser = new IdentityUser
        {
            UserName = "admin@example.com",
            NormalizedUserName = "ADMIN@EXAMPLE.COM",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "AdminPassword123!"),
            SecurityStamp = string.Empty
        };
        
        // SEEDING
        builder.Entity<IdentityUser>().HasData(adminUser);
    }
}