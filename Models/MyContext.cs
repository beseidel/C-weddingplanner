using Microsoft.EntityFrameworkCore;


namespace weddingplannerBES.Models


{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }

        // "users" table is represented by this DbSet "Users"

        // put only the tables that you want to go into mysql here

        public DbSet<User> UsersTable { get; set; }
        public DbSet<Wedding> WeddingsTable { get; set; }
        public DbSet<Reservation> ReservationsTable { get; set; }

        // public DbSet<Dish> DishesTable { get; set; }


    }
}
