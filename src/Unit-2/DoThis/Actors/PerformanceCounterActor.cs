using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartApp.Actors
{
    public class PerformanceCounterActor : ReceiveActor
    {
        private readonly string _seriesName;
        private Lazy<PerformanceCounter> _counter;

        private readonly HashSet<IActorRef> _subscriptions;
        private readonly ICancelable _cancelPublishing;

        public PerformanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator)
        {
            _seriesName = seriesName;
            _counter = new Lazy<PerformanceCounter>(performanceCounterGenerator);
            _subscriptions = new HashSet<IActorRef> { };
            _cancelPublishing = new Cancelable(Context.System.Scheduler);

            Receive<GatherMetrics>(gm => NotifySubscribers(gm));
            Receive<SubscribeCounter>(sc => _subscriptions.Add(sc.Subscriber));
            Receive<UnsubscribeCounter>(uc => _subscriptions.Remove(uc.Subscriber));
        }

        private void NotifySubscribers(GatherMetrics gm)
        {
            var metric = new Metric(_seriesName, _counter.Value.NextValue());
            foreach (var sub in _subscriptions)
            {
                sub.Tell(metric);
            }
        }

        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromMilliseconds(250),
                TimeSpan.FromMilliseconds(250),
                Self,
                new GatherMetrics(),
                Self,
                _cancelPublishing);
        }

        protected override void PostStop()
        {
            try
            {
                _cancelPublishing.Cancel(false);
                _counter.Value.Dispose();
            }
            catch
            {

            }
            finally
            {
                base.PostStop();
            }
        }
    }
}
