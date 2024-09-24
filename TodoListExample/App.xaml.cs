using CommunityToolkit.Mvvm.Messaging;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using TodoListExample.Data;
using TodoListExample.TodoList;
using TodoListExample.Todos;

namespace TodoListExample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    [STAThread]
    private static void Main(string[] args) {
        MainAsync(args).GetAwaiter().GetResult();
    }

    private static async Task MainAsync(string[] args) {
        using var host = CreateHostBuilder(args).Build();
        await host.StartAsync().ConfigureAwait(true);

        using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            await using (var todoListContext = scope.ServiceProvider.GetRequiredService<TodoListContext>()) {
                await todoListContext.Database.MigrateAsync();
            }

        App app = new();
        app.InitializeComponent();
        app.MainWindow = host.Services.GetRequiredService<MainWindow>();
        app.MainWindow.Visibility = Visibility.Visible;
        app.Run();

        await host.StopAsync().ConfigureAwait(true);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder)
                => configurationBuilder.AddUserSecrets(typeof(App).Assembly))
            .ConfigureServices((hostContext, services) => {
                services.AddSingleton<MainWindow>();

                services.AddSingleton<ViewTodoViewModel>();
                services.AddSingleton<TodoListViewModel>();

                services.AddSingleton<Func<Todo, TodoItemViewModel>>(serviceProvider => {
                    return todo => new(todo,
                        serviceProvider.GetRequiredService<TodoListContext>(),
                        serviceProvider.GetRequiredService<IMessenger>());
                });

                services.AddSingleton<Func<Data.TodoList, TodoListItemViewModel>>(serviceProvider => {
                    return todoList => new(todoList, serviceProvider.GetRequiredService<TodoListContext>(),
                        serviceProvider.GetRequiredService<IMessenger>());
                });

                services.AddSingleton<WeakReferenceMessenger>();
                services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider =>
                    provider.GetRequiredService<WeakReferenceMessenger>());

                services.AddDbContext<TodoListContext>(options => { options.UseSqlite("Data Source=todolist.db"); });

                services.AddSingleton(_ => Current.Dispatcher);

                services.AddTransient<ISnackbarMessageQueue>(provider => {
                    Dispatcher dispatcher = provider.GetRequiredService<Dispatcher>();

                    return new SnackbarMessageQueue(TimeSpan.FromSeconds(3.0), dispatcher);
                });
            });
}