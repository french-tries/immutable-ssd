using System.Collections.Generic;
using System.Collections.Immutable;
using blink.src;
using deskpi.src;
using NUnit.Framework;

namespace deskpi.test
{
    public static class StepTester
    {
        public static ImmutableList<Step> TestEmpty(this ImmutableList<Step> steps)
        {
            Assert.AreEqual(ImmutableList<Step>.Empty, steps);
            return steps;
        }

        public static ImmutableList<Step> TestWriteStep(this ImmutableList<Step> steps, uint pin, bool value)
        {
            if (steps[0] is WriteStep writeStep)
            {
                Assert.AreEqual(pin, writeStep.Pin);
                Assert.AreEqual(value, writeStep.Value);
            }
            else
            {
                Assert.Fail();
            }
            return steps.RemoveAt(0);
        }

        public static ImmutableList<Step> TestSleepStep(this ImmutableList<Step> steps, int length)
        {
            if (steps[0] is SleepStep sleepStep)
            {
                Assert.AreEqual(length, sleepStep.Length);
            }
            else
            {
                Assert.Fail();
            }
            return steps.RemoveAt(0);
        }
    }

    [TestFixture]
    public class SsdWriterTests
    {
        [TestCase]
        public void ClearSteps_Expects_WriteZerosToAllPins()
        {
            var writer = new SsdWriter(
                ImmutableList<uint>.Empty.Add(0),
                ImmutableList<uint>.Empty.Add(2).Add(3),
                1);
            var steps = ImmutableList<Step>.Empty;

            writer.ClearSteps(steps)
                .TestWriteStep(2, false)
                .TestWriteStep(3, false)
                .TestWriteStep(0, false)
                .TestEmpty();
        }

        [TestCase]
        public void WriteSteps_Expects_ProperSteps()
        {
            var writer = new SsdWriter(
                ImmutableList<uint>.Empty.Add(0).Add(1),
                ImmutableList<uint>.Empty.Add(2).Add(3),
                1);
            var steps = ImmutableList<Step>.Empty;

            writer.WriteSteps(0b10000000, 0, steps)
                .TestWriteStep(3, false)
                .TestWriteStep(0, true)
                .TestWriteStep(1, false)
                .TestWriteStep(2, true)
                .TestEmpty();
        }

        [TestCase]
        public void WriteSteps_WenInvalidPin_ClearSteps()
        {
            var writer = new SsdWriter(
                ImmutableList<uint>.Empty.Add(0),
                ImmutableList<uint>.Empty.Add(2).Add(3),
                1);
            var steps = ImmutableList<Step>.Empty;

            writer.WriteSteps(0b10000000, 2, steps)
                .TestWriteStep(2, false)
                .TestWriteStep(3, false)
                .TestWriteStep(0, false)
                .TestEmpty();
        }

        [TestCase]
        public void CycleSteps_Expects_ToCycle()
        {
            var values = new List<byte>
            {
                0b01000000,
                0b10000000
            };
            var writer = new SsdWriter(
                ImmutableList<uint>.Empty.Add(0).Add(1),
                ImmutableList<uint>.Empty.Add(2).Add(3),
                5);
            var steps = ImmutableList<Step>.Empty;

            steps = writer.CycleSteps(
                (i) =>
                {
                    if (i >= values.Count) return 0;
                    return values[i];
                }, 
                steps);
            steps = steps.TestWriteStep(3, false); 
            steps = steps.TestWriteStep(0, false);
            steps = steps.TestWriteStep(1, true);
            steps = steps.TestWriteStep(2, true);
            steps = steps.TestSleepStep(5);
            steps = steps.TestWriteStep(2, false);
            steps = steps.TestWriteStep(0, true);
            steps = steps.TestWriteStep(1, false);
            steps = steps.TestWriteStep(3, true);
            steps = steps.TestSleepStep(5);
            steps = steps.TestEmpty();
        }
    }
}
