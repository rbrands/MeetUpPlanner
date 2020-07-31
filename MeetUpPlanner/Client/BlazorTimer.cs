using System;
using System.Timers;

namespace MeetUpPlanner.Client
{
    public class BlazorTimer
    {
        private Timer _timer;

        public void SetTimer(double interval)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += NotifyTimerElapsed;
            _timer.Enabled = true;
        }
        public void DisableTimer()
        {
            if ( null != _timer )
            { 
                _timer.Enabled = false;
            }
        }

        public event Action OnElapsed;

        private void NotifyTimerElapsed(Object source, ElapsedEventArgs e)
        {
            OnElapsed?.Invoke();
            _timer.Dispose();
        }
    }
}
