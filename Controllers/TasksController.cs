using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListAPI.Data;
using ToDoListAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TasksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
    {
        return await _context.TaskItems.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> PostTask(TaskItem taskItem)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Crear una nueva instancia de TaskItem con las propiedades del parámetro taskItem
        var task = new TaskItem
        {
            Name = taskItem.Name,
            IsComplete = taskItem.IsComplete
        };

        // Añadir la tarea al contexto
        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();

        // Retornar el objeto creado
        return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(int id)
    {
        var taskItem = await _context.TaskItems.FindAsync(id);

        if (taskItem == null)
        {
            return NotFound();
        }

        return taskItem;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, TaskItem updatedTask)
    {
        var task = await _context.TaskItems.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        task.Name = updatedTask.Name;
        task.IsComplete = updatedTask.IsComplete;

        _context.TaskItems.Update(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.TaskItems.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
