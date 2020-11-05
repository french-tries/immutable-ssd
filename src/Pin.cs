using System;
namespace deskpi.src
{
    public class Pin
    {
        public Pin(int id, bool activeHigh)
        {
            Id = id;
            ActiveHigh = activeHigh;
        }

        public int Id { get; }
        public bool ActiveHigh { get;  }
    }
}
