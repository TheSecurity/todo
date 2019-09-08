using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using ToDo.Entities;
using ToDo.WebApi.Repositories;
using ToDo.WebApi.Storage;

namespace ToDo.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoListRepository _todoLists;

        public TodoController(ITodoListRepository todoLists)
        {
            _todoLists = todoLists;
        }

        [HttpGet("list")]
        public IActionResult ListAll()
        {
            var list = _todoLists.GetPublic();
            if (list is null) { return NotFound("The public list doesn't exist."); }

            return new JsonResult(list.Tasks);
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody]TodoTask task)
        {
            var list = _todoLists.GetPublic();
            if (list is null) { return NotFound("The public list doesn't exist."); }

            if (task.Name != null && task.Name.Length > 40)
            {
                return BadRequest("Name can be no longer than 40 characters.");
            }

            if (task.Name is null)
            {
                if (task.Description.Length < 40)
                {
                    task.Name = task.Description;
                    task.Description = string.Empty;
                }
                else
                {
                    var descriptionSnip = task.Description.Substring(0, 37);
                    task.Name = $"{descriptionSnip}�";
                }
            }

            task.Id = Guid.NewGuid();
            task.IsCompleted = false;

            list.Tasks.Add(task);
            _todoLists.Store(list);

            return Ok("Task Added.");
        }

        [HttpDelete("remove")]
        public IActionResult Remove([FromBody]Guid id)
        {
            var list = _todoLists.GetPublic();
            if (list is null) { return NotFound("The public list doesn't exist."); }

            var taskToDelete = list.Tasks.FirstOrDefault(t => t.Id == id);

            if (taskToDelete is null) { return BadRequest("A task with this ID doesn't exist."); }

            list.Tasks.Remove(taskToDelete);
            _todoLists.Store(list);

            return Ok("Task removed.");
        }

        [HttpDelete("removemany")]
        public IActionResult RemoveMany([FromBody]Guid[] ids)
        {
            var list = _todoLists.GetPublic();
            if (list is null) { return NotFound("The public list doesn't exist."); }

            var taskIds = list.Tasks.Select(t => t.Id);
            var containsInvalidId = ids.Any(id => !taskIds.Contains(id));

            if (containsInvalidId)
            {
                return BadRequest("One or more of the ids passed in were not found.");
            }

            foreach (var id in ids)
            {
                Remove(id);
            }

            return Ok("Tasks removed.");
        }
    }
}
