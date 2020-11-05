using System;
namespace deskpi.src
{
    public interface ITickable
    {
        ITickable Tick(int currentTime);
    }
}
