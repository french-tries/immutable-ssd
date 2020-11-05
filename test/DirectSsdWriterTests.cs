using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using blink.src;
using deskpi.src;
using NUnit.Framework;

namespace deskpi.test
{
    class TestGpio : IGPIO
    {
        public void Write(Pin pin, bool active)
        {
            written = written.Enqueue((pin, active));
        }

        public void TestWritten(int expectedId, bool expectedActive)
        {
            var (pin, active) = written.Peek();

            Assert.AreEqual(expectedId, pin.Id);
            Assert.AreEqual(expectedActive, active);

            written = written.Dequeue();
        }

        public void TestEmpty()
        {
            Assert.AreEqual(ImmutableQueue<(Pin, bool)>.Empty, written);
        }

        private ImmutableQueue<(Pin, bool)> written = ImmutableQueue<(Pin, bool)>.Empty;
    }

    [TestFixture]
    public class DirectSsdWriterTests
    {
        [TestCase]
        public void ClearSteps_Expects_WriteZerosToAllPins()
        {
            var gpio = new TestGpio();
            var writer = new DirectSsdWriter(
                ImmutableList<Pin>.Empty.Add(new Pin(0, true)),
                ImmutableList<Pin>.Empty.Add(new Pin(2, true)).Add(new Pin(3, true)),
                gpio, 1);

            writer.Clear();

            gpio.TestWritten(2, false);
            gpio.TestWritten(3, false);
            gpio.TestWritten(0, false);
            gpio.TestEmpty();
        }

        [TestCase]
        public void WriteSteps_Expects_ProperSteps()
        {
            var gpio = new TestGpio();
            var writer = new DirectSsdWriter(
                ImmutableList<Pin>.Empty.Add(new Pin(0, true)).Add(new Pin(1, true)),
                ImmutableList<Pin>.Empty.Add(new Pin(2, true)).Add(new Pin(3, true)),
                gpio, 1);

            writer.Write(0b10000000, 0);

            gpio.TestWritten(3, false);
            gpio.TestWritten(0, true);
            gpio.TestWritten(1, false);
            gpio.TestWritten(2, true);
            gpio.TestEmpty();
        }

        [TestCase]
        public void WriteSteps_WenInvalidPin_ClearSteps()
        {
            var gpio = new TestGpio();
            var writer = new DirectSsdWriter(
                ImmutableList<Pin>.Empty.Add(new Pin(0, true)),
                ImmutableList<Pin>.Empty.Add(new Pin(2, true)).Add(new Pin(3, true)),
                gpio, 1);

            writer.Write(0b10000000, 2);
            gpio.TestWritten(2, false);
            gpio.TestWritten(3, false);
            gpio.TestWritten(0, false);
            gpio.TestEmpty();
        }

        [TestCase]
        public void CycleSteps_Expects_ToCycle()
        {
            var values = new List<byte>
            {
                0b01000000,
                0b10000000
            };
            var gpio = new TestGpio();
            var writer = new DirectSsdWriter(
                ImmutableList<Pin>.Empty.Add(new Pin(0, true)).Add(new Pin(1, true)),
                ImmutableList<Pin>.Empty.Add(new Pin(2, true)).Add(new Pin(3, true)),
                gpio, 5);

            var tickable = writer.Write(
                (i) =>
                {
                    if (i >= values.Count) return 0;
                    return values[i];
                }).Tick(0);
            gpio.TestWritten(3, false);
            gpio.TestWritten(0, false);
            gpio.TestWritten(1, true);
            gpio.TestWritten(2, true);
            gpio.TestEmpty();

            tickable = tickable.Tick(5);
            gpio.TestWritten(2, false);
            gpio.TestWritten(0, true);
            gpio.TestWritten(1, false);
            gpio.TestWritten(3, true);
            gpio.TestEmpty();

            tickable = tickable.Tick(10);
            gpio.TestWritten(3, false);
            gpio.TestWritten(0, false);
            gpio.TestWritten(1, true);
            gpio.TestWritten(2, true);
            gpio.TestEmpty();
        }
    }
}
