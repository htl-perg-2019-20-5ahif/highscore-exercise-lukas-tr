using highscore_exercise;
using highscore_exercise.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        private HighscoreEntriesController createController(string dbName)
        {
            DbContextOptions<HighscoreContext> options = new DbContextOptionsBuilder<HighscoreContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
            HighscoreContext context = new HighscoreContext(options);
            return new HighscoreEntriesController(context);
        }

        [Fact]
        public async Task NoEntries()
        {
            HighscoreEntriesController controller = createController("NoEntries");

            // Act
            ActionResult<IEnumerable<HighscoreEntry>> result = await controller.GetHighscoreEntries();

            // Assert
            ActionResult<IEnumerable<HighscoreEntry>> viewResult = Assert.IsType<ActionResult<IEnumerable<HighscoreEntry>>>(result);
            Assert.Empty(viewResult.Value);
        }


        [Fact]
        public async Task EntriesCanBeAdded()
        {
            HighscoreEntriesController controller = createController("EntriesCanBeAdded");

            await controller.PostHighscoreEntry(new HighscoreEntry { Name = "asd", Points = 100 });

            // Act
            ActionResult<IEnumerable<HighscoreEntry>> result = await controller.GetHighscoreEntries();

            // Assert
            ActionResult<IEnumerable<HighscoreEntry>> viewResult = Assert.IsType<ActionResult<IEnumerable<HighscoreEntry>>>(result);
            Assert.NotEmpty(viewResult.Value);
        }


        [Fact]
        public async Task EntriesAreSorted()
        {
            HighscoreEntriesController controller = createController("EntriesAreSorted");

            await controller.PostHighscoreEntry(new HighscoreEntry { Name = "asd", Points = 80 });
            await controller.PostHighscoreEntry(new HighscoreEntry { Name = "asd", Points = 100 });
            await controller.PostHighscoreEntry(new HighscoreEntry { Name = "asd", Points = 90 });

            // Act
            ActionResult<IEnumerable<HighscoreEntry>> result = await controller.GetHighscoreEntries();

            // Assert
            ActionResult<IEnumerable<HighscoreEntry>> viewResult = Assert.IsType<ActionResult<IEnumerable<HighscoreEntry>>>(result);
            List<HighscoreEntry> list = viewResult.Value.ToList();
            Assert.Equal(100, list[0].Points);
            Assert.Equal(90, list[1].Points);
            Assert.Equal(80, list[2].Points);
        }


        [Fact]
        public async Task InvalidEntriesAreNotAdded()
        {
            HighscoreEntriesController controller = createController("InvalidEntriesAreNotAdded");

            await controller.PostHighscoreEntry(new HighscoreEntry { Name = "", Points = 80 });
            await controller.PostHighscoreEntry(new HighscoreEntry { Name = "asd", Points = -100 });
            await controller.PostHighscoreEntry(new HighscoreEntry { Name = "asd", Points = 0 });
            await controller.PostHighscoreEntry(new HighscoreEntry { Name = "ad", Points = 10 });

            // Act
            ActionResult<IEnumerable<HighscoreEntry>> result = await controller.GetHighscoreEntries();

            // Assert
            ActionResult<IEnumerable<HighscoreEntry>> viewResult = Assert.IsType<ActionResult<IEnumerable<HighscoreEntry>>>(result);
            Assert.Empty(viewResult.Value);
        }

        [Fact]
        public async Task EntriesAreCappedAt10()
        {
            HighscoreEntriesController controller = createController("EntriesAreCappedAt10");

            for (int i = 0; i < 11; i++)
            {
                await controller.PostHighscoreEntry(new HighscoreEntry { Name = "asdf", Points = 80 });
            }

            // Act
            ActionResult<IEnumerable<HighscoreEntry>> result = await controller.GetHighscoreEntries();

            // Assert
            ActionResult<IEnumerable<HighscoreEntry>> viewResult = Assert.IsType<ActionResult<IEnumerable<HighscoreEntry>>>(result);
            Assert.Equal(10, viewResult.Value.Count());
        }

        [Fact]
        public async Task NewEntriesNotAddedIfScoreLowerThanLastPlace()
        {
            HighscoreEntriesController controller = createController("NewEntriesNotAddedIfScoreLowerThanLastPlace");

            for (int points = 10; points <= 110; points += 10)
            {
                await controller.PostHighscoreEntry(new HighscoreEntry { Name = "asdf", Points = points });
            }

            // Act
            ActionResult<IEnumerable<HighscoreEntry>> result = await controller.GetHighscoreEntries();

            // Assert
            ActionResult<IEnumerable<HighscoreEntry>> viewResult = Assert.IsType<ActionResult<IEnumerable<HighscoreEntry>>>(result);
            Assert.Equal(10, viewResult.Value.Count());

            await controller.PostHighscoreEntry(new HighscoreEntry { Name = "asdf", Points = 9 });

            result = await controller.GetHighscoreEntries();
            viewResult = Assert.IsType<ActionResult<IEnumerable<HighscoreEntry>>>(result);
            Assert.Equal(10, viewResult.Value.Count());
            Assert.NotEqual(9, viewResult.Value.Last().Points);
        }
    }
}
