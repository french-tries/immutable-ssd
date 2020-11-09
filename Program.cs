using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using blink.src;
using deskpi.src;
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
            Pi.Init<BootstrapWiringPi>();

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

        public static void HelloWorld()
        {
            var gpioHandler = new GpioHandler();
            var segment_pins = ImmutableList<Pin>.Empty
                .Add(new Pin(21, false)).Add(new Pin(20, false))
                .Add(new Pin(16, false)).Add(new Pin(12, false))
                .Add(new Pin(7, false)).Add(new Pin(8, false))
                .Add(new Pin(25, false)).Add(new Pin(24, false));

            var digit_pins = ImmutableList<Pin>.Empty
                .Add(new Pin(26, false)).Add(new Pin(19, false))
                .Add(new Pin(13, false)).Add(new Pin(6, false))
                .Add(new Pin(5, false)).Add(new Pin(11, false))
                .Add(new Pin(9, false)).Add(new Pin(10, false));

            var directWriter =
                new DirectSsdWriter(segment_pins, digit_pins, gpioHandler.Handle, 500);

            var converter = new SegmentsConverter();

            var selector = new ScrollingGlyphSelector(100000, 200000, digit_pins.Count);

            ISsdWriter<string> stringWriter = 
                new StringSsdWriter(directWriter,converter.GetSegments, selector);

            stringWriter = stringWriter.Write("Hello world please work");

            uint millis = 0;
            uint step = 100;
            while (true)
            {
                stringWriter = stringWriter.Tick(millis);

                gpioHandler.SleepMillis(step);
                millis += step;
            }
        }

        public static void Main(string[] args)
        {
            HelloWorld();
            // TestLedBlinking();
            //Console.WriteLine("Hello World!");
        }
    }
}
