using System;
namespace deskpi.src
{
    public interface ITickable<T>
    {
        T Tick(uint currentTime);
        uint Remaining(uint currentTime);
    }
}
