using System.ComponentModel.DataAnnotations;

namespace TodoListExample.Data;

public class Todo {
    [Key]
    public int Id { get; init; }

    [MaxLength(64)]
    public string Header { get; set; } = "";

    public bool IsCompleted { get; set; }

    public int? TodoListId { get; init; }
    public TodoList? TodoList { get; init; } = null;
}