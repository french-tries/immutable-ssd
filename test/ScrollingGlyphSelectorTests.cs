using System;
using System.Collections.Immutable;
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

            Assert.AreEqual(Glyph.Empty, selector.GetGlyph(ImmutableList<Glyph>.Empty, 1, 2, 0));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GetGlyph_AtStart_ReturnsWithoutOffset(int pos)
        {
            var text = ImmutableList<Glyph>.Empty.Add(new Glyph('a', true))
                .Add(new Glyph('b', false)).Add(new Glyph('c', true));

            int delay = 1;
            int endsDelay = 2;

            var selector = new ScrollingGlyphSelector(delay, endsDelay, 0);

            Assert.AreEqual(text[pos], selector.GetGlyph(text, pos, 3, 0));
        }

        [TestCase]
        public void GetGlyph_WithDelay_ReturnsWithOffset()
        {
            var text = ImmutableList<Glyph>.Empty.Add(new Glyph('a', true))
                .Add(new Glyph('b', false)).Add(new Glyph('c', true));

            int delay = 1;
            int endsDelay = 2;

            var selector = new ScrollingGlyphSelector(delay, endsDelay, 0);

            Assert.AreEqual(text[1], selector.GetGlyph(text, 0, 1, 2));
            Assert.AreEqual(text[2], selector.GetGlyph(text, 0, 1, 3));
            Assert.AreEqual(text[0], selector.GetGlyph(text, 0, 1, 5));
        }
    }
}
