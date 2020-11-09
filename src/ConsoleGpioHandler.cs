using System;
using System.Diagnostics;

namespace deskpi.src
{
    public class ConsoleGpioHandler
    {
        public void Handle(Step step)
        {
            switch (step)
            {
                case WriteStep writeStep:
                    Console.WriteLine($"Writing {writeStep.Value == writeStep.Pin.ActiveHigh} to {writeStep.Pin.Id}");
                    break;
                default:
                    Debug.Fail("Unrecognised step");
                    break;
            }
        }
    }
}
