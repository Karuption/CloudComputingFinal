using CCFinal.Data;
using CCFinal.Dtos;
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
    private readonly ILogger<ToDoTaskController> _logger;

    public ToDoTaskController(CCFinalContext context, ITodoMapper todoMapper, ILogger<ToDoTaskController> logger) {
        _context = context;
        _todoMapper = todoMapper;
        _logger = logger;
    }

    // GET: api/ToDoTask
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ToDoTaskDTO>>> GetToDoTask()
    {
        if (_context.ToDoTask == null) return NotFound();

        return await _context.ToDoTask.Where(x => !x.IsDeleted)
            .Select(x => _todoMapper.TodoTaskToDto(x))
            .ToListAsync();
    }

    // GET: api/ToDoTask/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ToDoTaskDTO>> GetToDoTask(int id)
    {
        if (_context.ToDoTask == null) 
            return NotFound();
        var toDoTask = await _context.ToDoTask.FindAsync(id);

        if (toDoTask == null || toDoTask.IsDeleted)
            return NotFound();

        return _todoMapper.TodoTaskToDto(toDoTask);
    }

    // PUT: api/ToDoTask/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PutToDoTask(int id, ToDoTaskDTO toDoTaskDTO) {
        if (toDoTaskDTO.Id == default)
            toDoTaskDTO.Id = id;
        if (id != toDoTaskDTO.Id)
            return BadRequest();

        var dbTask = await _context.ToDoTask
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync(x => x.UserID == id);

        if (dbTask == null)
            return NotFound();

        //Making sure that the Created and updated get transferred correctly
        toDoTaskDTO.Created = dbTask.Created;
        toDoTaskDTO.Updated = DateTime.UtcNow;
        _todoMapper.TodoTaskDtoToModel(toDoTaskDTO, dbTask);

        try {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) {
            if (!ToDoTaskExists(id)) {
                _logger.LogInformation($"Task {id} was queried, but could not be found");
                return NotFound();
            }

            _logger.LogDebug(ex, $"Unable to delete task {dbTask.Id}");
        }

        return NoContent();
    }

    // POST: api/ToDoTask
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToDoTaskDTO>> PostToDoTask(ToDoTaskDTO toDoTaskDto) {
        if (_context.ToDoTask == null)
            return Problem("Entity set 'CCFinalContext.ToDoTask'  is null.");

        var toDoTask = new TodoMapper().TodoTaskDtoToModel(toDoTaskDto);
        toDoTask.Created = DateTime.UtcNow;
        toDoTask.Updated = DateTime.UtcNow;
        toDoTask.Id = default;

        _context.ToDoTask.Add(toDoTask);
        await _context.SaveChangesAsync();

        var toDoTaskReturn = _todoMapper.TodoTaskToDto(toDoTask);


        return CreatedAtAction("GetToDoTask", new { id = toDoTaskReturn.Id }, toDoTaskReturn);
    }

    // DELETE: api/ToDoTask/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteToDoTask(int id)
    {
        if (_context.ToDoTask == null)
            return NotFound();

        var toDoTask = await _context.ToDoTask.FindAsync(id);

        if (toDoTask == null)
            return NotFound();


        _context.ToDoTask.Remove(toDoTask);

        try {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) {
            _logger.LogDebug(ex, $"Unable to remove object {toDoTask?.Id}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

    private bool ToDoTaskExists(int id)
    {
        return (_context.ToDoTask?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}