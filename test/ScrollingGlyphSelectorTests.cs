using System;
using System.Collections.Immutable;
using System.Linq;
using immutableSsd.src;
using NUnit.Framework;

namespace immutableSsd.test
{
    [TestFixture]
    public class ScrollingGlyphSelectorTests
    {
        [TestCase]
        public void GetGlyph_Empty_ReturnsEmptyGlyphs()
        {
            var selector = new ScrollingGlyphSelector(1,2,1);

            Assert.AreEqual(ImmutableList<Glyph>.Empty, selector.GetSelected());
        }

        [TestCase]
        public void GetGlyph_AtStart_ReturnsWithoutOffset(int pos)
        {
            var text = ImmutableList<Glyph>.Empty.Add(new Glyph('a', true))
                .Add(new Glyph('b', false)).Add(new Glyph('c', true));

            uint delay = 1;
            uint endsDelay = 2;

            var selector = new ScrollingGlyphSelector(delay, endsDelay, 3).SetText(text);

            Assert.AreEqual(text, selector.GetSelected());
        }

        [TestCase]
        public void GetGlyph_PartialDisplay_ReturnsSubList(int pos)
        {
            var text = ImmutableList<Glyph>.Empty.Add(new Glyph('a', true))
                .Add(new Glyph('b', false)).Add(new Glyph('c', true));

            uint delay = 1;
            uint endsDelay = 2;

            var selector = new ScrollingGlyphSelector(delay, endsDelay, 2).SetText(text);

            Assert.True(text.GetRange(0,2).SequenceEqual(selector.GetSelected()));
        }

        [TestCase]
        public void GetGlyph_TextTooShort_ReturnsSubList(int pos)
        {
            var text = ImmutableList<Glyph>.Empty.Add(new Glyph('a', true))
                .Add(new Glyph('b', false)).Add(new Glyph('c', true));

            uint delay = 1;
            uint endsDelay = 2;

            var selector = new ScrollingGlyphSelector(delay, endsDelay, 3);

            Assert.True(text.SequenceEqual(selector.GetSelected()));
        }


        [TestCase]
        public void GetGlyph_WithDelay_ReturnsWithOffset()
        {
            var text = ImmutableList<Glyph>.Empty.Add(new Glyph('a', true))
                .Add(new Glyph('b', false)).Add(new Glyph('c', true));

            uint delay = 1;
            uint endsDelay = 2;

            var selector = new ScrollingGlyphSelector(delay, endsDelay, 1).SetText(text);

            selector = selector.Tick(2);
            Assert.True(text.GetRange(1, 1).SequenceEqual(selector.GetSelected()));

            selector = selector.Tick(3);
            Assert.True(text.GetRange(2, 1).SequenceEqual(selector.GetSelected()));

            selector = selector.Tick(5);
            Assert.True(text.GetRange(0, 1).SequenceEqual(selector.GetSelected()));
        }
    }
}
