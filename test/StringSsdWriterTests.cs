using System;
using System.Collections.Immutable;
using blink.src;
using deskpi.src;
using NUnit.Framework;

namespace deskpi.test
{
    [TestFixture]
    public class StringSsdWriterTests
    {
        class TestSsdWriter : ISsdWriter<Func<int, byte>>, ITickable
        {
            public int AvailableDigits => 3;

            public ITickable Write(Func<int, byte> values)
            {
                this.lastValues = values;
                return this;
            }

            public ITickable Tick(int currentTime)
            {
                lastTime = currentTime;
                return this;
            }

            public void TestValues(ImmutableList<byte> expected)
            {
                if (lastValues == null)
                {
                    Assert.Fail();
                }
                else
                {
                    for (int i = 0; i < expected.Count; i++)
                    {
                        Assert.AreEqual(expected[i], lastValues(i));
                    }
                }
            }

            public void TestUnwritten()
            {
                Assert.IsNull(lastValues);
            }

            public void TestTime(int expected)
            {
                if (lastTime == null)
                {
                    Assert.Fail();
                }
                else
                {
                    Assert.AreEqual(expected, lastTime);
                }
            }

            public void Reset()
            {
                lastValues = null;
                lastTime = null;
            }

            private Func<int, byte> lastValues;
            private int? lastTime;
        }

        [TestCase]
        public void WriteString_WritesAndTicks()
        {
            var testWriter = new TestSsdWriter();
            var stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character,
                (ImmutableList<Glyph> text, int availableDigits, int currentTime) 
                    => text);

            var str = "123";
            var tickable = stringWriter.Write(str).Tick(0);

            testWriter.TestValues(ImmutableList<byte>.Empty
                .Add((byte)'1').Add((byte)'2').Add((byte)'3'));
            testWriter.TestTime(0);
        }

        [TestCase]
        public void WriteString_SameContent_DoNotWritesAgain()
        {
            var testWriter = new TestSsdWriter();
            var stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character,
                (ImmutableList<Glyph> text, int availableDigits, int currentTime)
                    => text);

            var str = "123";
            var tickable = stringWriter.Write(str).Tick(0);

            testWriter.Reset();
            tickable = tickable.Tick(0);
            testWriter.TestUnwritten();
        }
    }
}
