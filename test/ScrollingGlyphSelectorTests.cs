using System;
using System.Collections.Immutable;
using System.Linq;
using deskpi.src;
using NUnit.Framework;

namespace deskpi.test
{
    [TestFixture]
    public class ScrollingGlyphSelectorTests
    {
        [TestCase]
        public void GetGlyph_LongerIndexThanSource_ReturnsEmptyGlyph()
        {
            var selector = new ScrollingGlyphSelector(1,2,0);

            Assert.AreEqual(ImmutableList<Glyph>.Empty, selector.GetSelected(ImmutableList<Glyph>.Empty, 2, 0));
        }

        [TestCase]
        public void GetGlyph_AtStart_ReturnsWithoutOffset(int pos)
        {
            var text = ImmutableList<Glyph>.Empty.Add(new Glyph('a', true))
                .Add(new Glyph('b', false)).Add(new Glyph('c', true));

            int delay = 1;
            int endsDelay = 2;

            var selector = new ScrollingGlyphSelector(delay, endsDelay, 0);

            Assert.AreEqual(text, selector.GetSelected(text, 3, 0));
        }

        [TestCase]
        public void GetGlyph_PartialDisplay_ReturnsSubList(int pos)
        {
            var text = ImmutableList<Glyph>.Empty.Add(new Glyph('a', true))
                .Add(new Glyph('b', false)).Add(new Glyph('c', true));

            int delay = 1;
            int endsDelay = 2;

            var selector = new ScrollingGlyphSelector(delay, endsDelay, 0);

            Assert.True(text.GetRange(0,2).SequenceEqual(selector.GetSelected(text, 2, 0)));
        }

        [TestCase]
        public void GetGlyph_TextTooShort_ReturnsSubList(int pos)
        {
            var text = ImmutableList<Glyph>.Empty.Add(new Glyph('a', true))
                .Add(new Glyph('b', false)).Add(new Glyph('c', true));

            int delay = 1;
            int endsDelay = 2;

            var selector = new ScrollingGlyphSelector(delay, endsDelay, 0);

            Assert.True(text.SequenceEqual(selector.GetSelected(text, 4, 0)));
        }


        [TestCase]
        public void GetGlyph_WithDelay_ReturnsWithOffset()
        {
            var text = ImmutableList<Glyph>.Empty.Add(new Glyph('a', true))
                .Add(new Glyph('b', false)).Add(new Glyph('c', true));

            int delay = 1;
            int endsDelay = 2;

            var selector = new ScrollingGlyphSelector(delay, endsDelay, 0);

            Assert.True(text.GetRange(1, 1).SequenceEqual(selector.GetSelected(text, 1, 2)));
            Assert.True(text.GetRange(2, 1).SequenceEqual(selector.GetSelected(text, 1, 3)));
            Assert.True(text.GetRange(0, 1).SequenceEqual(selector.GetSelected(text, 1, 5)));
        }
    }
}
