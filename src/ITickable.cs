using System;
namespace immutableSsd.src
{
    public interface ITickable<T>
    {
        T Tick(uint currentTime);
        uint Remaining(uint currentTime);
    }
}
