using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace highscore_exercise.Model
{
    [Route("api/[controller]")]
    [ApiController]
    public class HighscoreEntriesController : ControllerBase
    {
        private readonly HighscoreContext _context;

        public HighscoreEntriesController(HighscoreContext context)
        {
            _context = context;
        }

        // GET: api/HighscoreEntries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HighscoreEntry>>> GetHighscoreEntries()
        {
            return await _context.HighscoreEntries.OrderByDescending(he => he.Points).ToListAsync();
        }

        // GET: api/HighscoreEntries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HighscoreEntry>> GetHighscoreEntry(int id)
        {
            HighscoreEntry highscoreEntry = await _context.HighscoreEntries.FindAsync(id);

            if (highscoreEntry == null)
            {
                return NotFound();
            }

            return highscoreEntry;
        }

        // PUT: api/HighscoreEntries/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHighscoreEntry(int id, HighscoreEntry highscoreEntry)
        {
            if (id != highscoreEntry.ID)
            {
                return BadRequest();
            }

            _context.Entry(highscoreEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HighscoreEntryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/HighscoreEntries
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<HighscoreEntry>> PostHighscoreEntry(HighscoreEntry highscoreEntry)
        {
            if (string.IsNullOrWhiteSpace(highscoreEntry.Name) || highscoreEntry.Name.Trim().Length < 3 || highscoreEntry.Points <= 0)
            {
                return BadRequest();
            }
            if (_context.HighscoreEntries.Count() >= 10)
            {
                if (_context.HighscoreEntries.OrderByDescending(he => he.Points).Last().Points > highscoreEntry.Points)
                {
                    return BadRequest();
                }
                else
                {
                    _context.HighscoreEntries.Remove(_context.HighscoreEntries.OrderByDescending(he => he.Points).Last());
                }
            }
            _context.HighscoreEntries.Add(highscoreEntry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHighscoreEntry", new { id = highscoreEntry.ID }, highscoreEntry);
        }

        // DELETE: api/HighscoreEntries/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<HighscoreEntry>> DeleteHighscoreEntry(int id)
        {
            HighscoreEntry highscoreEntry = await _context.HighscoreEntries.FindAsync(id);
            if (highscoreEntry == null)
            {
                return NotFound();
            }

            _context.HighscoreEntries.Remove(highscoreEntry);
            await _context.SaveChangesAsync();

            return highscoreEntry;
        }

        private bool HighscoreEntryExists(int id)
        {
            return _context.HighscoreEntries.Any(e => e.ID == id);
        }
    }
}
