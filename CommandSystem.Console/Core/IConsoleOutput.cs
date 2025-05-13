
namespace CommandSystem.Console.Core
{
    public interface IConsoleOutput
    {
        void Write(string message);
        void WriteLine(string message);
        public void WriteError(string message);
        public void WriteWarning(string message);
        public void WriteSuccess(string message);
        void Clear();

        int CursorTop { get; }
        int CursorLeft { get; }
        int WindowWidth { get; }

        event ConsoleCancelEventHandler CancelKeyPress;
        public void SetCursorVisible(bool isVisible);
        void SetCursorPosition(int left, int top);
    }
}
