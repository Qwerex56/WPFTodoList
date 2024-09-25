using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TodoListExample.Data;
using TodoListExample.Enums;

namespace TodoListExample.TodoList;

public partial class TodoListItemViewModel : ObservableObject {
    public int Id { get; }

    [MaxLength(32)] 
    public string Title { get; set; }

    private TodoListContext DbContext { get; }
    private IMessenger Messenger { get; }

    public TodoListItemViewModel(Data.TodoList todoList, TodoListContext dbContext, IMessenger messenger) {
        Id = todoList.Id;
        Title = todoList.Title;
        
        DbContext = dbContext;
        Messenger = messenger;
    }

    [RelayCommand]
    public async Task OnDelete() {
        if (await DbContext.TodoLists.FindAsync(Id) is not { } foundList) {
            return;
        }

        var todosInList = DbContext.AllTodos.Where(t => t.TodoListId == Id);
        DbContext.AllTodos.RemoveRange(todosInList);
        DbContext.TodoLists.Remove(foundList);

        await DbContext.SaveChangesAsync();
        Messenger.Send(new TodoListUpdates(foundList, UpdateTypeEnum.Update));
    }

    [RelayCommand]
    public async Task OnSelectTodoList() {
        if (await DbContext.TodoLists.FindAsync(Id) is not { } foundList) {
            return;
        }
        
        Messenger.Send(new TodoListUpdates(foundList, UpdateTypeEnum.Update));
    }
}

public record TodoListUpdates(Data.TodoList TodoList, UpdateTypeEnum UpdateType);