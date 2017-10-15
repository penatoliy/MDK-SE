using System;
using System.Threading;

namespace MDK.Modularity
{
    /// <summary>
    /// A helper class to keep track of process.
    /// </summary>
    public class Progress
    {
        int _step;

        /// <summary>
        /// Creates an instance of <see cref="Progress"/>
        /// </summary>
        /// <param name="stepCount">The number of steps in this operation (=> 1)</param>
        /// <param name="reporter">An optional progress reporter</param>
        /// <param name="synchronizationContext">An optional synchronization context through which the reporter will be called</param>
        public Progress(int stepCount, IProgress<float> reporter = null, SynchronizationContext synchronizationContext = null)
        {
            if (stepCount <= 1)
                throw new ArgumentOutOfRangeException(nameof(stepCount));
            StepCount = stepCount;
            Reporter = reporter;
            SynchronizationContext = synchronizationContext;
        }

        /// <summary>
        /// The number of steps in this operation
        /// </summary>
        public int StepCount { get; }

        /// <summary>
        /// An optional external reporter
        /// </summary>
        public IProgress<float> Reporter { get; set; }

        /// <summary>
        /// An optional synchronization context through which the <see cref="Reporter"/> will be called
        /// </summary>
        public SynchronizationContext SynchronizationContext { get; set; }

        /// <summary>
        /// The current step, a value between 0 and <see cref="StepCount"/>
        /// </summary>
        public int Step
        {
            get => _step;
            set
            {
                value = Math.Max(0, Math.Min(StepCount, value));
                if (_step == value)
                    return;
                _step = value;
                OnStepChanged();
            }
        }

        /// <summary>
        /// Called when <see cref="Step"/> changes
        /// </summary>
        protected virtual void OnStepChanged()
        {
            var reporter = Reporter;
            if (reporter == null)
                return;
            var context = SynchronizationContext;
            if (context != null)
                context.Post(state => reporter.Report((float)state), (float)StepCount / _step);
            else
                reporter.Report((float)StepCount / _step);
        }

        /// <summary>
        /// Advances a number of steps forward.
        /// </summary>
        /// <param name="steps">The number of steps to advance. Defaults to 1 and cannot be negative</param>
        public void Advance(int steps = 1)
        {
            if (steps <= 0)
                throw new ArgumentOutOfRangeException(nameof(steps));
            Step += steps;
        }
    }
}
