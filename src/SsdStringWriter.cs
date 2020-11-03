using System;
using System.Collections.Immutable;
using blink.src;

namespace deskpi.src
{
    public class SsdStringWriter
    {
        public class Operation
        {
            public Operation(SsdStringWriter textWriter, string text)
            {
                this.textWriter = textWriter;
                this.text = text;
            }

            // @todo caching
            public Operation Tick(int currentTime)
            {
                var glyphs = Glyph.FromString(text);
                var selected = textWriter.selector.GetSelected(glyphs, textWriter.writer.AvailableDigits,
                    currentTime);
                var writeSteps = textWriter.writer.CycleSteps(
                    (index) => textWriter.converter.GetSegments(selected[index]),
                    ImmutableList<Step>.Empty);
                textWriter.applier(writeSteps);

                return this;
            }

            private readonly SsdStringWriter textWriter;
            private readonly string text;
        }

        public SsdStringWriter(SsdWriter writer, SegmentsConverter converter,
            ScrollingGlyphSelector selector, Action<ImmutableList<Step>> applier)
        {
            this.writer = writer;
            this.converter = converter;
            this.selector = selector;
            this.applier = applier;
        }

        public Operation Write(string text)
        {
            return new Operation(this, text);
        }

        private readonly SsdWriter writer;
        private readonly SegmentsConverter converter;
        private readonly ScrollingGlyphSelector selector;
        private readonly Action<ImmutableList<Step>> applier;
    }
}
