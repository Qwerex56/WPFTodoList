using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TodoListExample.Todos;

public partial class ViewTodoView : UserControl {
    public ViewTodoView() {
        InitializeComponent();
    }


    private async void ViewTodoView_OnLoaded(object sender, RoutedEventArgs e) {
        if (DataContext is ViewTodoViewModel viewModel) {
            await viewModel.RefreshCommand.ExecuteAsync(null);
        }
    }
}