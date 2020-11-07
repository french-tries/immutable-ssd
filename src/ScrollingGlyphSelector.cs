using System;
using System.Collections.Immutable;

namespace deskpi.src
{
    public class ScrollingGlyphSelector : IGlyphSelector
    {
        public ScrollingGlyphSelector(uint delay, uint endsDelay, int availableDigits)
        {
            this.delay = delay;
            this.endsDelay = endsDelay;
            this.availableDigits = availableDigits;
            this.text = ImmutableList<Glyph>.Empty;
            this.offset = 0;
            this.timer = new ImmutableTimer(this.endsDelay);
        }


        private ScrollingGlyphSelector(uint delay, uint endsDelay, int availableDigits,
                ImmutableList<Glyph> text, int offset, ImmutableTimer timer = null)
        {
            this.delay = delay;
            this.endsDelay = endsDelay;
            this.availableDigits = availableDigits;
            this.text = text ?? ImmutableList<Glyph>.Empty;
            this.offset = offset;
            this.timer = timer ?? new ImmutableTimer(this.endsDelay);
        }

        public IGlyphSelector Tick(uint currentTime)
        {
            var newTimer = timer.Tick(currentTime);
            if (timer == newTimer)
            {
                return this;
            }

            var newOffset = offset + 1;
            if (newOffset > text.Count - availableDigits) newOffset = 0;

            var newInterval =
                (newOffset == 0 || newOffset == text.Count - availableDigits) ?
                endsDelay : delay;
            newTimer = timer.SetInterval(newInterval);

            return new ScrollingGlyphSelector(delay, endsDelay, availableDigits,
                text, newOffset, newTimer);
        }

        public IGlyphSelector SetText(ImmutableList<Glyph> text)
        {
            return new ScrollingGlyphSelector(delay, endsDelay, availableDigits,
                text, 0);
        }

        public ImmutableList<Glyph> GetSelected()
        {
            if (text.Count <= offset)
            {
                return ImmutableList<Glyph>.Empty;
            }

            return text.GetRange(offset, Math.Min(availableDigits, text.Count - offset));
        }

        private readonly uint delay;
        private readonly uint endsDelay;
        private readonly ImmutableList<Glyph> text;
        private readonly ImmutableTimer timer;
        private readonly int offset;
        private readonly int availableDigits;
    }
}
