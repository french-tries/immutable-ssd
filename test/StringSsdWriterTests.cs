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
        static TestSelector newInstance = new TestSelector();

        class TestSelector : IGlyphSelector
        {
            public ImmutableList<Glyph> GetSelected() => text;

            public IGlyphSelector SetText(ImmutableList<Glyph> text)
            {
                this.text = text;
                return this;
            }

            public IGlyphSelector Tick(uint currentTime)
            {
                if (CreateNew)
                {
                    newInstance.text = text;
                    return newInstance;
                }
                return this;
            }

            public bool CreateNew { get; set; }

            private ImmutableList<Glyph> text;
        }

        class TestSsdWriter : ISsdWriter<Func<int, byte>>
        {
            public int AvailableDigits => 3;

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

            public ISsdWriter<Func<int, byte>> Write(Func<int, byte> values)
            {
                lastValues = values;
                return this;
            }

            public ISsdWriter<Func<int, byte>> Tick(uint currentTime)
            {
                lastTime = currentTime;
                return this;
            }

            private Func<int, byte> lastValues;
            private uint? lastTime;
        }

        [TestCase]
        public void WriteString_WritesAndTicks()
        {
            var testWriter = new TestSsdWriter();
            var testSelector = new TestSelector();
            var stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character, testSelector);

            var str = "123";
            testSelector.CreateNew = true;
            var tickable = stringWriter.Write(str).Tick(0);

            testWriter.TestValues(ImmutableList<byte>.Empty
                .Add((byte)'1').Add((byte)'2').Add((byte)'3'));
            testWriter.TestTime(0);
        }

        [TestCase]
        public void WriteString_SameContent_DoNotWritesAgain()
        {
            var testWriter = new TestSsdWriter();
            var testSelector = new TestSelector();
            var stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character, testSelector);

            var str = "123";
            var tickable = stringWriter.Write(str).Tick(0);

            testWriter.Reset();
            tickable = tickable.Tick(0);
            testWriter.TestUnwritten();
        }
    }
}
