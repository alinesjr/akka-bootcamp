using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartApp.Actors
{
    public class GatherMetrics { }

    public class Metric
    {
        public string Series { get; private set; }

        public float CounterValue { get; private set; }

        public Metric(string series, float counterValue)
        {
            CounterValue = counterValue;
            Series = series;
        }
    }

    public enum CounterType
    {
        CPU,
        Memory,
        Disk,
    }

    public class SubscribeCounter
    {
        public CounterType Counter { get; private set; }
        public IActorRef Subscriber { get; private set; }

        public SubscribeCounter(CounterType counter, IActorRef subscriber)
        {
            Subscriber = subscriber;
            Counter = counter;
        }
    }

    public class UnsubscribeCounter
    {
        public CounterType Counter { get; private set; }
        public IActorRef Subscriber { get; private set; }

        public UnsubscribeCounter(CounterType counter, IActorRef subscriber)
        {
            Subscriber = subscriber;
            Counter = counter;
        }
    }
}
