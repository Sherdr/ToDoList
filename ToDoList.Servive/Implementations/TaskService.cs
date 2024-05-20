using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Extensions;
using ToDoList.Domain.Filters.Task;
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
            catch (Exception ex) {
                taskLogger.LogError(ex, $"[TaskService.Create] - {ex.Message}.");
                return new BaseResponse<TaskEntity>() {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<bool>> EndTask(long id) {
            try {
                var task = await taskRepository.GetAll().FirstAsync(x => x.ID == id);
                if (task == null) {
                    return new BaseResponse<bool>() {
                        Description = "Задача не найдена.",
                        StatusCode = StatusCode.TaskNotFound
                    };
                }
                task.IsDone = true;
                await taskRepository.Update(task);
                return new BaseResponse<bool>() {
                    Description = "Задача заверешена.",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex) {
                taskLogger.LogError(ex, $"[TaskService.EndTask] - {ex.Message}.");
                return new BaseResponse<bool>() {
                    Description = $"{ex.Message}.",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> GetTasks(TaskFilter filter) {
            try {
                var tasks = await taskRepository.GetAll()
                    .Where(x => !x.IsDone)
                    .WhereIf(!string.IsNullOrWhiteSpace(filter.Name), x => x.Name == filter.Name)
                    .WhereIf(filter.Priority.HasValue, x => x.Priority == filter.Priority)
                    .Select(x => new TaskViewModel() {
                        Id = x.ID,
                        Name = x.Name,
                        Priority = x.Priority.GetDisplayName(),
                        Description = x.Description,
                        IsDone = x.IsDone == true ? "Готова" : "Не готова",
                        Created = x.Created.ToLongDateString()
                    })
                    .ToListAsync();
                return new BaseResponse<IEnumerable<TaskViewModel>>() {
                    Data = tasks,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex) {
                taskLogger.LogError(ex, $"[TaskService.Create] - {ex.Message}.");
                return new BaseResponse<IEnumerable<TaskViewModel>>() {
                    Description = $"{ex.Message}.",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
