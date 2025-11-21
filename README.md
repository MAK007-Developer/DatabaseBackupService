

🗄️ Database Backup Windows Service

A lightweight and reliable Windows Service that automates SQL Server database backups at configurable time intervals.






📌 Overview

The Database Backup Service is a Windows Service built using C# (.NET Framework) to automatically back up a SQL Server database at fixed intervals.
It runs continuously in the background, performs scheduled backups, and logs all activities for reliability and auditing.

This service is ideal for developers, DB admins, or small applications that need automated, low-maintenance backup without setting up SQL Agent Jobs or external schedulers.

🚀 Features
🔄 Automated Backup Execution

Backs up a specified SQL Server database at intervals defined by the user in the .config file.

Uses System.Timers.Timer to trigger backup operations consistently.

⚙️ Configurable Through App.config

Define all key parameters without modifying code:

<appSettings>
  <add key="DatabaseName" value="MyDatabase" />
  <add key="BackupFolder" value="C:\DatabaseBackups" />
  <add key="BackupIntervalMinutes" value="30" />
  <add key="SqlConnectionString" value="Server=MYPC\SQLEXPRESS;Database=master;Trusted_Connection=True;" />
  <add key="LogFolder" value="C:\DatabaseLogs" />
</appSettings>

📝 Event Logging

The service logs:

Service start and stop events

Backup start and success messages

Backup completion path

Errors and exception details

SQL execution failures

Timer-triggered events

Logs are written to:

A daily log file

Windows Event Viewer

🧯 Graceful Error Handling

Try/catch around database operations

SQL exceptions logged with timestamp

Service continues running even after failures

🛠️ Minimal Setup & Deployment

Includes a ProjectInstaller for proper Windows Service configuration:

Sets service name

Startup type: Automatic

Supports InstallUtil installation

🧩 How It Works

Service starts → loads configuration values

Validates or creates the backup & log directories

Timer triggers according to the interval

Service runs SQL command:

BACKUP DATABASE [DatabaseName]
TO DISK = 'C:\DatabaseBackups\Database_YYYYMMDD_HHMMSS.bak'
WITH INIT;


Backup file is created with a timestamp

All actions logged with timestamps

On error → service logs it and continues running

🧪 Console Mode for Debugging (Optional)

When run interactively (e.g., in Visual Studio), the service can switch to console mode:

Displays log messages in the console

Allows manually stopping the program

Useful for verifying SQL connection, backup logic, logging, and configuration

📦 Installation (Service Mode)
1. Build the project in Release mode

In Visual Studio → Build → Configuration: Release

2. Copy output files

Located in:

bin\Release\


Includes:

<ServiceName>.exe

.config file

Required DLLs

3. Install using InstallUtil

For 64-bit systems:

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe DatabaseBackupService.exe


For 32-bit systems:

C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe DatabaseBackupService.exe

4. Start the service

Open Services.msc → find DatabaseBackupService → Start

📂 Example Log Output
[2025-01-21 09:10:00] Service Started.
[2025-01-21 09:10:00] Backup triggered.
[2025-01-21 09:10:01] Backup completed: C:\DatabaseBackups\MyDatabase_20250121_091001.bak
[2025-01-21 09:40:00] Backup triggered.
[2025-01-21 09:40:02] Error: Cannot open database requested in login.
[2025-01-21 09:40:02] Service Continues Running.
[2025-01-21 10:00:00] Service Stopped.

📁 Folder Structure Example
DatabaseBackupService/
├── bin/
│   └── Release/
├── Logs/
├── Installers/
├── DatabaseBackupService.cs
├── BackupManager.cs
├── ProjectInstaller.cs
├── App.config
└── README.md

🧠 Design Considerations

✔ Running as a Windows Service ensures uninterrupted operation
✔ Centralized logging enables auditing and troubleshooting
✔ Timer-based scheduling avoids resource-heavy loops
✔ App.config-based settings allow non-technical users to adjust backup intervals
✔ Service recovers automatically after restart

🧰 Tech Stack

C#

.NET Framework

Windows Service API

SQL Server

File I/O & Logging

Timer-based scheduling

📜 License

This project is available for personal or educational use.
