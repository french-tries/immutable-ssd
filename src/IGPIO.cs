using System;
namespace deskpi.src
{
    public interface IGPIO
    {
        void Write(Pin pin, bool active);
    }
}
