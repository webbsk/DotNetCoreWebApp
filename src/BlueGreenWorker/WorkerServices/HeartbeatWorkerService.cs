//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.MarketplaceServicesCore.Core;

namespace Microsoft.MarketplaceServices.ReferenceWorker.WorkerServices
{
    /// <summary>
    /// A simple implementation of a hosted worker that writes some stuff to the console. These
    /// <see cref="IHostedService"/> in the <see cref="IHost"/> are analogous to controllers in a
    /// <see cref="IWebHost"/>.
    /// </summary>
    public sealed class HeartbeatWorkerService : IHostedService, IDisposable
    {
        /// <summary>
        /// The cancellation token source that signals the long-running background worker task to terminate.
        /// </summary>
        CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// The id of this task; just a demo value.
        /// </summary>
        readonly string id;

        /// <summary>
        /// A simple counter denoting amount of work done.
        /// </summary>
        long counter;

        /// <summary>
        /// The background worker task.
        /// </summary>
        Task workerTask;

        /// <summary>
        /// Create a default hosted worker that does some unit of work.
        /// </summary>
        public HeartbeatWorkerService()
        {
            this.id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// The main loop for the background worker task.
        /// </summary>
        /// <returns>A task that is stopped when <see cref="cancellationTokenSource"/> is cancelled.</returns>
        async Task LoopAsync()
        {
            // the main loop, do your unit-of-work and processing here
            while (!this.cancellationTokenSource.Token.IsCancellationRequested)
            {
                this.counter++;

                Console.WriteLine($"Heartbeat worker {this.id} is alive");
                Console.WriteLine($"Memory consumed: {GC.GetTotalMemory(false)} bytes");
                Console.WriteLine($"Work done: {this.counter}");

                await Task.Delay(1000).OnAnyContext();
            }
        }

        /// <summary>
        /// Start the background worker.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // make sure worker is actually stopped, before starting
            await this.StopAsync(cancellationToken).OnAnyContext();

            // start the background worker task
            this.cancellationTokenSource = new CancellationTokenSource();
            this.workerTask = await Task
                .Factory
                .StartNew(
                    this.LoopAsync,
                    this.cancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default)
                .OnAnyContext();
        }

        /// <summary>
        /// Stop the background worker.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // cancel the worker task's cancellation token source
            if (this.cancellationTokenSource != null)
            {
                using (this.cancellationTokenSource)
                {
                    this.cancellationTokenSource.Cancel();
                }

                this.cancellationTokenSource = null;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Dispose of the worker.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
