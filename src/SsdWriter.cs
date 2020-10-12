using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace blink.src
{
    public class SsdWriter
    {
        public delegate void WriteAction(int pin, bool value);

        public SsdWriter(WriteAction write, ImmutableList<int> segmentPins,
            ImmutableList<int> digitPins)
        {
            this.Write = write;
            this.segmentPins = segmentPins;
            this.digitPins = digitPins;

            Debug.Assert(this.segmentPins.Count == 8);
        }

        public void WriteValue(byte segments, int digit)
        {
            if (digit < 0 || digit >= digitPins.Count)
            {
                Clear();
            }
            else
            {
                Write(digitPins[digit >= 0 ? digit - 1 : digitPins.Count - 1], false);

                for (int i = 0; i < segmentPins.Count; ++i)
                {
                    Write(segmentPins[i], (segments & (1 << (7-i))) != 0);
                }

                Write(digitPins[digit], true);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < digitPins.Count; ++i)
            {
                Write(digitPins[i], false);
            }
            for (int i = 0; i < segmentPins.Count; ++i)
            {
                Write(segmentPins[i], false);
            }
        }

        public int AvailableDigits {
            get
            {
                return digitPins.Count;
            }
        }

        private readonly WriteAction Write;

        private readonly ImmutableList<int> segmentPins;
        private readonly ImmutableList<int> digitPins;
    }
}
