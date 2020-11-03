using System;
using System.Collections.Immutable;

namespace deskpi.src
{
    public class ScrollingGlyphSelector
    {
        public ScrollingGlyphSelector(int delay, int endsDelay, int startTime)
        {
            this.delay = delay;
            this.endsDelay = endsDelay;
            this.startTime = startTime;
        }

        private int GetOffset(ImmutableList<Glyph> text, int availableDigits, int currentTime)
        {
            if (text.Count <= availableDigits)
            {
                return 0;
            }

            var steps = text.Count - availableDigits + 1;

            var period = 2 * endsDelay + delay * (steps - 2);

            var modulo = (currentTime - startTime) % period;

            int offset = 0;

            if (modulo + endsDelay > period)
            {
                offset = steps - 1;
            }
            else if (modulo >= endsDelay)
            {
                offset++;
                modulo -= endsDelay;

                offset += modulo / delay;
            }

            return offset;
        }


        public ImmutableList<Glyph> GetSelected(ImmutableList<Glyph> text,
            int availableDigits, int currentTime)
        {
            var offset = GetOffset(text, availableDigits, currentTime);

            if (text.Count <= offset)
            {
                return ImmutableList<Glyph>.Empty;
            }

            return text.GetRange(offset, Math.Min(availableDigits, text.Count - offset));
        }

        private readonly int delay;
        private readonly int endsDelay;
        private readonly int startTime;
    }
}
