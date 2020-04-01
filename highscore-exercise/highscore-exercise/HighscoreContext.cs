using highscore_exercise.Model;
using Microsoft.EntityFrameworkCore;

namespace highscore_exercise
{
    public class HighscoreContext : DbContext
    {
        public HighscoreContext(DbContextOptions<HighscoreContext> options) : base(options)
        {

        }

        public DbSet<HighscoreEntry> HighscoreEntries { get; set; }
    }
}
