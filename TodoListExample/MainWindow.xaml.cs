using System.Windows.Input;
using TodoListExample.TodoList;
using TodoListExample.Todos;

namespace TodoListExample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow {
    public MainWindow(ViewTodoViewModel viewTodoViewModel, TodoListViewModel todoListViewModel) {
        InitializeComponent();

        ViewTodoView.DataContext = viewTodoViewModel;
        AddTodoView.DataContext = viewTodoViewModel;
        TodoListNameChangeView.DataContext = todoListViewModel;

        CommandBindings.Add(new (ApplicationCommands.Close, OnClose));
    }

    private void OnClose(object sender, ExecutedRoutedEventArgs e) {
        Close();
    }
}