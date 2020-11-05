using System;
namespace deskpi.src
{
    public class Step
    {
    }

    public class WriteStep : Step
    {
        public WriteStep(int pin, bool value)
        {
            Pin = pin;
            Value = value;
        }

        public int Pin { get; }
        public bool Value { get; }
    }

    public class SleepStep : Step
    {
        public SleepStep(int length)
        {
            Length = length;
        }

        public int Length { get; }
    }

    public delegate void StepApplier(Step step);
}
