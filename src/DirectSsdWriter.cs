using System;
using System.Collections.Immutable;
using System.Diagnostics;
using immutableSsd.src;

namespace blink.src
{
    public class DirectSsdWriter : ISsdWriter<Func<int, byte>>
    {
        public DirectSsdWriter(ImmutableList<Pin> segmentPins, ImmutableList<Pin> digitPins,
            StepApplier applier, uint interval)
        {
            this.segmentPins = segmentPins;
            this.digitPins = digitPins;
            this.applier = applier;
            this.timer = new ImmutableTimer(interval);
            this.values = (arg) => 0;
            this.currentDigit = digitPins.Count - 1;

            Debug.Assert(this.segmentPins.Count == 8);
        }

        private DirectSsdWriter(ImmutableList<Pin> segmentPins, ImmutableList<Pin> digitPins,
            StepApplier applier, Func<int, byte> values, ImmutableTimer timer,
            int? currentDigit = null)
        {
            this.segmentPins = segmentPins;
            this.digitPins = digitPins;
            this.applier = applier;
            this.values = values;
            this.timer = timer;
            this.currentDigit = currentDigit ?? digitPins.Count - 1;
        }

        public void Clear()
        {
            for (int i = 0; i < digitPins.Count; ++i)
            {
                applier(new WriteStep(digitPins[i], false));
            }
            for (int i = 0; i < segmentPins.Count; ++i)
            {
                applier(new WriteStep(segmentPins[i], false));
            }
        }

        public void Write(byte segments, int digit)
        {
            if (digit < 0 || digit >= digitPins.Count)
            {
                Clear();
                return;
            }
            applier(new WriteStep(digitPins[digit > 0 ? digit - 1 : digitPins.Count - 1], false));

            for (int i = 0; i < segmentPins.Count; ++i)
            {
                applier(new WriteStep(segmentPins[i], (segments & (1 << (7 - i))) != 0));
            }
            applier(new WriteStep(digitPins[digit], true));
        }

        public ISsdWriter<Func<int, byte>> Write(Func<int, byte> values)
        {
            return new DirectSsdWriter(segmentPins, digitPins, applier,
                values, new ImmutableTimer(timer.Interval));
        }

        public ISsdWriter<Func<int, byte>> Tick(uint currentTime)
        {
            var newTimer = timer.Tick(currentTime);
            if (timer == newTimer)
            {
                return this;
            }
            int nextDigit = currentDigit + 1;
            if (nextDigit >= AvailableDigits) nextDigit = 0;

            Write(values(nextDigit), nextDigit);

            return new DirectSsdWriter(segmentPins, digitPins, applier, values,
                newTimer, nextDigit);
        }

        public uint Remaining(uint currentTime) => timer.Remaining(currentTime);

        public int AvailableDigits => digitPins.Count;

        private readonly StepApplier applier;
        private readonly ImmutableList<Pin> segmentPins;
        private readonly ImmutableList<Pin> digitPins;

        private readonly Func<int, byte> values;
        private readonly ImmutableTimer timer;
        private readonly int currentDigit;
    }
}
