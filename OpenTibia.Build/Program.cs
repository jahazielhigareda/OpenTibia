using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenTibia.Build
{
    internal class Program
    {
        private static LogLevel logLevelFilter = LogLevel.Debug;

        static async Task<int> Main(string[] args)
        {
            var tcs = new TaskCompletionSource<string>();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;

                tcs.SetResult(null);
            };

            WriteLine("Available commands:");
            WriteLine("help, publish, migration-config, migration-add, migration-remove, migration-script, clear, exit, about, support");
            WriteLine();

            var configDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTOTS");
                                           
            var configFilePath = Path.Combine(configDirectoryPath, ".config");

            try
            {
                bool exit = false;

                while ( !exit )
                {
                    var option = await await Task.WhenAny(tcs.Task, Task.Run( () => Console.ReadLine() ) );
                        
                    switch (option)
                    {
                        case "help":

                            WriteLine("publish\t\t\tRebuild the solution, run tests, publish and zip.");
                            WriteLine("migration-config\tCreate migration configuration file.");
                            WriteLine("migration-add\t\tCreate migration for SQLite, MySQL, Microsoft SQL Server, Postgre SQL and Oracle databases.");
                            WriteLine("migration-remove\tRemove last migration for SQLite, MySQL, Microsoft SQL Server, Postgre SQL and Oracle databases.");
                            WriteLine("migration-script\tGenerate migration scripts for SQLite, MySQL, Microsoft SQL Server, Postgre SQL and Oracle databases.");
                            WriteLine("clear\t\t\tClear the console screen. Alternative commands: cls.");
                            WriteLine("exit\t\t\tExit this build tool.");
                            WriteLine("about\t\t\tDisplay license.");
                            WriteLine("support\t\t\tDisplay donation address.");
                            WriteLine();

                            break;

                        case "publish":
                            {
                                WriteLine("Remember to update server, console and gui versions.", LogLevel.Information);

                                WriteLine("Running tests...", LogLevel.Information);

                                    int exitCode = Run(
                                        workingDirectory: "OpenTibia.Tests", 
                                        command: "dotnet", 
                                        arguments: "test OpenTibia.Tests.csproj -c Release");

                                    if (exitCode != 0)
                                    {
                                        WriteLine("Could not test OpenTibia.Tests, exited with code " + exitCode, LogLevel.Error);

                                        break;
                                    }

                                WriteLine("Building plugins...", LogLevel.Information);

                                    DirectoryDelete(
                                        directoryName: "OpenTibia.Plugins\\bin\\Release\\netstandard2.0");

                                    exitCode = Run(
                                        workingDirectory: "OpenTibia.Plugins", 
                                        command: "dotnet", 
                                        arguments: "build OpenTibia.Plugins.csproj -c Release");

                                    if (exitCode != 0)
                                    {
                                        WriteLine("Could not build OpenTibia.Plugins, exited with code " + exitCode, LogLevel.Error);

                                        break;
                                    }

                                    FileCopy(
                                        sourceFileName: "OpenTibia.Plugins\\bin\\Release\\netstandard2.0\\OpenTibia.Plugins.dll", 
                                        destinationFileName: "OpenTibia.GameData\\data\\dlls\\OpenTibia.Plugins\\OpenTibia.Plugins.dll");

                                WriteLine("Building Windows (console) application...", LogLevel.Information);
                             
                                    DirectoryDelete(
                                        directoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\win-x64\\publish");

                                    exitCode = Run(
                                        workingDirectory: "OpenTibia.Host", 
                                        command: "dotnet", 
                                        arguments: "publish OpenTibia.Host.csproj -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false");

                                    if (exitCode != 0)
                                    {
                                        WriteLine("Could not publish OpenTibia.Host for win-x64, exited with code " + exitCode, LogLevel.Error);

                                        break;
                                    }

                                    FileDeletePdb(
                                        directoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\win-x64\\publish");

                                    DirectoryCopy(
                                        sourceDirectoryName: "OpenTibia.GameData\\data", 
                                        destinationDirectoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\win-x64\\publish\\data");

                                    Zip(
                                        directoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\win-x64\\publish", 
                                        fileName: "OpenTibia.Host\\bin\\Release\\net8.0\\win-x64\\OpenTibia.Host_win-x64.zip");

                                WriteLine("Building Linux (console) application...", LogLevel.Information);

                                    DirectoryDelete(
                                        directoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\linux-x64\\publish");

                                    exitCode = Run(
                                        workingDirectory: "OpenTibia.Host",
                                        command: "dotnet", 
                                        arguments: "publish OpenTibia.Host.csproj -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained true");

                                    if (exitCode != 0)
                                    {
                                        WriteLine("Could not publish OpenTibia.Host for linux-x64, exited with code " + exitCode, LogLevel.Error);

                                        break;
                                    }

                                    FileDeletePdb(
                                         directoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\linux-x64\\publish");

                                    DirectoryCopy(
                                        sourceDirectoryName: "OpenTibia.GameData\\data",
                                        destinationDirectoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\linux-x64\\publish\\data");

                                    DirectoryDelete(
                                        directoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\linux-x64\\publish\\data\\clibs");

                                    DirectoryDelete(
                                        directoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\linux-x64\\publish\\data\\lualibs");

                                    DirectoryCreate(
                                        directoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\linux-x64\\publish\\data\\clibs");

                                    DirectoryCreate(
                                        directoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\linux-x64\\publish\\data\\lualibs");

                                    Zip(
                                        directoryName: "OpenTibia.Host\\bin\\Release\\net8.0\\linux-x64\\publish",
                                        fileName: "OpenTibia.Host\\bin\\Release\\net8.0\\linux-x64\\OpenTibia.Host_linux-x64.zip");

                                WriteLine("Building Windows (GUI) application...", LogLevel.Information);

                                    DirectoryDelete(
                                        directoryName: "OpenTibia.Host.GUI\\bin\\Release\\net8.0-windows\\win-x64\\publish");

                                    exitCode = Run(
                                        workingDirectory: "OpenTibia.Host.GUI", 
                                        command: "dotnet",
                                        arguments: "publish OpenTibia.Host.GUI.csproj -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false");

                                    if (exitCode != 0)
                                    {
                                        WriteLine("Could not publish OpenTibia.Host.GUI for win-x64, exited with code " + exitCode, LogLevel.Error);

                                        break;
                                    }

                                    FileDeletePdb(
                                        directoryName: "OpenTibia.Host.GUI\\bin\\Release\\net8.0-windows\\win-x64\\publish");

                                    DirectoryCopy(
                                        sourceDirectoryName: "OpenTibia.GameData\\data", 
                                        destinationDirectoryName: "OpenTibia.Host.GUI\\bin\\Release\\net8.0-windows\\win-x64\\publish\\data");

                                    Zip(
                                        directoryName: "OpenTibia.Host.GUI\\bin\\Release\\net8.0-windows\\win-x64\\publish",
                                        fileName: "OpenTibia.Host.GUI\\bin\\Release\\net8.0-windows\\win-x64\\OpenTibia.Host.GUI_win-x64.zip");

                                WriteLine("Done... 3 files were created:");
                                WriteLine("OpenTibia\\OpenTibia.Host\\bin\\Release\\net8.0\\win-x64\\OpenTibia.Host_win-x64.zip");
                                WriteLine("OpenTibia\\OpenTibia.Host\\bin\\Release\\net8.0\\linux-x64\\OpenTibia.Host_linux-x64.zip");
                                WriteLine("OpenTibia\\OpenTibia.Host.GUI\\bin\\Release\\net8.0-windows\\win-x64\\OpenTibia.Host.GUI_win-x64.zip");
                                WriteLine();

                                WriteLine("OpenTibia.Host_win-x64.zip");
                                WriteLine(Hash("OpenTibia.Host\\bin\\Release\\net8.0\\win-x64\\OpenTibia.Host_win-x64.zip") );
                                WriteLine();

                                WriteLine("OpenTibia.Host_linux-x64.zip");
                                WriteLine(Hash("OpenTibia.Host\\bin\\Release\\net8.0\\linux-x64\\OpenTibia.Host_linux-x64.zip") );
                                WriteLine();

                                WriteLine("OpenTibia.Host.GUI_win-x64.zip");
                                WriteLine(Hash("OpenTibia.Host.GUI\\bin\\Release\\net8.0-windows\\win-x64\\OpenTibia.Host.GUI_win-x64.zip") );
                                WriteLine();
                            }
                            break;

                        case "migration-config":
                            {
                                var config = new Config();

                                WriteLine("SQLite connection string:");

                                config.SqliteConnectionString = Console.ReadLine();

                                WriteLine("MySQL connection string:");

                                config.MysqlConnectionString = Console.ReadLine();

                                WriteLine("Microsoft SQL Server connection string:");

                                config.MssqlConnectionString = Console.ReadLine();

                                WriteLine("Postgre SQL Server connection string:");

                                config.PostgresqlConnectionString = Console.ReadLine();

                                WriteLine("Oracle connection string:");

                                config.OracleConnectionString = Console.ReadLine();

                                Directory.CreateDirectory(configDirectoryPath);

                                File.WriteAllBytes(configFilePath, ProtectedData.Protect(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(config) ), null, DataProtectionScope.CurrentUser) );

                                WriteLine("Done... 1 encrypted file was created:");
                                WriteLine(configFilePath);
                                WriteLine();
                            }
                            break;

                        case "migration-add":
                            {
                                if ( !File.Exists(configFilePath) )
                                {
                                    WriteLine("Migration configuration file not found. Create one using the migration-config option.", LogLevel.Error);

                                    break;
                                }

                                WriteLine("New migration name:");

                                string name = Console.ReadLine();

                                if ( !string.IsNullOrEmpty(name) )
                                {
                                    var config = JsonSerializer.Deserialize<Config>(Encoding.UTF8.GetString(ProtectedData.Unprotect(File.ReadAllBytes(configFilePath), null, DataProtectionScope.CurrentUser)));

                                    foreach (var item in new[] {
                                        ("SQLite", "OpenTibia.Data.Sqlite", config.SqliteConnectionString),
                                        ("MySQL", "OpenTibia.Data.MySql", config.MysqlConnectionString),
                                        ("Microsoft SQL Server", "OpenTibia.Data.MsSql", config.MssqlConnectionString), 
                                        ("Postgre SQL", "OpenTibia.Data.PostgreSql", config.PostgresqlConnectionString),
                                        ("Oracle", "OpenTibia.Data.Oracle", config.OracleConnectionString) } )
                                    {
                                        if ( !string.IsNullOrEmpty(item.Item3) )
                                        {
                                            WriteLine("Scripting " + item.Item1 + "...", LogLevel.Information);
                                    
                                            int exitCode = Run(
                                                workingDirectory: item.Item2, 
                                                command: "dotnet",
                                                arguments: "ef migrations add \"" + name + "\" -- \"" + item.Item3 + "\"");

                                            if (exitCode != 0)
                                            {
                                                WriteLine("Could not generate script for " + item.Item2 + ", exited with code " + exitCode, LogLevel.Warning);
                                            }
                                        }
                                    }
                                }

                                WriteLine("Done.");
                                WriteLine();
                            }
                            break;

                        case "migration-remove":
                            {
                                if ( !File.Exists(configFilePath) )
                                {
                                    WriteLine("Migration configuration file not found. Create one using the migration-config option.", LogLevel.Error);

                                    break;
                                }

                                var config = JsonSerializer.Deserialize<Config>(Encoding.UTF8.GetString(ProtectedData.Unprotect(File.ReadAllBytes(configFilePath), null, DataProtectionScope.CurrentUser) ) );

                                foreach (var item in new[] {
                                    ("SQLite", "OpenTibia.Data.Sqlite", config.SqliteConnectionString),
                                    ("MySQL", "OpenTibia.Data.MySql", config.MysqlConnectionString),
                                    ("Microsoft SQL Server", "OpenTibia.Data.MsSql", config.MssqlConnectionString), 
                                    ("Postgre SQL", "OpenTibia.Data.PostgreSql", config.PostgresqlConnectionString),
                                    ("Oracle", "OpenTibia.Data.Oracle", config.OracleConnectionString) } )
                                {
                                    if ( !string.IsNullOrEmpty(item.Item3) )
                                    {
                                        WriteLine("Scripting " + item.Item1 + "...", LogLevel.Information);
                                    
                                        int exitCode = Run(
                                            workingDirectory: item.Item2, 
                                            command: "dotnet",
                                            arguments: "ef migrations remove -- \"" + item.Item3 + "\"");

                                        if (exitCode != 0)
                                        {
                                            WriteLine("Could not generate script for " + item.Item2 + ", exited with code " + exitCode, LogLevel.Warning);
                                        }
                                    }
                                }

                                WriteLine("Done.");
                                WriteLine();
                            }
                            break;

                        case "migration-script":
                            {
                                if ( !File.Exists(configFilePath) )
                                {
                                    WriteLine("Migration configuration file not found. Create one using the migration-config option.", LogLevel.Error);

                                    break;
                                }

                                WriteLine("Last migration name:");

                                string name = Console.ReadLine();

                                var config = JsonSerializer.Deserialize<Config>(Encoding.UTF8.GetString(ProtectedData.Unprotect(File.ReadAllBytes(configFilePath), null, DataProtectionScope.CurrentUser) ) );

                                foreach (var item in new[] {
                                    ("SQLite", "OpenTibia.Data.Sqlite", config.SqliteConnectionString),
                                    ("MySQL", "OpenTibia.Data.MySql", config.MysqlConnectionString),
                                    ("Microsoft SQL Server", "OpenTibia.Data.MsSql", config.MssqlConnectionString), 
                                    ("Postgre SQL", "OpenTibia.Data.PostgreSql", config.PostgresqlConnectionString),
                                    ("Oracle", "OpenTibia.Data.Oracle", config.OracleConnectionString) } )
                                {
                                    if ( !string.IsNullOrEmpty(item.Item3) )
                                    {
                                        WriteLine("Scripting " + item.Item1 + "...", LogLevel.Information);
                                    
                                        int exitCode = Run(
                                            workingDirectory: item.Item2, 
                                            command: "dotnet",
                                            arguments: string.IsNullOrEmpty(name) ? "ef migrations script -- \"" + item.Item3 + "\"" : 
                                                                                    "ef migrations script \"" + name + "\" -- \"" + item.Item3 + "\"");

                                        if (exitCode != 0)
                                        {
                                            WriteLine("Could not generate script for " + item.Item2 + ", exited with code " + exitCode, LogLevel.Warning);
                                        }
                                    }
                                }

                                WriteLine("Done.");
                                WriteLine();
                            }
                            break;

                        case "clear":
                        case "cls":

                            Console.Clear();

                            break;

                        case null:
                        case "exit":

                            exit = true;

                            break;

                        case "about":

                            WriteLine("MTOTS - An open Tibia server developed by ");
                            WriteLine("Copyright (C) 2024 ");
                            WriteLine();
                            WriteLine("This program is free software: you can redistribute it and/or modify");
                            WriteLine("it under the terms of the GNU General Public License as published by");
                            WriteLine("the Free Software Foundation, either version 3 of the License, or");
                            WriteLine("(at your option) any later version.");
                            WriteLine();
                            WriteLine("This program is distributed in the hope that it will be useful,");
                            WriteLine("but WITHOUT ANY WARRANTY; without even the implied warranty of");
                            WriteLine("MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the");
                            WriteLine("GNU General Public License for more details.");
                            WriteLine();
                            WriteLine("You should have received a copy of the GNU General Public License");
                            WriteLine("along with this program. If not, see <https://www.gnu.org/licenses/>.");
                            WriteLine();

                            break;

                        case "support":

                            WriteLine("If you enjoy using open source projects and would like to support our work,");
                            WriteLine("consider making a donation! Your contributions help us maintain and improve");
                            WriteLine("the project. You can support us by sending directly to the following address:");
                            WriteLine("");
                            WriteLine("Bitcoin (BTC) Address: bc1qc2p79gtjhnpff78su86u8vkynukt8pmfnr43lf");
                            WriteLine("");
                            WriteLine("Monero (XMR) Address: 87KefRhqaf72bYBUF3EsUjY89iVRH72GsRsEYZmKou9ZPFhGaGzc1E4URbCV9oxtdTYNcGXkhi9XsRhd2ywtt1bq7PoBfd4");
                            WriteLine("");
                            WriteLine("Thank you for your support!");
                            WriteLine("Every contribution, no matter the size, makes a difference.");
                            WriteLine("");

                            break;  
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString(), LogLevel.Error);

                return 1;
            }

            return 0;
        }

        private enum LogLevel
        {
            Debug,

            Information,

            Warning,

            Error,

            Default
        }

        private static readonly object sync = new object();

        private static void WriteLine(string message = null, LogLevel logLevel = LogLevel.Default)
        {
            if (logLevel < logLevelFilter)
            {
                return;
            }

            lock (sync)
            {
                switch (logLevel)
                {
                    case LogLevel.Debug:
                                
                        Console.ForegroundColor = ConsoleColor.Green;

                        break;

                    case LogLevel.Information:

                        Console.ForegroundColor = ConsoleColor.Blue;

                        break;

                    case LogLevel.Warning:

                        Console.ForegroundColor = ConsoleColor.DarkYellow;

                        break;

                    case LogLevel.Error:

                        Console.ForegroundColor = ConsoleColor.Red;

                        break;

                    default:

                        Console.ResetColor();

                        break;
                }

                Console.WriteLine(message);

                Console.ResetColor();
            }
        }

        private static void FileDeletePdb(string directoryName)
        {
            directoryName = Path.Combine(GetBaseDirectory(), directoryName);

            foreach (var file in Directory.GetFiles(directoryName, "*.pdb" ) )
            {
                File.Delete(file);
            }
        }

        private static void FileCopy(string sourceFileName, string destinationFileName)
        {
            sourceFileName = Path.Combine(GetBaseDirectory(), sourceFileName);
            destinationFileName = Path.Combine(GetBaseDirectory(), destinationFileName);

            File.Copy(sourceFileName, destinationFileName, true);
        }

        private static void DirectoryCreate(string directoryName)
        {
            directoryName = Path.Combine(GetBaseDirectory(), directoryName);

            Directory.CreateDirectory(directoryName);
        }

        private static void DirectoryDelete(string directoryName)
        {
            directoryName = Path.Combine(GetBaseDirectory(), directoryName);

            if (Directory.Exists(directoryName) )
            {
                Directory.Delete(directoryName, true);
            }
        }

        private static void DirectoryCopy(string sourceDirectoryName, string destinationDirectoryName)
        {
            sourceDirectoryName = Path.Combine(GetBaseDirectory(), sourceDirectoryName);
            destinationDirectoryName = Path.Combine(GetBaseDirectory(), destinationDirectoryName);

            CopyEachItem(sourceDirectoryName, destinationDirectoryName);

            void CopyEachItem(string source, string destination)
            {
                Directory.CreateDirectory(destination);

                foreach (var file in Directory.GetFiles(source) )
                {
                    File.Copy(file, Path.Combine(destination, Path.GetFileName(file) ) );
                }

                foreach (var directory in Directory.GetDirectories(source) )
                {
                    CopyEachItem(directory, Path.Combine(destination, Path.GetFileName(directory) ) );
                }
            }
        }

        private static void Zip(string directoryName, string fileName)
        {
            directoryName = Path.Combine(GetBaseDirectory(), directoryName);
            fileName = Path.Combine(GetBaseDirectory(), fileName);

            if (File.Exists(fileName) )
            {
                File.Delete(fileName);
            }

            ZipFile.CreateFromDirectory(directoryName, fileName, CompressionLevel.SmallestSize, false);
        }

        private static string Hash(string fileName)
        {
            fileName = Path.Combine(GetBaseDirectory(), fileName);

            using (var sha256 = SHA256.Create() )
            {
                using (var fileStream = File.OpenRead(fileName) )
                {
                    return Convert.ToHexString(sha256.ComputeHash(fileStream) );
                }
            }
        }

        private static int Run(string workingDirectory, string command, string arguments)
        {
            workingDirectory = Path.Combine(GetBaseDirectory(), workingDirectory);

            using (var process = new Process() )
            {
                process.StartInfo.WorkingDirectory = workingDirectory;

                process.StartInfo.FileName = command;

                process.StartInfo.Arguments = arguments;

                process.StartInfo.RedirectStandardOutput = true;

                process.StartInfo.RedirectStandardError = true;

                process.StartInfo.CreateNoWindow = true;

                process.OutputDataReceived += (s, e) =>
                {
                    WriteLine(e.Data, LogLevel.Debug);
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    WriteLine(e.Data, LogLevel.Debug);
                };

                process.Start();

                process.BeginOutputReadLine();

                process.BeginErrorReadLine();   
                
                process.WaitForExit();

                return process.ExitCode;
            }
        }

        private static string GetBaseDirectory()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null)
            {
                if (directory.Name == "OpenTibia")
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new NotImplementedException();
        }

        private class Config
        {
            public string SqliteConnectionString { get; set; }

            public string MysqlConnectionString { get; set; }

            public string MssqlConnectionString { get; set; }

            public string PostgresqlConnectionString { get; set; }

            public string OracleConnectionString { get; set; }
        }
    }
}
