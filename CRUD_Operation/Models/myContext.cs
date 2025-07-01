using Microsoft.EntityFrameworkCore;

namespace CRUD_Operation.Models
{
    public class myContext : DbContext
    {
        public myContext(DbContextOptions<myContext> dbContext) :base(dbContext) { }

        public DbSet<User> tbl_user { get; set; }
    }
}
