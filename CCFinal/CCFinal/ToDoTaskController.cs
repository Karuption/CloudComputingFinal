using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CCFinal.Data;
using CCFinal.Entities;

namespace CCFinal
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoTaskController : ControllerBase
    {
        private readonly CCFinalContext _context;

        public ToDoTaskController(CCFinalContext context)
        {
            _context = context;
        }

        // GET: api/ToDoTask
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoTask>>> GetToDoTask()
        {
          if (_context.ToDoTask == null)
          {
              return NotFound();
          }
            return await _context.ToDoTask.ToListAsync();
        }

        // GET: api/ToDoTask/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoTask>> GetToDoTask(int id)
        {
          if (_context.ToDoTask == null)
          {
              return NotFound();
          }
            var toDoTask = await _context.ToDoTask.FindAsync(id);

            if (toDoTask == null)
            {
                return NotFound();
            }

            return toDoTask;
        }

        // PUT: api/ToDoTask/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDoTask(int id, ToDoTask toDoTask)
        {
            if (id != toDoTask.Id)
            {
                return BadRequest();
            }

            _context.Entry(toDoTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoTaskExists(id))
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

        // POST: api/ToDoTask
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ToDoTask>> PostToDoTask(ToDoTask toDoTask)
        {
          if (_context.ToDoTask == null)
          {
              return Problem("Entity set 'CCFinalContext.ToDoTask'  is null.");
          }
            _context.ToDoTask.Add(toDoTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetToDoTask", new { id = toDoTask.Id }, toDoTask);
        }

        // DELETE: api/ToDoTask/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoTask(int id)
        {
            if (_context.ToDoTask == null)
            {
                return NotFound();
            }
            var toDoTask = await _context.ToDoTask.FindAsync(id);
            if (toDoTask == null)
            {
                return NotFound();
            }

            _context.ToDoTask.Remove(toDoTask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ToDoTaskExists(int id)
        {
            return (_context.ToDoTask?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
