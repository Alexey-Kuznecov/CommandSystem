
namespace CommandSystem.Gui.MVVM
{
    public class CommandAvailabilityNotifier
    {
        public event Action? AvailabilityChanged;

        public void Notify()
        {
            AvailabilityChanged?.Invoke();
        }
    }
}
