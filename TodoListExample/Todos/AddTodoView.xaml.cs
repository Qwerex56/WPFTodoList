using System.Windows.Controls;

namespace TodoListExample.Todos;

public partial class AddTodoView : UserControl {
    public AddTodoView() {
        InitializeComponent();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (DataContext is ViewTodoViewModel viewModel) {
            viewModel.FilterTodosCommand.ExecuteAsync(null);
        }
    }
}