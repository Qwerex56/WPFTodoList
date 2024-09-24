using System.Collections.ObjectModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TodoListExample.Data;

namespace TodoListExample.TodoList;

public partial class TodoListViewModel : ObservableObject, IRecipient<TodoListUpdates> {
    public ObservableCollection<TodoListItemViewModel> TodoLists { get; set; } = [];

    private TodoListContext DbContext { get; }
    private IMessenger Messenger { get; }
    private Func<Data.TodoList, TodoListItemViewModel> TodoListViewModelFactory { get; }
    

    public TodoListViewModel(TodoListContext dbContext, IMessenger messenger,
                             Func<Data.TodoList, TodoListItemViewModel> todoListViewModelFactory) {
        DbContext = dbContext;
        Messenger = messenger;
        TodoListViewModelFactory = todoListViewModelFactory;

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

        } catch (Exception e) {
            Console.WriteLine(e);

            throw;
        }
    }

    public void Receive(TodoListUpdates message) {
        // throw new NotImplementedException();
    }
}