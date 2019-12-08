//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage.Extensions
{
    /// <summary>
    /// Extensions for use internally by PCLStorage for awaiting.
    /// </summary>
    internal static class AwaitExtensions
    {
        public static void RunSync(this Task task) => task.GetAwaiter().GetResult();
        public static T RunSync<T>(this Task<T> task) => task.GetAwaiter().GetResult();
#if WINDOWS_UWP
        public static T RunSync<T>(this Windows.Foundation.IAsyncOperation<T> task) => task.GetAwaiter().GetResult();
#endif

        /// <summary>
        /// Causes the caller who awaits this method to
        /// switch off the Main thread. It has no effect if
        /// the caller is already off the main thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable that does the thread switching magic.</returns>
        internal static TaskSchedulerAwaiter SwitchOffMainThreadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new TaskSchedulerAwaiter(SynchronizationContext.Current != null ? TaskScheduler.Default : null, cancellationToken);
        }

        internal readonly struct TaskSchedulerAwaiter : INotifyCompletion
        {
            private readonly TaskScheduler? _taskScheduler;
            private readonly CancellationToken _cancellationToken;

            internal TaskSchedulerAwaiter(TaskScheduler? taskScheduler, CancellationToken cancellationToken)
            {
                _taskScheduler = taskScheduler;
                _cancellationToken = cancellationToken;
            }

            internal TaskSchedulerAwaiter GetAwaiter() => this;

            public bool IsCompleted => _taskScheduler == null;

            public void OnCompleted(Action continuation)
            {
                if (_taskScheduler == null)
                    throw new InvalidOperationException("IsCompleted is true, so this is unexpected.");

                Task.Factory.StartNew(continuation, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
            }

            public void GetResult() => _cancellationToken.ThrowIfCancellationRequested();
        }
    }
}