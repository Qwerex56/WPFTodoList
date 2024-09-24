using System.ComponentModel.DataAnnotations;

namespace TodoListExample.Data;

public class TodoList {
    [Key]
    public int Id { get; init; }

    [MaxLength(32)] 
    public string Title { get; set; } = "New todo list";

}