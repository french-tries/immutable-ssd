using System;
namespace deskpi.src
{
    public interface IWriter<T>
    {
        ITickable Write(T value);
    }
}
