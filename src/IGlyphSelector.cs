using System.Collections.Immutable;

namespace deskpi.src
{
    public interface IGlyphSelector : ITickable<IGlyphSelector>
    {
        ImmutableList<Glyph> GetSelected();
        IGlyphSelector SetText(ImmutableList<Glyph> text);
    }
}