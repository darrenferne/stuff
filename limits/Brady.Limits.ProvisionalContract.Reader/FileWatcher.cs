using Brady.Limits.ProvisionalContract.DataService;
using Brady.Limits.ProvisionalContract.DataService.RecordTypes;
using BWF.DataServices.Core.Concrete.ChangeSets;
using BWF.DataServices.Domain.Models;
using BWF.DataServices.Nancy.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Brady.Limits.ProvisionalContract.Reader
{
    internal class FileWatcher : IDisposable
    {
        IDataServiceHostSettings _dshs;
        ProvisionalContractDataService _dataService;
        ProvisionalContractRecordType _recordType;

        FileSystemWatcher _watcher;
        string _path;
        string _filter;
        string _successPath;
        string _failurePath;

        public FileWatcher(IDataServiceHostSettings dshs, IProvisionalContractDataService dataService, string path, string filter)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException(string.Format("Invalid path. The watch directory {0} is invalid.", path));

            _dshs = dshs;
            _dataService = dataService as ProvisionalContractDataService;
            _recordType = _dataService.GetRecordType(nameof(ProvisionalContract)) as ProvisionalContractRecordType;
            _path = Path.GetFullPath(path);
            _filter = filter;

            _successPath = Path.Combine(_path, "Processed");
            _failurePath = Path.Combine(_path, "Failed");
            
            _watcher = new FileSystemWatcher(_path, _filter);

            _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            _watcher.Created += new FileSystemEventHandler(ProcessFile);
            _watcher.Changed += new FileSystemEventHandler(ProcessFile);
            _watcher.EnableRaisingEvents = true;
        }
        
        private void ProcessFile(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (File.Exists(e.FullPath))
                {
                    var reader = new ContractReader(e.FullPath);
                    var contracts = reader.Read();

                    var settings = new ProcessChangeSetSettings(_dshs.SystemToken, "", true);
                    var changeSet = _recordType.GetNewChangeSet() as ChangeSet<long, Domain.ProvisionalContract>;
                    var id = 0L;

                    foreach (var contract in contracts)
                        changeSet.AddCreate(id++, contract, Enumerable.Empty<long>(), Enumerable.Empty<long>());

                    var results = _recordType.ProcessChangeSet(_dataService, changeSet, settings);

                    if (!Directory.Exists(_successPath))
                        Directory.CreateDirectory(_successPath);

                    if (File.Exists(Path.Combine(_successPath, e.Name)))
                        File.Delete(Path.Combine(_successPath, e.Name));

                    File.Move(e.FullPath, Path.Combine(_successPath, e.Name));
                }
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(_failurePath))
                    Directory.CreateDirectory(_failurePath);

                if (File.Exists(Path.Combine(_failurePath, e.Name)))
                    File.Delete(Path.Combine(_failurePath, e.Name));
                
                File.Move(e.FullPath, Path.Combine(_failurePath, e.Name));
            }
            
        }

        ~FileWatcher()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (disposing && _watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;

                _dshs = null;
                _dataService = null;
            }
        }
    }
}
