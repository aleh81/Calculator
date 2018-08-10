using System;
using System.Threading;

namespace Test
{
    class EventRaiser
    {
        int _counter;

        public event EventHandler<EventRaiserCounterChangedEventArgs> CounterChanged;

        public int Counter
        {
            get
            {
                return _counter;
            }

            set
            {
                if (_counter != value)
                {
                    var old = _counter;
                    _counter = value;
                    OnCounterChanged(old, value);
                }
            }
        }

        public void DoWork()
        {
            new Thread(new ThreadStart(() =>
            {
                for (var i = 0; i < 100; i++)
                    Counter = i;
            })).Start();
        }

        void OnCounterChanged(int oldValue, int newValue)
        {
            if (CounterChanged != null)
                CounterChanged.Invoke(this, new EventRaiserCounterChangedEventArgs(oldValue, newValue));
        }
    }

    class EventRaiserCounterChangedEventArgs : EventArgs
    {
        public int NewValue { get; set; }
        public int OldValue { get; set; }
        public EventRaiserCounterChangedEventArgs(int oldValue, int newValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var raiser = new EventRaiser();
            raiser.CounterChanged += Raiser_CounterChanged;
            raiser.DoWork();
            Console.ReadLine();
        }

        static void Raiser_CounterChanged(object sender, EventRaiserCounterChangedEventArgs e)
        {
            Console.WriteLine(string.Format("OldValue: {0}; NewValue: {1}", e.OldValue, e.NewValue));
        }
    }
}
