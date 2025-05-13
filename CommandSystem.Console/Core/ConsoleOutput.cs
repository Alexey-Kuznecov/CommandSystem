
using CommandSystem.Console.Core;

public class ConsoleOutput : IConsoleOutput
{
    public event ConsoleCancelEventHandler CancelKeyPress
    {
        add => Console.CancelKeyPress += value;
        remove => Console.CancelKeyPress -= value;
    }

    public void Write(string message)
    {
        Console.Write(message);
    }
    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }
    public void WriteError(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }
    public void WriteWarning(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public void WriteSuccess(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public void Clear()
    {
        Console.Clear();
    }

    public int CursorTop => Console.CursorTop;

    public int CursorLeft => Console.CursorLeft;

    public int WindowWidth => Console.WindowWidth;

    public void SetCursorPosition(int left, int top)
    {
        Console.SetCursorPosition(left, top);
    }

    public void SetCursorVisible(bool isVisible)
    {
        Console.CursorVisible = isVisible;
    }
}