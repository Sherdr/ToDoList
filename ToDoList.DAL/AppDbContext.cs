﻿using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Entity;

namespace ToDoList.DAL {
    public class AppDbContext : DbContext {
        public DbSet<TaskEntity> Tasks { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
