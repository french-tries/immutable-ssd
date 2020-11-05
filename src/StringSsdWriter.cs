using System;
using System.Collections.Immutable;
using System.Linq;
using blink.src;

namespace deskpi.src
{
    public class StringSsdWriter : ISsdWriter<string>
    {
        public delegate ImmutableList<Glyph> GlyphSelector(ImmutableList<Glyph> text,
            int availableDigits, int currentTime);

        public delegate byte GlyphToSegments(Glyph glyph);

        private class Tickable : ITickable
        {
            public Tickable(StringSsdWriter textWriter, ImmutableList<Glyph> text)
            {
                this.stringWriter = textWriter;
                this.text = text;
                this.lastSelected = ImmutableList<Glyph>.Empty;
                this.writeTickable = null;
            }

            private Tickable(StringSsdWriter textWriter, ImmutableList<Glyph> text,
                ImmutableList<Glyph> lastSelected, ITickable writeTickable)
            {
                this.stringWriter = textWriter;
                this.text = text;
                this.lastSelected = lastSelected;
                this.writeTickable = writeTickable;
            }

            public ITickable Tick(int currentTime)
            {
                // @todo better caching
                var selected = stringWriter.selector(
                    text, stringWriter.writer.AvailableDigits, currentTime);

                if (writeTickable == null || !selected.SequenceEqual(lastSelected))
                {
                    var writeOp = stringWriter.writer.Write(
                    (index) => stringWriter.converter(selected[index]))
                        .Tick(currentTime);

                    return new Tickable(stringWriter, text, selected, writeOp);
                }

                writeTickable.Tick(currentTime);
                return this;
            }

            private readonly ImmutableList<Glyph> lastSelected;
            private readonly StringSsdWriter stringWriter;
            private readonly ImmutableList<Glyph> text;
            private readonly ITickable writeTickable;
        }

        public StringSsdWriter(ISsdWriter<Func<int, byte>> writer, GlyphToSegments converter,
            GlyphSelector selector)
        {
            this.writer = writer;
            this.converter = converter;
            this.selector = selector;
        }

        public ITickable Write(string text) 
            => new Tickable(this, Glyph.FromString(text));

        public int AvailableDigits => writer.AvailableDigits;

        private readonly ISsdWriter<Func<int, byte>> writer;
        private readonly GlyphToSegments converter;
        private readonly GlyphSelector selector;
    }
}
