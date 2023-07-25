﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace Overwatch
{
    internal class Watcher
    {
        private static string _logPath;
        public static string LogPath
        {
            get { return _logPath; }
            set { _logPath = value; }
        }
        FileSystemWatcher watcher;
        static Thread thread;
        static long eventCounter;

        // DLLImport for getting the event if the application is closed
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;
        static DateTime startTime;
        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            // Calculating the duration of the length of the programs use
            TimeSpan duration = DateTime.Now - startTime;

            // Here for opereations before closing
            StreamWriter sw = new StreamWriter("Logs.txt", true);

            // Logging the process to a Textfile
            string msg = $"\n----------\nProcess from\t{startTime}\n" +
                $"Stopped at:\t{DateTime.Now}\n" +
                $"Duration: {((int)duration.TotalHours)}h {((int)duration.TotalMinutes)}m " +
                $"{((int)duration.TotalSeconds)}s\n" +
                $"Total events: {eventCounter}\n" +
                $"Tracked directory: {_logPath}";
            sw.WriteLine(msg);

            sw.Close();

            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    return false;
            }
        }

        public Watcher(string path)
        {
            watcher = new FileSystemWatcher(path);
            _logPath = path;
            eventCounter = 0;
        }

        public void Watch()
        {
            Console.Title = $"Logging in {_logPath}...";

            // Adds the events to the FileSystemWatcher
            watcher.Created += OnFileCreated;
            
            watcher.Changed += OnFileChanged;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            // Adds an event for when the console is closed
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            // Saves the timestamp when the program starts
            startTime = DateTime.Now;


        }
        // Wenn eine Datei oder etwas in einem Pfad umbenannt wird
        private static void OnRenamed(object sender, RenamedEventArgs e) 
        {
            CountLog(LogString($"File renamed: {e.Name}  from  {e.OldName}"), ConsoleColor.Yellow);
        }
        // Wenn eine Datei erstellt wird
        private static void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            CountLog(LogString($"File created: {e.FullPath}"), ConsoleColor.Green);
        } 
        // Wenn eine Datei geändert wird
        private static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            CountLog(LogString($"File changed: {e.FullPath}"), ConsoleColor.Cyan);
        }
        // Bei einem Error
        private static void OnError(object sender, ErrorEventArgs e) => PrintException(e.GetException()); 
        
        // Default log with number entry
        private static void CountLog(string content, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = ConsoleColor.White;
            eventCounter++;
            Console.Write($"#{eventCounter}\t ");
            Console.ForegroundColor = consoleColor;
            Console.Write(content + "\n");
        }

        // Returns the default DateTime and Log-Template String
        private static string LogString(string msg) => $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {msg}";

        // For printing the error message
        private static void PrintException(Exception e)
        {
            if(e != null)
            {
                CountLog(LogString($"Error: {e.Message}"), ConsoleColor.Red);
                Console.ForegroundColor = ConsoleColor.Red;
                if(e.StackTrace != null) Console.WriteLine($"Stacktrace: {e.StackTrace}");
                Console.WriteLine();
                PrintException(e.InnerException);
            }
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            // Thread.Sleep(1000);
            // StreamWriter sw = new StreamWriter("Test.txt");
            // sw.WriteLine($"Der Shid wurde am {DateTime.Now} geschlossen. AuuA");
            // sw.Close();
        }



    }
}
