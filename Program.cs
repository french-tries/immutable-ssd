using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

// segment_pins = [ 21, 20, 16, 12, 7, 8, 25, 24 ]
// digit_pins = [ 26, 19, 13, 6, 5, 11, 9, 10 ]

namespace blink
{
    class MainClass
    {
        public static void TestLedBlinking()
        {
            var digitPin = Pi.Gpio[26];
            digitPin.PinMode = GpioPinDriveMode.Output;
            digitPin.Write(false);

            // Get a reference to the pin you need to use.
            // Both methods below are equivalent
            var blinkingPin = Pi.Gpio[21];
            //blinkingPin = Pi.Gpio[BcmPin.Gpio17];

            // Configure the pin as an output
            blinkingPin.PinMode = GpioPinDriveMode.Output;

            // perform writes to the pin by toggling the isOn variable
            var isOn = false;
            while (true)
            {
                isOn = !isOn;
                blinkingPin.Write(isOn);
                System.Threading.Thread.Sleep(500);
            }
        }

        public static void Main(string[] args)
        {
            Pi.Init<BootstrapWiringPi>();
            TestLedBlinking();
            //Console.WriteLine("Hello World!");
        }
    }
}
