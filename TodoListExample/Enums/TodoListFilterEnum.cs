using TodoListExample.Data;

namespace TodoListExample.Enums;

public enum TodoListFilterEnum {
    All,
    Completed,
    Incomplete,
}

public static class TodoListFilterExtension {
    public static bool FilterToVisibilityConverter(this TodoListFilterEnum filter, Todo todo) {
        return filter switch {
            TodoListFilterEnum.All => true,
            TodoListFilterEnum.Completed => todo.IsCompleted,
            TodoListFilterEnum.Incomplete => !todo.IsCompleted,
            _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
        };
    }

    public static TodoListFilterEnum StringToTodoListFilterEnum(this string filterName) {
        var success = Enum.TryParse<TodoListFilterEnum>(filterName, true, out var result);

        return success ? result : TodoListFilterEnum.All;
    }
}