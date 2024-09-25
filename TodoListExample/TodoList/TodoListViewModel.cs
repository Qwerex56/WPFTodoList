using System.Collections.ObjectModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using TodoListExample.Data;
using TodoListExample.Enums;

namespace TodoListExample.TodoList;

public partial class TodoListViewModel : ObservableObject, IRecipient<TodoListUpdates> {
    public ObservableCollection<TodoListItemViewModel> TodoLists { get; set; } = [];

    private TodoListContext DbContext { get; }
    private IMessenger Messenger { get; }
    private Func<Data.TodoList, TodoListItemViewModel> TodoListViewModelFactory { get; }

    [ObservableProperty]
    private int _selectedTodoListId;

    [ObservableProperty]
    private string _selectedTodoListName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _createTodoListName = string.Empty;

    public TodoListViewModel(TodoListContext dbContext, IMessenger messenger,
                             Func<Data.TodoList, TodoListItemViewModel> todoListViewModelFactory) {
        DbContext = dbContext;
        Messenger = messenger;
        TodoListViewModelFactory = todoListViewModelFactory;

        var firstList = DbContext.TodoLists.FirstOrDefault();

        if (firstList is not null) {
            _selectedTodoListId = firstList.Id;
        }

        messenger.RegisterAll(this);

        BindingOperations.EnableCollectionSynchronization(TodoLists, new());
    }

    [RelayCommand]
    public async Task OnRefresh() {
        try {
            TodoLists.Clear();

            await foreach (var todoList in DbContext.TodoLists.AsAsyncEnumerable()) {
                TodoLists.Add(TodoListViewModelFactory(todoList));
            }

            SelectedTodoListName = GetTodoListName();
        } catch (Exception e) {
            Console.WriteLine(e);

            throw;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private async Task OnSubmit() {
        var newTodoList = new Data.TodoList {
            Title = CreateTodoListName,
        };

        DbContext.TodoLists.Add(newTodoList);

        CreateTodoListName = string.Empty;
        await DbContext.SaveChangesAsync();
        await RefreshCommand.ExecuteAsync(null);
    }

    private bool CanSubmit() => !string.IsNullOrWhiteSpace(CreateTodoListName);

    public async void Receive(TodoListUpdates message) {
        switch (message.UpdateType) {
            case UpdateTypeEnum.Update:
                SelectedTodoListId = message.TodoList.Id;

                break;
            case UpdateTypeEnum.Delete:
                SelectedTodoListId = message.TodoList.Id;

                break;
            case UpdateTypeEnum.Create:
                var firstList = DbContext.TodoLists.FirstOrDefault();

                if (firstList is not null) {
                    SelectedTodoListId = firstList.Id;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        await RefreshCommand.ExecuteAsync(null);
    }
    
    public string GetTodoListName() {
        if (TodoLists.FirstOrDefault(t => t.Id == SelectedTodoListId) is { } foundList) {
            return foundList.Title;
        }

        return string.Empty;
    }
}