using System;
namespace deskpi.src
{
    public class Step
    {
    }

    public class WriteStep : Step
    {
        public WriteStep(uint pin, bool value)
        {
            Pin = pin;
            Value = value;
        }

        public uint Pin { get; }
        public bool Value { get; }
    }

    public class SleepStep : Step
    {
        public SleepStep(uint length)
        {
            Length = length;
        }

        public uint Length { get; }
    }

    public delegate void StepApplier(Step step);
}
