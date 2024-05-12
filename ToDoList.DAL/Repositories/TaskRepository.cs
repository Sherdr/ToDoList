using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;

namespace ToDoList.DAL.Repositories {
    public class TaskRepository : IBaseRepository<TaskEntity> {
        readonly AppDbContext appDbContext;

        public TaskRepository(AppDbContext context) {
            appDbContext = context;
        }
        public async Task Create(TaskEntity entity) {
            await appDbContext.Tasks.AddAsync(entity);
            await appDbContext.SaveChangesAsync();
        }
        public IQueryable<TaskEntity> GetAll() {
            return appDbContext.Tasks;
        }
        public async Task Delete(TaskEntity entity) {
            appDbContext.Tasks.Remove(entity);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<TaskEntity> Update(TaskEntity entity) {
            appDbContext.Tasks.Update(entity);
            await appDbContext.SaveChangesAsync();
            return entity;
        }
    }
}
