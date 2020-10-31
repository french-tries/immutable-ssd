using System;
using System.Collections.Immutable;
using System.Diagnostics;
using deskpi.src;

namespace blink.src
{
    public class SsdWriter
    {
        public delegate byte SegmentsValues(int index);

        public SsdWriter(StepApplier applyStep, 
            ImmutableList<uint> segmentPins, ImmutableList<uint> digitPins)
        {
            this.applyStep = applyStep;
            this.segmentPins = segmentPins;
            this.digitPins = digitPins;

            Debug.Assert(this.segmentPins.Count == 8);
        }

        public void ClearSteps()
        {
            for (int i = 0; i < digitPins.Count; ++i)
            {
                applyStep(new WriteStep(digitPins[i], false));
            }
            for (int i = 0; i < segmentPins.Count; ++i)
            {
                applyStep(new WriteStep(segmentPins[i], false));
            }
        }

        public void WriteSteps(byte segments, int digit)
        {
            if (digit < 0 || digit >= digitPins.Count)
            {
                ClearSteps();
                return;
            }

            applyStep(new WriteStep(
                digitPins[digit > 0 ? digit - 1 : digitPins.Count - 1], false));

            for (int i = 0; i < segmentPins.Count; ++i)
            {
                applyStep(new WriteStep(
                    segmentPins[i], (segments & (1 << (7 - i))) != 0));
            }

            applyStep(new WriteStep(digitPins[digit], true));
        }

        public void CycleSteps(SegmentsValues values, uint interval)
        {
            var sleepStep = new SleepStep(interval);

            for (int i = 0; i < AvailableDigits; ++i)
            {
                WriteSteps(values(i), i);
                applyStep(sleepStep);
            }
        }

        public int AvailableDigits {
            get
            {
                return digitPins.Count;
            }
        }

        private readonly StepApplier applyStep;
        private readonly ImmutableList<uint> segmentPins;
        private readonly ImmutableList<uint> digitPins;
    }
}
