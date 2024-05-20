using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Controllers {
    public class TaskController : Controller {
        readonly ITaskService taskService;

        public TaskController(ITaskService service) {
            taskService = service;
        }
        public IActionResult Index() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskViewModel model) {
            var responce = await taskService.Create(model);
            if (responce.StatusCode == Domain.Enum.StatusCode.OK) {
                return Ok(new {
                    description = responce.Description
                });
            }
            return BadRequest(new {
                description = responce.Description
            });
        }
        [HttpPost]
        public async Task<IActionResult> TaskHandler(TaskFilter filter) {
            var responce = await taskService.GetTasks(filter);
            return Json(new { data = responce.Data });
        }

        [HttpPost]
        public async Task<IActionResult> EndTask(long id) {
            var responce = await taskService.EndTask(id);
            if (responce.StatusCode == Domain.Enum.StatusCode.OK) {
                return Ok(new {
                    description = responce.Description
                });
            }
            return BadRequest(new {
                description = responce.Description
            });
        }
    }
}
