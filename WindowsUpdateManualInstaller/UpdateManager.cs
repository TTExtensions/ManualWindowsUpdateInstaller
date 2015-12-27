using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsUpdateManualInstaller
{
    public class UpdateManager
    {
        private dynamic updateSession;

        public UpdateManager()
        {
            updateSession = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.Session"));

        }

        public List<UpdateEntry> GetAvailableUpdates()
        {
            List<UpdateEntry> list = new List<UpdateEntry>();

            var updateSearcher = updateSession.CreateupdateSearcher();
            var searchResult = updateSearcher.Search("IsInstalled=0 and Type='Software'");
            var updates = searchResult.Updates;
            for (int i = 0; i < updates.Count; i++)
            {
                var item = updates.Item(i);
                list.Add(UpdateEntry.Create(item));
            }

            return list;
        }

        public void DownloadUpdates(List<UpdateEntry> list)
        {
            dynamic updatesToDownload = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.UpdateColl"));
            foreach (UpdateEntry e in list)
                updatesToDownload.Add(e.InternalUpdateObject);

            var downloader = updateSession.CreateUpdateDownloader();
            downloader.Updates = updatesToDownload;
            downloader.Download();
        }

        public InstallResult InstallUpdates(List<UpdateEntry> list)
        {
            dynamic updatesToInstall = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.UpdateColl"));
            List<int> updatesToInstallIndexes = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                UpdateEntry e = list[i];
                if (e.InternalUpdateObject.IsDownloaded == true)
                {
                    updatesToInstallIndexes.Add(i);
                    updatesToInstall.Add(e.InternalUpdateObject);
                }
            }

            var installer = updateSession.CreateUpdateInstaller();
            installer.Updates = updatesToInstall;

            dynamic installationResult = installer.Install();

            InstallResult result = new InstallResult()
            {
                 Result = (InstallResultCode)installationResult.ResultCode,
                 RebootRequired = installationResult.RebootRequired
            };

            result.EntryResults = new InstallEntryResult[updatesToInstallIndexes.Count];
            for (int i = 0; i < updatesToInstallIndexes.Count; i++)
            {
                var updateResult = installationResult.GetUpdateResult(i);
                result.EntryResults[i] = new InstallEntryResult()
                {
                    Result = (InstallResultCode)updateResult.ResultCode,
                    OriginalListIndex = updatesToInstallIndexes[i]
                };
            }

            return result;
        }


        public class UpdateEntry
        {
            internal dynamic InternalUpdateObject { get; set; }
            public bool AutoSelectOnWebSites { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public long MinDownloadSize { get; set; }
            public long MaxDownloadSize { get; set; }
            public string SupportUrl { get; set; }
            public int DownloadPriority { get; set; }

            public static UpdateEntry Create(dynamic updateItem)
            {
                UpdateEntry e = new UpdateEntry()
                {
                    InternalUpdateObject = updateItem,
                    AutoSelectOnWebSites = updateItem.AutoSelectOnWebSites,
                    Title = updateItem.Title,
                    Description = updateItem.Description,
                    MinDownloadSize = (long)(decimal)updateItem.MinDownloadSize,
                    MaxDownloadSize = (long)(decimal)updateItem.MaxDownloadSize,
                    SupportUrl = updateItem.SupportUrl,
                    DownloadPriority = updateItem.DownloadPriority
                };
                return e;
            }
        }

        public class InstallResult
        {
            public InstallEntryResult[] EntryResults;
            public InstallResultCode Result;
            public bool RebootRequired;
        }

        public class InstallEntryResult
        {
            public InstallResultCode Result;
            public int OriginalListIndex;
        }

        public enum InstallResultCode : int
        {
            NotStarted = 0,
            InProgress = 1,
            Succeeded = 2,
            SucceededWithErrors = 3,
            Failed = 4,
            ProcessStoppedBeforeCompleting = 5
        }
    }
}
