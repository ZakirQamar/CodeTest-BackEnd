using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class BannersContext : DbContext
    {
        public BannersContext(DbContextOptions<BannersContext> options) : base(options)
        {

        }

        public DbSet<Banner> Banners { get; set; }

    }
}
