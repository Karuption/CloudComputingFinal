using CCFinal.Data;
using CCFinal.Dtos;
using CCFinal.Mappers;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _ca;

    public ToDoTaskController(CCFinalContext context, ITodoMapper todoMapper, ILogger<ToDoTaskController> logger,
        UserManager<IdentityUser> userManager) {
        _context = context;
        _todoMapper = todoMapper;
        _logger = logger;
        _userManager = userManager;
    }

    // GET: api/ToDoTask
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ToDoTaskDTO>>> GetToDoTask()
    {
        if (_context.ToDoTask == null)
            return NotFound();

        // Get List by User ID
        if (HttpContext.User.Identity.IsAuthenticated) {
            var userId = await _userManager.FindByNameAsync(_ca.HttpContext.User.Identity.Name);
            if (userId == null)
                return NotFound();
            return await _context.ToDoTask.Where(x => !x.IsDeleted && x.UserID == Guid.Parse(userId.Id))
                .Select(x => _todoMapper.TodoTaskToDto(x))
                .ToListAsync();
        }

        // Return globally shared for account-less
        return await _context.ToDoTask.Where(x => !x.IsDeleted && x.UserID == default)
            .Select(x => _todoMapper.TodoTaskToDto(x))
            .ToListAsync();
    }

    // GET: api/ToDoTask/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToDoTaskDTO>> GetToDoTask(int id)
    {
        if (_context.ToDoTask == null) 
            return NotFound();

        // get the task by ID
        var toDoTask = await _context.ToDoTask.FindAsync(id);
        if (toDoTask == null || toDoTask.IsDeleted)
            return NotFound();

        // Check the user ID, if user is authenticated
        if (HttpContext.User.Identity.IsAuthenticated || toDoTask.UserID != default) {
            var user = await _userManager.FindByNameAsync(_ca?.HttpContext?.User?.Identity?.Name);
            if (user == null)
                return NotFound();
            if (Guid.Parse(user.Id) == toDoTask.UserID)
                return Ok(_todoMapper.TodoTaskToDto(toDoTask));
            return NotFound();
        }

        return Ok(_todoMapper.TodoTaskToDto(toDoTask));
    }

    // PUT: api/ToDoTask/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutToDoTask(int id, ToDoTaskDTO toDoTaskDTO) {
        if (toDoTaskDTO.Id == default)
            toDoTaskDTO.Id = id;
        if (id != toDoTaskDTO.Id)
            return BadRequest();

        // Fetch task by User ID or globally shared, return not found otherwise
        var dbTask = await _context.ToDoTask.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
        if (dbTask is null || (dbTask.UserID != default && _ca.HttpContext.User.Identity.IsAuthenticated))
            return NotFound();
        if (_ca?.HttpContext?.User.Identity?.IsAuthenticated ?? (false || dbTask.UserID != default)) {
            var user = await _userManager.FindByNameAsync(_ca.HttpContext.User.Identity.Name);
            if (user is null || dbTask.UserID != Guid.Parse(user.Id))
                return NotFound();
        }


        // Making sure that the Created and updated fields get transferred correctly
        toDoTaskDTO.Created = dbTask.Created;
        toDoTaskDTO.Updated = DateTime.UtcNow;
        _todoMapper.TodoTaskDtoToModel(toDoTaskDTO, dbTask);

        // Save changes and return
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ToDoTaskDTO>> PostToDoTask(ToDoTaskDTO toDoTaskDto) {
        if (_context.ToDoTask == null)
            return Problem("Entity set 'CCFinalContext.ToDoTask'  is null.");

        // Convert DTO to Entity
        var toDoTask = new TodoMapper().TodoTaskDtoToModel(toDoTaskDto);
        toDoTask.Created = DateTime.UtcNow;
        toDoTask.Updated = DateTime.UtcNow;
        toDoTask.Id = default;

        // Set UserId
        if (_ca?.HttpContext?.User?.Identity?.IsAuthenticated ?? false) {
            var user = await _userManager.FindByNameAsync(_ca!.HttpContext!.User.Identity!.Name!);
            if (user is null)
                return BadRequest();
            toDoTask.UserID = Guid.Parse(user.Id);
        }

        // Add item to DB
        _context.ToDoTask.Add(toDoTask);
        var result = await _context.SaveChangesAsync();

        if (result < 1)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "Unable to create this task." });

        // Convert saved item to DTO 
        var toDoTaskReturn = _todoMapper.TodoTaskToDto(toDoTask);
        return CreatedAtAction("GetToDoTask", new { id = toDoTaskReturn.Id }, toDoTaskReturn);
    }

    // DELETE: api/ToDoTask/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteToDoTask(int id)
    {
        if (_context.ToDoTask == null)
            return NotFound();

        var toDoTask = await _context.ToDoTask.FindAsync(id);

        if (toDoTask == null)
            return NotFound();

        // Check if that user created that task
        if (_ca?.HttpContext?.User?.Identity?.IsAuthenticated ?? (false || toDoTask.UserID != default)) {
            var user = await _userManager.FindByNameAsync(_ca.HttpContext.User.Identity.Name);
            if (user is null || toDoTask.UserID != Guid.Parse(user.Id))
                return NotFound();
        }

        // Removing task
        _context.ToDoTask.Remove(toDoTask);

        try {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) {
            _logger.LogDebug(ex, $"Unable to remove object {toDoTask?.Id}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Failed", Message = $"Failed to delete task {toDoTask?.Id}" });
        }

        return Ok(new Response { Status = "Success", Message = "Task deleted successfully!" });
    }

    private bool ToDoTaskExists(int id)
    {
        return (_context.ToDoTask?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}