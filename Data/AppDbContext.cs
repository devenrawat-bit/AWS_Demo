using AWS_S3_Tutorial.Entity;
using Microsoft.EntityFrameworkCore;

namespace AWS_S3_Tutorial.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<FileData> FileDatas { get; set; }
    }
    
}
