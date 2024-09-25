using System.Windows;
using CommunityToolkit.Mvvm.Input;

namespace TodoListExample.TodoList;

public partial class CreateTodoListEmpty {
    public CreateTodoListEmpty() {
        InitializeComponent();

        DataContext = this;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
        if (DataContext is not CreateTodoListEmptyViewModel viewModel) return;
        
        Visibility = Visibility.Hidden;
        // viewModel
    }
}