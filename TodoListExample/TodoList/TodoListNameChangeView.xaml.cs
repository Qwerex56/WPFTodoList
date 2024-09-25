using System.Windows;
using System.Windows.Controls;

namespace TodoListExample.TodoList;

public partial class TodoListNameChangeView : UserControl {
    public TodoListNameChangeView() {
        InitializeComponent();
    }

    private async void TodoListNameChangeView_OnLoaded(object sender, RoutedEventArgs e) {
        if (DataContext is TodoListViewModel viewModel) {
            await viewModel.RefreshCommand.ExecuteAsync(null);
            TodoListName.Text = viewModel.GetTodoListName();
        }
    }
}