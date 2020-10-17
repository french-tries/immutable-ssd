using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using blink.src;
using deskpi.src;
using NUnit.Framework;

namespace deskpi.test
{
    [TestFixture]
    public class SsdWriterTests
    {
        class TestApplier
        {
            public TestApplier()
            {
                Steps = new List<Step>();
            }

            public List<Step> Steps { get; }

            public void Apply(Step step)
            {
                Steps.Add(step);
            }

            public void TestWriteStep(int id,uint pin, bool value)
            {
                if (Steps[id] is WriteStep writeStep)
                {
                    Assert.IsTrue(pin == writeStep.Pin && value == writeStep.Value);
                }
                else
                {
                    Assert.Fail();
                }
            }

            public void TestSleepStep(int id, int length)
            {
                if (Steps[id] is SleepStep sleepStep)
                {
                    Assert.IsTrue(length == sleepStep.Length);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }

        [TestCase]
        public void ClearSteps_Expects_WriteZerosToAllPins()
        {
            var applier = new TestApplier();
            var writer = new SsdWriter(
                applier.Apply,
                ImmutableList<uint>.Empty.Add(0),
                ImmutableList<uint>.Empty.Add(2).Add(3)
                );
            writer.ClearSteps();

            applier.TestWriteStep(0, 2, false);
            applier.TestWriteStep(1, 3, false);
            applier.TestWriteStep(2, 0, false);

            Assert.IsTrue(applier.Steps.Count == 3);
        }

        [TestCase]
        public void WriteSteps_Expects_ProperSteps()
        {
            var applier = new TestApplier();
            var writer = new SsdWriter(
                applier.Apply,
                ImmutableList<uint>.Empty.Add(0).Add(1),
                ImmutableList<uint>.Empty.Add(2).Add(3)
                );
            writer.WriteSteps(0b10000000, 0);

            applier.TestWriteStep(0, 3, false);
            applier.TestWriteStep(1, 0, true);
            applier.TestWriteStep(2, 1, false);
            applier.TestWriteStep(3, 2, true);

            Assert.IsTrue(applier.Steps.Count == 4);
        }

        [TestCase]
        public void WriteSteps_WenInvalidPin_ClearSteps()
        {
            var applier = new TestApplier();
            var writer = new SsdWriter(
                applier.Apply,
                ImmutableList<uint>.Empty.Add(0),
                ImmutableList<uint>.Empty.Add(2).Add(3)
                );
            writer.WriteSteps(0b10000000, 2);

            applier.TestWriteStep(0, 2, false);
            applier.TestWriteStep(1, 3, false);
            applier.TestWriteStep(2, 0, false);

            Assert.IsTrue(applier.Steps.Count == 3);
        }

        [TestCase]
        public void CycleSteps_Expects_ToCycle()
        {
            var values = new List<byte>
            {
                0b01000000,
                0b10000000
            };
            var applier = new TestApplier();
            var writer = new SsdWriter(
                applier.Apply,
                ImmutableList<uint>.Empty.Add(0).Add(1),
                ImmutableList<uint>.Empty.Add(2).Add(3)
                );
            writer.CycleSteps(
                (i) => {
                    if (i >= values.Count) return 0;
                    else return values[i];
                },
                5);

            applier.TestWriteStep(0, 3, false);
            applier.TestWriteStep(1, 0, false);
            applier.TestWriteStep(2, 1, true);
            applier.TestWriteStep(3, 2, true);

            applier.TestSleepStep(4, 5);

            applier.TestWriteStep(5, 2, false);
            applier.TestWriteStep(6, 0, true);
            applier.TestWriteStep(7, 1, false);
            applier.TestWriteStep(8, 3, true);

            applier.TestSleepStep(9, 5);

            Assert.IsTrue(applier.Steps.Count == 10);
        }
    }
}
