using System;
using System.Timers;

namespace blink.src
{
    public class Ssd
    {
        public int NextDigit(int digit) =>
            digit + 1 < Writer.AvailableDigits ? digit + 1 : 0;

        public Ssd(SsdWriter writer, SegmentsConverter converter, double interval,
            string text = "")
        {
            Writer = writer;
            Converter = converter;
            Text = text;

            MultiTimer = new Timer
            {
                Interval = interval,
                AutoReset = false,

            };
            MultiTimer.Elapsed += MultiplexDigitEventFunc(this, 0);
            MultiTimer.Enabled = true;
        }

        public Ssd SetText(string text)
        {
            MultiTimer.Dispose();
            return new Ssd(Writer, Converter, MultiTimer.Interval, text);
        }

        private void Write(int digit)
        {
            byte segments = Converter.GetSegments(digit < Text.Length ? Text[digit] : ' ');
            //Writer.WriteValue(segments, digit);
        }

        private static Func<Ssd, int, ElapsedEventHandler> MultiplexDigitEventFunc = 
        (Ssd ssd, int digit) => (Object source, ElapsedEventArgs e) =>
        {
            ssd.Write(digit);
            ssd.MultiTimer.Elapsed += MultiplexDigitEventFunc(ssd, ssd.NextDigit(digit));
        };

        private readonly SsdWriter Writer;
        private readonly SegmentsConverter Converter;
        private readonly string Text;

        private Timer MultiTimer;
    }
}
