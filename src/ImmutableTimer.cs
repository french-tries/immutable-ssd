namespace deskpi.src
{
    public class ImmutableTimer : ITickable<ImmutableTimer>
    {
        public ImmutableTimer(uint interval, uint? lastUpdate = null)
        {
            Interval = interval;
            this.lastUpdate = lastUpdate;
        }

        public ImmutableTimer Tick(uint currentTime)
        {
            if (!lastUpdate.HasValue)
            {
                return new ImmutableTimer(Interval, lastUpdate);
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

        public uint Interval { get; }
        private uint? lastUpdate;
    }
}
