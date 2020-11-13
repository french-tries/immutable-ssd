using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using blink.src;
using immutableSsd.src;
using NUnit.Framework;

namespace immutableSsd.test
{
    class TestGpio
    {
        public void Write(Step step)
        {
            if (step is WriteStep writeStep)
            {
                written = written.Enqueue(writeStep);
            }
            else
            {
                Assert.Fail();
            }
        }

        public void TestWritten(int expectedId, bool expectedActive)
        {
            var step = written.Peek();

            Assert.AreEqual(expectedId, step.Pin.Id);
            Assert.AreEqual(expectedActive, step.Value);

            written = written.Dequeue();
        }

        public void TestEmpty()
        {
            Assert.AreEqual(ImmutableQueue<WriteStep>.Empty, written);
        }

        private ImmutableQueue<WriteStep> written = ImmutableQueue<WriteStep>.Empty;
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
                gpio.Write, 1);

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
                gpio.Write, 1);

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
                gpio.Write, 1);

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
                gpio.Write, 5);

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
