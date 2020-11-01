using System;
using System.Collections.Immutable;
using System.Diagnostics;
using deskpi.src;

namespace blink.src
{
    public class SsdWriter
    {
        public delegate byte SegmentsValues(int index);

        public SsdWriter(ImmutableList<uint> segmentPins, ImmutableList<uint> digitPins)
        {
            this.segmentPins = segmentPins;
            this.digitPins = digitPins;

            Debug.Assert(this.segmentPins.Count == 8);
        }

        public ImmutableList<Step> ClearSteps(ImmutableList<Step> steps)
        {
            for (int i = 0; i < digitPins.Count; ++i)
            {
                steps = steps.Add(new WriteStep(digitPins[i], false));
            }
            for (int i = 0; i < segmentPins.Count; ++i)
            {
                steps = steps.Add(new WriteStep(segmentPins[i], false));
            }
            return steps;
        }

        public ImmutableList<Step> WriteSteps(byte segments, int digit,
            ImmutableList<Step> steps)
        {
            if (digit < 0 || digit >= digitPins.Count)
            {
                return ClearSteps(steps);
            }

            steps = steps.Add(new WriteStep(
                digitPins[digit > 0 ? digit - 1 : digitPins.Count - 1], false));

            for (int i = 0; i < segmentPins.Count; ++i)
            {
                steps = steps.Add(new WriteStep(
                    segmentPins[i], (segments & (1 << (7 - i))) != 0));
            }

            steps = steps.Add(new WriteStep(digitPins[digit], true));
            return steps;
        }

        public ImmutableList<Step> CycleSteps(SegmentsValues values, uint interval,
            ImmutableList<Step> steps)
        {
            var sleepStep = new SleepStep(interval);

            for (int i = 0; i < AvailableDigits; ++i)
            {
                steps = WriteSteps(values(i), i, steps);
                steps = steps.Add(sleepStep);
            }
            return steps;
        }

        public int AvailableDigits {
            get
            {
                return digitPins.Count;
            }
        }

        private readonly ImmutableList<uint> segmentPins;
        private readonly ImmutableList<uint> digitPins;
    }
}
