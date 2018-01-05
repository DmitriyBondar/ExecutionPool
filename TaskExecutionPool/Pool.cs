using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TaskExecutionPool
{
    /// <summary>
    /// Execution pool for Delegates. 
    /// </summary>
    public sealed class Pool
    {
        private readonly ConcurrentQueue<Action> _delegateQueue;

        private readonly object _startExecutionLock = new object();

        /// <summary>
        /// Execution pool state
        /// </summary>
        public bool Idle { get; private set; }

        /// <summary>
        /// Count of successfully processed tasks
        /// </summary>
        public int ProcessedTasksCount { get; private set; }

        /// <summary>
        /// New Instance of Task Execution Pool
        /// </summary>
        public Pool()
        {
            _delegateQueue = new ConcurrentQueue<Action>();
            Idle = true;
        }

        /// <summary>
        /// Executes delegate asynchronously in sequential order
        /// </summary>
        public void AddForExecution(Action task)
        {
            if (task == null)
                return;

            _delegateQueue.Enqueue(task);

            lock (_startExecutionLock)
            {
                if (Idle)
                    StartExecution();
            }
        }

        /// <summary>
        /// Starts execution of tasks in queue
        /// </summary>
        private async void StartExecution()
        {
            Console.WriteLine($"Execution started: {DateTime.Now}");

            Action headTask;
            if (!_delegateQueue.TryDequeue(out headTask))
                return;

            Idle = false;
            await ProcessDelegateAsync(headTask);
        }

        /// <summary>
        /// Recursively executes delegates from queue
        /// </summary>
        private async Task ProcessDelegateAsync(Action task)
        {
            if (task != null)
            {
                try
                {
                    await Task.Run(task)
                                .ContinueWith(t => ProcessedTasksCount++, TaskContinuationOptions.OnlyOnRanToCompletion);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
                
            Action nextTask;
            if (_delegateQueue.TryDequeue(out nextTask))
            {
                await ProcessDelegateAsync(nextTask);
            }
            else
            {
                Idle = true;
                Console.WriteLine($"Idle");
            }
        }

        /// <summary>
        /// Count of Execution Queue
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return _delegateQueue?.Count ?? 0;
        }
    }
}
