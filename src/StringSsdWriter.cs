using System;
using blink.src;

namespace immutableSsd.src
{
    public class StringSsdWriter : ISsdWriter<string>
    {
        public delegate byte GlyphToSegments(Glyph glyph);

        public StringSsdWriter(ISsdWriter<Func<int, byte>> writer, GlyphToSegments converter,
            IGlyphSelector selector)
        {
            this.writer = writer;
            this.converter = converter;
            this.selector = selector;
        }

        public ISsdWriter<string> Write(string text)
           => new StringSsdWriter(writer, converter, selector.SetText(Glyph.FromString(text)));

        public ISsdWriter<string> Tick(uint currentTime)
        {
            var newSelector = selector.Tick(currentTime);
            var newWriter = writer;

            if (selector != newSelector)
            {
                var selected = newSelector.GetSelected();
                newWriter = newWriter.Write((index) =>
                    index >= selected.Count ? (byte)0 : converter(selected[index]));
            }
            newWriter = newWriter.Tick(currentTime);

            if (selector != newSelector || newWriter != writer)
            {
                return new StringSsdWriter(newWriter, converter, newSelector);
            }
            return this;
        }

        public uint Remaining(uint currentTime) =>
            Math.Min(writer.Remaining(currentTime), selector.Remaining(currentTime));

        public int AvailableDigits => writer.AvailableDigits;

        private readonly ISsdWriter<Func<int, byte>> writer;
        private readonly GlyphToSegments converter;
        private readonly IGlyphSelector selector;
    }
}
