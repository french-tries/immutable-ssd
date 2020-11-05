using System;
using System.Collections.Immutable;
using System.Diagnostics;
using deskpi.src;

namespace blink.src
{
    public class DirectSsdWriter : ISsdWriter<Func<int, byte>>
    {
        private class Tickable : ITickable
        {
            public Tickable(DirectSsdWriter writer, Func<int, byte> values,
                int? lastUpdate = null, int currentDigit = 0)
            {
                this.writer = writer;
                this.values = values;
                this.lastUpdate = lastUpdate;
                this.currentDigit = currentDigit;
            }

            public ITickable Tick(int currentTime)
            {
                if (lastUpdate == null)
                {
                    writer.Write(values(currentDigit), currentDigit);
                    return new Tickable(writer, values, currentTime, currentDigit);
                }
                if (currentTime - lastUpdate < writer.interval)
                {
                    return this;
                }
                int nextDigit = currentDigit + 1;
                if (nextDigit >= writer.AvailableDigits) nextDigit = 0;

                writer.Write(values(nextDigit), nextDigit);

                return new Tickable(writer, values,
                    lastUpdate + writer.interval, nextDigit);
            }

            private readonly DirectSsdWriter writer;
            private readonly Func<int, byte> values;
            private readonly int? lastUpdate;
            private readonly int currentDigit;
        }

        public DirectSsdWriter(ImmutableList<Pin> segmentPins, ImmutableList<Pin> digitPins,
            IGPIO gPIO, int interval)
        {
            this.segmentPins = segmentPins;
            this.digitPins = digitPins;
            this.gPIO = gPIO;
            this.interval = interval;

            Debug.Assert(this.segmentPins.Count == 8);
        }

        public void Clear()
        {
            for (int i = 0; i < digitPins.Count; ++i)
            {
                gPIO.Write(digitPins[i], false);
            }
            for (int i = 0; i < segmentPins.Count; ++i)
            {
                gPIO.Write(segmentPins[i], false);
            }
        }

        public void Write(byte segments, int digit)
        {
            if (digit < 0 || digit >= digitPins.Count)
            {
                Clear();
                return;
            }
            gPIO.Write(digitPins[digit > 0 ? digit - 1 : digitPins.Count - 1], false);

            for (int i = 0; i < segmentPins.Count; ++i)
            {
                gPIO.Write(segmentPins[i], (segments & (1 << (7 - i))) != 0);
            }
            gPIO.Write(digitPins[digit], true);
        }

        public ITickable Write(Func<int, byte> values)
            => new Tickable(this, values);

        public int AvailableDigits => digitPins.Count;

        private readonly IGPIO gPIO;
        private readonly ImmutableList<Pin> segmentPins;
        private readonly ImmutableList<Pin> digitPins;

        private readonly int interval;
    }
}
