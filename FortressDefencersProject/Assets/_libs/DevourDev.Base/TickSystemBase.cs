using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevourDev.Base
{
    public abstract class TickSystemBase
    {
        private DateTime? _tickingStartedDate;
        private DateTime? _tickingCancelledDate;
        private int _ticksPerSecond;

        private Task _tickingTask;
        private CancellationTokenSource _tickingCancellation;
        private TimeSpan _crossTicksSpan;

        private long _lateTicks;

        public TickSystemBase(int ticksPerSecond)
        {
            _tickingStartedDate = null;
            _tickingCancelledDate = null;
            SetNewTickrate(ticksPerSecond);
            _tickingTask = null;
            _tickingCancellation = null;
        }

        public int TPS => _ticksPerSecond;
        public DateTime? TickingStartedDate => _tickingStartedDate;
        public DateTime? TickingCancelledDate => _tickingCancelledDate;

        protected bool IsLateForTicks => _lateTicks > 0;

        public void StartTicking()
        {
            if (_tickingTask != null)
            {
                if (_tickingTask.Status == TaskStatus.Running)
                {
                    throw new InvalidOperationException("Already ticking!");
                }
            }
            OnStartTicking();
            _tickingStartedDate = DateTime.Now;
            _lateTicks = 0;
            _tickingCancellation = new();
            _tickingTask = Task.Factory.StartNew(Ticking, _tickingCancellation.Token,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected virtual void OnStartTicking()
        {
        }

        public void CancellTicking()
        {
            if (_tickingTask == null)
                throw new InvalidOperationException("Not ticking yet!");

            _tickingCancelledDate = DateTime.Now;
            _tickingCancellation.Cancel();
        }
        public void SetNewTickrate(int newTickrate)
        {
            _ticksPerSecond = newTickrate;
            _crossTicksSpan = new TimeSpan(TimeSpan.TicksPerSecond / _ticksPerSecond);
        }



        private async Task Ticking()
        {
            DateTime nextTickDate = DateTime.Now;
            while (true)
            {
                if (TryGetSpareTime(out var waitTime))
                {
#if DEBUG
                    if (_lateTicks < 0)
                        throw new Exception($"Not expected _lateForTicks value: {_lateTicks}");
#endif
                    await Task.Delay(waitTime.Value).ConfigureAwait(false);

                }

                HandleTick();

                nextTickDate = nextTickDate.Add(_crossTicksSpan);
            }

            bool TryGetSpareTime(out TimeSpan? waitTime)
            {
                long spareTicks = nextTickDate.Subtract(DateTime.Now).Ticks;

                if (spareTicks > 0)
                {
                    if (IsLateForTicks)
                    {
                        if (_lateTicks < spareTicks)
                        {
                            spareTicks -= _lateTicks;
                            _lateTicks = 0;
                            goto SpareTime;
                        }
                        else
                        {
                            _lateTicks -= spareTicks;
                            goto NoSpareTime;
                        }
                    }

                    goto SpareTime;
                }

                _lateTicks -= spareTicks;
                goto NoSpareTime;


            SpareTime:
                waitTime = new TimeSpan(spareTicks);
                return true;

            NoSpareTime:
                waitTime = null;
                return false;
            }

        }

        protected abstract void HandleTick();
    }
}
