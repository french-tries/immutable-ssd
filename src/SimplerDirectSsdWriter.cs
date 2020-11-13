using System;
using System.Collections.Immutable;
using System.Diagnostics;
using blink.src;

namespace immutableSsd.src
{
    public class SimplerDirectSsdWriter : ISsdWriter<Func<int, byte>>
    {
        public SimplerDirectSsdWriter(ImmutableList<Pin> segmentPins, ImmutableList<Pin> digitPins,
            GpioHandler handler)
        {
            this.segmentPins = segmentPins;
            this.digitPins = digitPins;
            this.handler = handler;
            this.values = (arg) => 0;
            this.currentDigit = digitPins.Count - 1;

            Debug.Assert(this.segmentPins.Count == 8);
        }

        private SimplerDirectSsdWriter(ImmutableList<Pin> segmentPins, ImmutableList<Pin> digitPins,
            GpioHandler handler, Func<int, byte> values, int? currentDigit = null)
        {
            this.segmentPins = segmentPins;
            this.digitPins = digitPins;
            this.handler = handler;
            this.values = values;
            this.currentDigit = currentDigit ?? digitPins.Count - 1;
        }

        public void Clear()
        {
            for (int i = 0; i < digitPins.Count; ++i)
            {
                handler.Write(digitPins[i], false);
            }
            for (int i = 0; i < segmentPins.Count; ++i)
            {
                handler.Write(segmentPins[i], false);
            }
        }

        public void Write(byte segments, int digit)
        {
            if (digit < 0 || digit >= digitPins.Count)
            {
                Clear();
                return;
            }
            handler.Write(digitPins[digit > 0 ? digit - 1 : digitPins.Count - 1], false);

            for (int i = 0; i < segmentPins.Count; ++i)
            {
                handler.Write(segmentPins[i], (segments & (1 << (7 - i))) != 0);
            }
            handler.Write(digitPins[digit], true);
        }

        public ISsdWriter<Func<int, byte>> Write(Func<int, byte> values)
        {
            return new SimplerDirectSsdWriter(segmentPins, digitPins, handler,
                values);
        }

        public ISsdWriter<Func<int, byte>> Tick(uint currentTime)
        {
            currentDigit = currentDigit + 1;
            if (currentDigit >= AvailableDigits) currentDigit = 0;

            Write(values(currentDigit), currentDigit);

            return this;
        }

        public uint Remaining(uint currentTime) => 0;

        public int AvailableDigits => digitPins.Count;

        private readonly GpioHandler handler;
        private readonly ImmutableList<Pin> segmentPins;
        private readonly ImmutableList<Pin> digitPins;

        private readonly Func<int, byte> values;
        private int currentDigit;
    }
}
