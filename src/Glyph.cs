using System;
namespace deskpi.src
{
    public class Glyph
    {
        public Glyph(char character, bool dot)
        {
            Character = character;
            Dot = dot;
        }

        public char Character { get; }

        public bool Dot { get; }
    }

    public interface IGlyphs
    {
        Glyph GetGlyph(uint index, uint displayLength);

        uint Count { get; }
    }
}
