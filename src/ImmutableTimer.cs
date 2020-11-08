using System.Diagnostics;

namespace deskpi.src
{
    public class ImmutableTimer : ITickable<ImmutableTimer>
    {
        public ImmutableTimer(uint interval, uint? lastUpdate = null)
        {
            Interval = interval;
            this.lastUpdate = lastUpdate;

            Debug.Assert(interval < uint.MaxValue / 4);
        }

        public ImmutableTimer Tick(uint currentTime)
        {
            if (!lastUpdate.HasValue)
            {
                return new ImmutableTimer(Interval, currentTime);
            }
            if (currentTime - lastUpdate >= Interval)
            {
                return new ImmutableTimer(Interval, lastUpdate + Interval);
            }
            return this;
        }

        public ImmutableTimer SetInterval(uint interval)
        {
            return new ImmutableTimer(interval, lastUpdate);
        }

        public uint Remaining(uint currentTime)
        {
            if (!lastUpdate.HasValue)
            {
                return 0;
            }
            var lastValue = lastUpdate.Value;
            var next = lastValue + Interval;

            if (next - currentTime > currentTime - next)
            {
                return 0;
            }
            return next - currentTime;
        }

        public uint Interval { get; }
        private uint? lastUpdate;
    }
}
