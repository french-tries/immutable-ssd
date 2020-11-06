using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace deskpi.src
{
    public class GpioHandler
    {
        public GpioHandler()
        {
            Pi.Init<BootstrapWiringPi>();
            pins = new Dictionary<Pin, IGpioPin>();
        }

        private void Write(WriteStep step)
        {
            if(!pins.ContainsKey(step.Pin))
            {
                var gpioPin = Pi.Gpio[step.Pin.Id];
                gpioPin.PinMode = GpioPinDriveMode.Output;

                pins.Add(step.Pin, gpioPin);
            }
            pins[step.Pin].Write(step.Value == step.Pin.ActiveHigh);
        }

        public void Handle(Step step)
        {
            switch(step)
            {
                case WriteStep writeStep:
                    Write(writeStep);
                    break;
                default:
                    Debug.Fail("Unrecognised step");
                    break;
            }
        }

        private readonly Dictionary<Pin, IGpioPin> pins;
    }
}
