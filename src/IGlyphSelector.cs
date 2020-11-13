using System.Collections.Immutable;

namespace immutableSsd.src
{
    public interface IGlyphSelector : ITickable<IGlyphSelector>
    {
        ImmutableList<Glyph> GetSelected();
        IGlyphSelector SetText(ImmutableList<Glyph> text);
    }
}