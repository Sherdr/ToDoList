using ToDoList.Domain.Enum;

namespace ToDoList.Domain.Entity {
    public class TaskEntity {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
    }
}
