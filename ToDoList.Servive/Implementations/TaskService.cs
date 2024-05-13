using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Service.Implementations {
    public class TaskService : ITaskService {
        readonly IBaseRepository<TaskEntity> taskRepository;
        ILogger<TaskService> taskLogger;

        public TaskService(IBaseRepository<TaskEntity> repository, ILogger<TaskService> logger) {
            taskRepository = repository;
            taskLogger = logger;
        }
        public async Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model) {
            try {
                model.Validate();
                taskLogger.LogInformation($"Запрос на создание задачи - {model.Name}.");
                var task = await taskRepository.GetAll()
                    .Where(x => x.Created.Date == DateTime.Today)
                    .FirstOrDefaultAsync(x => x.Name == model.Name);
                if (task != null) {
                    return new BaseResponse<TaskEntity>() {
                        Description = "Задача с таким названием уже есть.",
                        StatusCode = StatusCode.TaskIsReady
                    };
                }
                task = new TaskEntity() {
                    Name = model.Name,
                    Description = model.Description,
                    Priority = model.Priority,
                    Created = DateTime.Now,
                    IsDone = false
                };
                await taskRepository.Create(task);
                taskLogger.LogInformation($"Задача создалась - {task.Name}, {task.Created}.");
                return new BaseResponse<TaskEntity>() {
                    Description = "Задача создалась.",
                    StatusCode = StatusCode.OK
                };
            }
            catch(Exception ex) {
                taskLogger.LogError(ex, $"[TaskService.Create] - {ex.Message}.");
                return new BaseResponse<TaskEntity>() {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
