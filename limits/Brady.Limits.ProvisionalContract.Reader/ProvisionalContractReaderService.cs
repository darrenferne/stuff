using Brady.Limits.ProvisionalContract.DataService;
using BWF.DataServices.Nancy.Interfaces;
using BWF.Hosting.Infrastructure.Interfaces;
using log4net;
using Ninject;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brady.Limits.ProvisionalContract.Reader
{
    public class ProvisionalContractReaderService : IHostedService
    {
        private ILog _log;
        private IKernel _kernel;
        private IDataServiceHostSettings _dshs;
        private IProvisionalContractDataService _dataService;
        private string _path;
        private string _filter;
        private FileWatcher _watcher;

        public ProvisionalContractReaderService(IKernel kernel, IHostConfiguration hostConfig)
        {
            _log = LogManager.GetLogger(typeof(ProvisionalContractDataService));
            _kernel = kernel;
            _dshs = _kernel.Get<IDataServiceHostSettings>();
            _dataService = _kernel.Get<IProvisionalContractDataService>();

            _path = hostConfig.AppSetting("DropLocation");
            _filter = hostConfig.AppSetting("Filter");
        }

        public void Start()
        {
            try
            {
                _watcher = new FileWatcher(_dshs, _dataService, _path, _filter);
                
            }
            catch (Exception ex)
            {
                string message = "Failed to start the Brady Provisional Contract Reader Service.";
                _log.Error(message, ex);
                throw new ApplicationException(message, ex);
            }
        }

        public void Stop()
        {
            if (_watcher != null)
            {
                try
                {
                    _watcher.Dispose();
                    _watcher = null;
                }
                catch (Exception ex)
                {
                    _log.Error("Failed to stop the Brady Provisional Contract Reader Service.", ex);
                }
            }
        }
    }
}
