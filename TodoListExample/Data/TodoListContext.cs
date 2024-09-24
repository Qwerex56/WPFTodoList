using System.Data;
using Microsoft.EntityFrameworkCore;

namespace TodoListExample.Data;

public class TodoListContext : DbContext {
    public DbSet<Todo> AllTodos => Set<Todo>();
    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public TodoListContext() {
    }

    public TodoListContext(DbContextOptions<TodoListContext> options) : base(options) {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=todolist.db");
        
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Todo>().Property(e => e.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<TodoList>().Property(e => e.Id).ValueGeneratedOnAdd();
        
        base.OnModelCreating(modelBuilder);
    }
}