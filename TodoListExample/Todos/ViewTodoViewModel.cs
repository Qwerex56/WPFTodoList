using System.Collections.ObjectModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using TodoListExample.Data;
using TodoListExample.Enums;
using TodoListExample.TodoList;

namespace TodoListExample.Todos;

public partial class ViewTodoViewModel : ObservableObject, IRecipient<TodoUpdates>, IRecipient<TodoListUpdates> {
    private TodoListContext Context { get; }
    private Func<Todo, TodoItemViewModel> TodoViewModelFactory { get; }

    #region CreationProperties

    public ObservableCollection<TodoItemViewModel> Todos { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string? _todoHeader;

    #endregion


    #region FilterProperties

    // Foolish but simple workaround
    public ObservableCollection<string> FiltersNamCollection { get; } = [
        nameof(TodoListFilterEnum.All),
        nameof(TodoListFilterEnum.Completed),
        nameof(TodoListFilterEnum.Incomplete)
    ];

    [ObservableProperty]
    private string _selectedFilterString = nameof(TodoListFilterEnum.All);

    private TodoListFilterEnum _filter = TodoListFilterEnum.All;

    [ObservableProperty]
    private int? _selectedTodoListId;

    #endregion

    public ViewTodoViewModel(TodoListContext context,
                             Func<Todo, TodoItemViewModel> todoViewModelFactory,
                             IMessenger messenger) {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        TodoViewModelFactory = todoViewModelFactory;

        _selectedTodoListId = Context.TodoLists.FirstOrDefault()?.Id;

        messenger.RegisterAll(this);

        BindingOperations.EnableCollectionSynchronization(Todos, new());
    }

    #region Commands

    [RelayCommand]
    private async Task OnRefresh() {
        try {
            Todos.Clear();

            foreach (var todo in Context.AllTodos.ToList()
                                        .Where(t => _filter.FilterToVisibilityConverter(t))
                                        .Where(t => t.TodoListId == SelectedTodoListId)) {
                Todos.Add(TodoViewModelFactory(todo));
            }
        } catch (Exception e) {
            Console.WriteLine(e);

            throw;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private async Task OnSubmit() {
        if (string.IsNullOrWhiteSpace(TodoHeader)) return;

        var newTodo = new Todo {
            Header = TodoHeader,
            IsCompleted = false,
            TodoListId = SelectedTodoListId,
            TodoList = await Context.TodoLists.FindAsync(SelectedTodoListId)
        };

        Context.AllTodos.Add(newTodo);

        await Context.SaveChangesAsync();
        TodoHeader = string.Empty;

        await OnRefresh();
    }

    [RelayCommand]
    private async Task OnFilterTodos() {
        _filter = SelectedFilterString.StringToTodoListFilterEnum();

        await OnRefresh();
    }

    #endregion

    private bool CanSubmit() => !string.IsNullOrWhiteSpace(TodoHeader) && Context.TodoLists.Any();

    public async void Receive(TodoUpdates messenger) {
        await RefreshCommand.ExecuteAsync(null);
    }

    public async void Receive(TodoListUpdates messenger) {
        SelectedTodoListId = Context.TodoLists.LastOrDefault()?.Id;
        await RefreshCommand.ExecuteAsync(null);
    }
}