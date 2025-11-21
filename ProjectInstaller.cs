using System.ComponentModel;
using System.ServiceProcess;

namespace DatabaseBackupService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            InitializeComponent();

            // Service account
            processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            // Service configuration
            serviceInstaller = new ServiceInstaller
            {
                ServiceName = "DatabaseBackupService",
                DisplayName = "Database Backup Service",
                StartType = ServiceStartMode.Manual,
                Description = "Database Backup Service every time interval you specify in .config file.",
                ServicesDependedOn = new string[] { "RpcSs", "EventLog", "MSSQLSERVER" } // Dependencies
            };

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);

        }

    }
}
