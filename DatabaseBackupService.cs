using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Timers;
using System.IO;


namespace DatabaseBackupService
{
    public partial class DatabaseBackupService : ServiceBase
    {

        private string logDirectory;
        private string logFilePath;

        private Timer timer;

        private string DBBackupFileName;


        string BackUpDatabase()
        {
            

            try
            {
                string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
                SqlConnection connection = new SqlConnection(connectionString);

                string Query = "EXEC SP_BackUpC21DB @BackUpFolder = @BackupFilename";

                SqlCommand command = new SqlCommand(Query, connection);
                command.CommandType = CommandType.Text;


                //D:\DatabaseBackups\C21_DB1_20251121_1138.bak
                string DBBackupFileName = ConfigurationManager.AppSettings["BackupFolder"] + "\\"
                    + ConfigurationManager.AppSettings["DatabaseToBackup"] + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".bak";

                // Add parameters
                command.Parameters.AddWithValue("@BackupFilename", DBBackupFileName);
                // Execute
                connection.Open();
                command.ExecuteScalar();

                connection.Close();
                connection.Dispose();
                command.Dispose();

                return DBBackupFileName;
            }
            catch (Exception ex)
            {
                LogServiceEvent($"Error during backup: {ex.Message}");
                return null;
            }
            finally
            {
                
            }
        }

        void CallingBackupProcedure(object sender, ElapsedEventArgs e)
        {

            DBBackupFileName = BackUpDatabase();

            
                if (!string.IsNullOrEmpty(DBBackupFileName))
                {
                    LogServiceEvent("Database backup sucessful: " + ConfigurationManager.AppSettings["BackupFolder"] + $"\\{DBBackupFileName}");
                }
            
            



        }


        void InitializeTimer()
        {

            if (double.TryParse(ConfigurationManager.AppSettings["BackupIntervalMinutes"], out double BackupIntervalMinutes))
            {
                timer = new Timer(BackupIntervalMinutes * 60 * 1000); // x minutes
                timer.Elapsed += CallingBackupProcedure;
                timer.AutoReset = true;
                timer.Start();

            }


        }


        private void LogServiceEvent(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
            File.AppendAllText(logFilePath, logMessage);

            // Write to console if running interactively
            if (Environment.UserInteractive)
            {
                Console.WriteLine(logMessage);
            }

        }

        public DatabaseBackupService()
        {
            InitializeComponent();

            

            CanPauseAndContinue = true; //The service supports pausing and resuming operations.

            // Enable support for OnShutdown
            CanShutdown = true; // The service is notified when the system shuts down.


            // Read log directory path from App.config
            //The service reads the log directory path from an external configuration file (App.config) for flexibility.
            logDirectory = ConfigurationManager.AppSettings["LogDirectory"];


            // Validate and create directory if it doesn't exist
            if (string.IsNullOrWhiteSpace(logDirectory))
            {
                LogServiceEvent("LogDirectory is not specified in the configuration file.");
                throw new ConfigurationErrorsException("LogDirectory is not specified in the configuration file.");
            }

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            logFilePath = Path.Combine(logDirectory, "BackupServiceLogs.txt");
        }

        protected override void OnStart(string[] args)
        {
            LogServiceEvent("Service Started");

            InitializeTimer();

        }

        protected override void OnStop()
        {


            LogServiceEvent("Service Stopped");
            timer.Stop();
            timer.Dispose();

        }


        public void StartInConsole()
        {

            OnStart(null); // Trigger OnStart logic
            Console.WriteLine("Press Enter to stop the service...");
            Console.ReadLine(); // Wait for user input to simulate service stopping
            OnStop(); // Trigger OnStop logic
            Console.ReadKey();

        }




    }
}
