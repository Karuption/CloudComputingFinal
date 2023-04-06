using CCFinal.Data;
using CCFinal.Dtos;
using CCFinal.Entities;
using CCFinal.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCFinal.Controllers; 

[Route("api/[controller]")]
[ApiController]
public class ToDoTaskController : ControllerBase
{
    private readonly CCFinalContext _context;
    private readonly ITodoMapper _todoMapper;

    public ToDoTaskController(CCFinalContext context, ITodoMapper todoMapper)
    {
        _context = context;
        _todoMapper = todoMapper;
    }

    // GET: api/ToDoTask
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToDoTaskDTO>>> GetToDoTask()
    {
        if (_context.ToDoTask == null) return NotFound();
        
        return _context.ToDoTask.Select(x=>_todoMapper.ToDoTaskToDto(x)).ToList();
    }

    // GET: api/ToDoTask/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ToDoTaskDTO>> GetToDoTask(int id)
    {
        if (_context.ToDoTask == null) return NotFound();
        var toDoTask = await _context.ToDoTask.FindAsync(id);

        if (toDoTask == null)
        {
            return NotFound();
        }

        return _todoMapper.ToDoTaskToDto(toDoTask);
    }

    // PUT: api/ToDoTask/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutToDoTask(int id, ToDoTaskDTO toDoTaskDTO) {
        
        var toDoTask = _todoMapper.ToDoTaskDtoToModel(toDoTaskDTO);

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
    public async Task<ActionResult<ToDoTaskDTO>> PostToDoTask(ToDoTaskDTO toDoTaskDto) {
        
        var toDoTask = (new Mappers.TodoMapper()).ToDoTaskDtoToModel(toDoTaskDto);

        if (_context.ToDoTask == null) 
            return Problem("Entity set 'CCFinalContext.ToDoTask'  is null.");
        
        _context.ToDoTask.Add(toDoTask);
        await _context.SaveChangesAsync();

        toDoTaskDto = _todoMapper.ToDoTaskToDto(toDoTask);
        

        return CreatedAtAction("GetToDoTask", new { id = toDoTaskDto.Id }, toDoTaskDto);
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