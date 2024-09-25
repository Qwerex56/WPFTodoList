using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TodoListExample.Enums;

namespace TodoListExample.TodoList;

public partial class CreateTodoListEmptyViewModel : ObservableObject {
    private IMessenger Messenger { get; }

    public CreateTodoListEmptyViewModel(IMessenger messenger) {
        Messenger = messenger;
    }

    [RelayCommand]
    public void OnCreateFirstTodo() {
        var todoList = new Data.TodoList {
            Title = "My first todo list"
        };

        Messenger.Send(new TodoListUpdates(todoList, UpdateTypeEnum.Create));
    }
}