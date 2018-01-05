using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using TaskExecutionPool;

namespace UnitTestProject1
{
    [TestFixture]
    public class PoolTests
    {
        private Pool _executionPool;

        [SetUp]
        public void SetUp()
        {
            _executionPool = new Pool();
        }

        [Test]
        public void AddForExecution_SequentialTaskAdding()
        {
            var tasks = GetTasks().ToList();
            foreach (var task in tasks)
            {
                _executionPool.AddForExecution(task);
            }

            Thread.Sleep(35000);

            Assert.AreEqual(_executionPool.ProcessedTasksCount, tasks.Count);
        }

        [Test]
        public void AddForExecution_ConcurrentTaskAdding()
        {
            var tasks = GetTasks().ToList();

            Parallel.Invoke(() => _executionPool.AddForExecution(tasks[0]),
                () => _executionPool.AddForExecution(tasks[1]), () => _executionPool.AddForExecution(tasks[2]),
                () => _executionPool.AddForExecution(tasks[3]), () => _executionPool.AddForExecution(tasks[4]));

            Thread.Sleep(35000);

            Assert.AreEqual(_executionPool.ProcessedTasksCount, tasks.Count);
        }

        [Test]
        public void AddForExecution_TaskWithException_ProcessedTaskCountEquals_3()
        {
            var tasks = GetTasksWithException().ToList();

            Parallel.Invoke(() => _executionPool.AddForExecution(tasks[0]),
                () => _executionPool.AddForExecution(tasks[1]), () => _executionPool.AddForExecution(tasks[2]),
                () => _executionPool.AddForExecution(tasks[3]));

            Thread.Sleep(11000);

            Assert.AreEqual(_executionPool.ProcessedTasksCount, 3);
        }



        private IEnumerable<Action> GetTasks()
        {
            return new List<Action>
            {
                () =>
                {
                    Thread.Sleep(2000);
                    Console.WriteLine($"First task Executed: {DateTime.Now}");
                },

                () =>
                {
                    Thread.Sleep(6000);
                    Console.WriteLine($"Second task Executed: {DateTime.Now}");
                },

                () =>
                {
                    Thread.Sleep(9000);
                    Console.WriteLine($"Third task Executed: {DateTime.Now}");
                },

                () =>
                {
                    Thread.Sleep(12000);
                    Console.WriteLine($"Fourth task Executed: {DateTime.Now}");
                },

                () =>
                {
                    Thread.Sleep(4000);
                    Console.WriteLine($"Fifth task Executed: {DateTime.Now}");
                }
            };
        }

        private IEnumerable<Action> GetTasksWithException()
        {
            return new List<Action>
            {
                () =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine($"First task Executed: {DateTime.Now}");
                },

                () =>
                {
                    Thread.Sleep(2000);
                    Console.WriteLine($"Second task Executed: {DateTime.Now}");
                },

                () =>
                {
                    Thread.Sleep(3000);
                    throw new Exception("Very unexpected exception");
                },

                () =>
                {
                    Thread.Sleep(4000);
                    Console.WriteLine($"Fourth task Executed: {DateTime.Now}");
                }
            };
        }
    }
}
