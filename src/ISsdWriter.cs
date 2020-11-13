using immutableSsd.src;

namespace blink.src
{
    public interface ISsdWriter<TPARAM> : ITickable<ISsdWriter<TPARAM>>
    {
        int AvailableDigits { get; }
        ISsdWriter<TPARAM> Write(TPARAM values);
    }
}