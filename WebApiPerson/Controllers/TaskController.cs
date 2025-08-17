using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApiPerson.Factories;
using WebApiPerson.Models;
using WebApiPerson.Services;





namespace WebApiPerson.TaskControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _service;

        public TaskController(TaskService service)
        {
            _service = service;

            // Configurar las acciones (delegados de notificación)
            _service.OnCreated = (t) => Console.WriteLine($"[Task Created] #{t.Id} - {t.Description} vence {t.DueDate:yyyy-MM-dd}");
            _service.OnDeleted = (t) => Console.WriteLine($"[Task Deleted] #{t.Id} - {t.Description}");

            // Cambiar validación si querés sobreescribir la default
            _service.Validate = delegate (TaskItem t) {
                if (string.IsNullOrWhiteSpace(t.Description))
                    return (false, "La descripción no puede estar vacía.");
                if (t.DueDate == default)
                    return (false, "La fecha de vencimiento es requerida.");
                if (t.DueDate <= DateTime.UtcNow.Date.AddDays(-1))
                    return (false, "La fecha de vencimiento no puede estar en el pasado.");
                return (true, string.Empty);
            };
        }

        // GET api/tasks
        [HttpGet]
        public ActionResult<IEnumerable<TaskItem>> Get([FromQuery] bool? pending = null, [FromQuery] string? text = null)
        {
            Func<TaskItem, bool> filter = t => true;

            if (pending.HasValue)
            {
                filter = t => pending.Value ? !t.IsCompleted : t.IsCompleted;
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                var prev = filter;
                filter = t => prev(t) && t.Description.Contains(text, StringComparison.OrdinalIgnoreCase);
            }

            var items = _service.Get(filter);
            return Ok(items);
        }

        // GET api/tasks/5
        [HttpGet("{id:int}")]
        public ActionResult<TaskItem> GetById(int id)
        {
            var item = _service.Get(t => t.Id == id).FirstOrDefault();
            return item is null ? NotFound() : Ok(item);
        }

        // POST api/tasks
        [HttpPost]
        public ActionResult<TaskItem> Post(TaskItem input)
        {
            var (ok, error, created) = _service.Create(input);
            if (!ok) return BadRequest(new { message = error });
            return CreatedAtAction(nameof(GetById), new { id = created!.Id }, created);
        }
        [HttpPost("Alta")]
        public ActionResult<TaskItem> PostHighPriority(TaskItem input)
        {
            //var task = TaskFactory.CreateHighPriorityTask(description);
            var (ok, error, created) = _service.Create(input);
            if (!ok) return BadRequest(new { message = error });
            return CreatedAtAction(nameof(GetById), new { id = created!.Id }, created);
        }

    
        [HttpPost("enqueue")]
        public IActionResult AddTask([FromQuery] string description, [FromQuery] string priority)
        {

            TaskItem task;

            switch (priority.ToLower())
            {
                case "high":
                    task = TaskItemFactory.CreateHighPriorityTask(description);
                    break;
                case "low":
                    task = TaskItemFactory.CreateLowPriorityTask(description);
                    break;
                default:
                    task = TaskItemFactory.CreateNormalPriorityTask(description);
                    break;
            }

            TaskQueue.Enqueue(task);

            return Ok(new { message = "Tarea agregada a la cola", task.Description });
        }

        //[HttpPost("critical")]
        //public ActionResult<TaskItem> PostCritical([FromBody] string description)
        //{
        //    var task = TaskFactory.CreateCriticalTask(description);
        //    var (ok, error, created) = _service.Create(task);
        //    if (!ok) return BadRequest(new { message = error });
        //    return CreatedAtAction(nameof(GetById), new { id = created!.Id }, created);
        //}

        // DELETE api/tasks/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            return _service.Delete(id) ? NoContent() : NotFound();
        }

        // PUT api/tasks/5/complete
        [HttpPut("{id:int}/complete")]
        public IActionResult Complete(int id)
        {
            return _service.Complete(id) ? NoContent() : NotFound();
        }
    }
}
