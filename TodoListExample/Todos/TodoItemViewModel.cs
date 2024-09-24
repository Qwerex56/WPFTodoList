using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TodoListExample.Data;
using TodoListExample.Enums;

namespace TodoListExample.Todos;

public partial class TodoItemViewModel : ObservableObject {
    private int Id { get; }
    
    [MaxLength(64)]
    public string Header { get; set; }
    public bool IsCompleted { get; set; }
    public int? TodoListId { get; }
    public Data.TodoList? TodoList { get; }

    private TodoListContext TodoListContext { get; }
    private IMessenger Messenger { get; }
    
    public TodoItemViewModel(Todo todo, TodoListContext todoListContext, IMessenger messenger) {
        Id = todo.Id;
        Header = todo.Header;
        IsCompleted = todo.IsCompleted;
        TodoListId = todo.TodoListId;
        TodoList = todo.TodoList;

        TodoListContext = todoListContext;
        Messenger = messenger;
    }

    [RelayCommand]
    private async Task OnDelete() {
        if (await TodoListContext.AllTodos.FindAsync(Id) is { } foundTodo) {
            TodoListContext.AllTodos.Remove(foundTodo);
            await TodoListContext.SaveChangesAsync();
            Messenger.Send(new TodoUpdates(foundTodo));
        }
    }

    [RelayCommand]
    private async Task OnEdit(string? newHeader = null) {
        if (string.IsNullOrWhiteSpace(newHeader)) return;
        
        if (await TodoListContext.AllTodos.FindAsync(Id) is { } foundTodo) {
            foundTodo.Header = newHeader;
            TodoListContext.AllTodos.Update(foundTodo);
            await TodoListContext.SaveChangesAsync();
            Messenger.Send(new TodoUpdates(foundTodo));
        }
    }

    [RelayCommand]
    private async Task OnTodoCompleted() {
        if (await TodoListContext.AllTodos.FindAsync(Id) is { } foundTodo) {
            foundTodo.IsCompleted = !foundTodo.IsCompleted;
            TodoListContext.AllTodos.Update(foundTodo);
            await TodoListContext.SaveChangesAsync();
            Messenger.Send(new TodoUpdates(foundTodo));
        }
    }
}

public record TodoUpdates(Todo Todo);