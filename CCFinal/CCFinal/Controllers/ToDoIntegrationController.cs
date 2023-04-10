﻿using CCFinal.Data;
using CCFinal.Dtos;
using CCFinal.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCFinal.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ToDoIntegrationController : ControllerBase {
    private readonly CCFinalContext _context;
    private readonly ITodoMapper _todoMapper;

    public ToDoIntegrationController(CCFinalContext context, ITodoMapper todoMapper) {
        _context = context;
        _todoMapper = todoMapper;
    }

    // GET: api/ToDoTask
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ToDoTaskIntegrationDto>>> GetToDoTask() {
        if (_context.ToDoTask == null)
            return NotFound();

        return _context.ToDoTask.Select(x => _todoMapper.TodoTaskModelToDto(x)).ToList();
    }

    // GET: api/ToDoTask/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ToDoTaskIntegrationDto>> GetToDoTask(string id) {
        if (_context.ToDoTask == null)
            return NotFound();
        var toDoTask = await _context.ToDoTask.FirstOrDefaultAsync(x => x.IntegrationId == id);

        if (toDoTask == null)
            return NotFound();

        return _todoMapper.TodoTaskModelToDto(toDoTask);
    }

    // PUT: api/ToDoTask/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PutToDoTask(string id, ToDoTaskIntegrationDto toDoTaskDTO) {
        if (toDoTaskDTO.IntegrationId == default)
            toDoTaskDTO.IntegrationId = id;
        if (id != toDoTaskDTO.IntegrationId)
            return BadRequest();
        var task = _context.ToDoTask.FirstOrDefault(x => x.IntegrationId == id);

        if (task is null)
            return NotFound();

        toDoTaskDTO.Id = task.Id;
        toDoTaskDTO.Created = task.Created;
        toDoTaskDTO.Updated = DateTime.UtcNow;

        _todoMapper.TodoTaskDtoToModel(toDoTaskDTO, task);

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) {
            if (!ToDoTaskExists(task.Id))
                return NotFound();
            throw;
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
        toDoTask.Id = default;
        toDoTask.Created = DateTime.UtcNow;
        toDoTask.Updated = DateTime.UtcNow;

        _context.ToDoTask.Add(toDoTask);
        await _context.SaveChangesAsync();

        _todoMapper.TodoTaskToDto(toDoTask, toDoTaskDto);


        return CreatedAtAction("GetToDoTask", new { id = toDoTaskDto.Id }, toDoTaskDto);
    }

    // DELETE: api/ToDoTask/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteToDoTask(int id) {
        if (_context.ToDoTask == null) return NotFound();
        var toDoTask = await _context.ToDoTask.FindAsync(id);
        if (toDoTask == null) return NotFound();

        _context.ToDoTask.Remove(toDoTask);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ToDoTaskExists(int id) {
        return (_context.ToDoTask?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}