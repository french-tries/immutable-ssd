using deskpi.src;

namespace blink.src
{
    public interface ISsdWriter<T>
    {
        int AvailableDigits { get; }

        ITickable Write(T values);
    }
}