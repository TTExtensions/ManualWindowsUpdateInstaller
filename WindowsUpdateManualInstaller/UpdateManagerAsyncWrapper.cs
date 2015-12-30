using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsUpdateManualInstaller
{
    public class UpdateManagerAsyncWrapper : IDisposable
    {

        private Thread updateRunnerThread;
        private BlockingCollection<ThreadRunEntry> threadQueue;
        private SemaphoreSlim waitSemaphore = new SemaphoreSlim(0);

        private UpdateManager updateManager;

        public UpdateManagerAsyncWrapper()
        {
            // Set up the thread.
            threadQueue =
                new BlockingCollection<ThreadRunEntry>();
            updateRunnerThread = new Thread(() => RunThread(threadQueue));
            updateRunnerThread.Start();

            AddThreadAction(() =>
            {
                updateManager = new UpdateManager();
            });
        }

        private void RunThread(BlockingCollection<ThreadRunEntry> queue)
        {
            using (queue)
            {
                using (waitSemaphore)
                {
                    while (true)
                    {
                        ThreadRunEntry e = queue.Take();
                        if (e.exit)
                            return;

                        e.action();
                    }
                }
            }
        }

        private void AddThreadAction(Action action)
        {
            ThreadRunEntry e = new ThreadRunEntry()
            {
                action = action
            };
            threadQueue.Add(e);
        }


        public async Task<List<UpdateManager.UpdateEntry>> GetAvailableUpdatesAsync()
        {
            List<UpdateManager.UpdateEntry> val = null;
            Exception ex = null;
            AddThreadAction(() =>
            {
                try {
                    val = updateManager.GetAvailableUpdates();
                }
                catch (Exception e)
                {
                    ex = e;
                }
                finally
                {
                    waitSemaphore.Release();
                }
            });

            await waitSemaphore.WaitAsync();
            if (ex != null)
                throw ex;
            else
                return val;
        }

        public async Task DownloadUpdatesAsync(List<UpdateManager.UpdateEntry> list)
        {
            Exception ex = null;
            AddThreadAction(() =>
            {
                try
                {
                    updateManager.DownloadUpdates(list);
                }
                catch (Exception e)
                {
                    ex = e;
                }
                finally
                {
                    waitSemaphore.Release();
                }
            });

            await waitSemaphore.WaitAsync();
            if (ex != null)
                throw ex;
        }

        public async Task<UpdateManager.InstallResult> InstallUpdatesAsync(List<UpdateManager.UpdateEntry> list)
        {
            UpdateManager.InstallResult val = null;
            Exception ex = null;
            AddThreadAction(() =>
            {
                try
                {
                    val = updateManager.InstallUpdates(list);
                }
                catch (Exception e)
                {
                    ex = e;
                }
                finally
                {
                    waitSemaphore.Release();
                }
            });

            await waitSemaphore.WaitAsync();
            if (ex != null)
                throw ex;
            else
                return val;
        }


        public void Dispose()
        {
            threadQueue.Add(new ThreadRunEntry() { exit = true });
            updateRunnerThread.Join();
        }


        private struct ThreadRunEntry
        {
            public bool exit;
            public Action action;
        }
    }
}
