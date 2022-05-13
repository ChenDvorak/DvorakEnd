using Microsoft.EntityFrameworkCore;

namespace DvorakEnd.EntityFramework;

public class BlogsContext : DbContext
{
    // public BlogsContext() { }
    public BlogsContext(DbContextOptions<BlogsContext> options)
        : base(options)
    {
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlite("Data Source=./data.db");
    // }

    public DbSet<Blog> Blogs { get; set; }
}