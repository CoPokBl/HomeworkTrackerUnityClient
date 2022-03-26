using System;
using System.IO;

public class FileLogging {
    private const int LoggingLevel = 3;
    private static string _logFile = "/home/copokbl/homeworktrackerunityclient.log";
    public static void Debug(string msg, Severity severity = Severity.Debug) {
        if (LoggingLevel >= (int) severity) {
                
            ConsoleColor textColour = severity switch {
                Severity.Error => ConsoleColor.Red,
                Severity.Info => ConsoleColor.Green,
                _ => ConsoleColor.White
            };
            if (!File.Exists(_logFile)) {
                FileStream a = File.Create(_logFile);
                a.Close();
            }
            string ccontent = File.ReadAllText(_logFile);
            ccontent += $"[{DateTime.Now}] [{severity}]: {msg}\n";
            File.WriteAllText(_logFile, ccontent);
            UnityEngine.Debug.Log($"[{DateTime.Now}] [{severity}]: {msg}");
            Console.WriteLine($"[{DateTime.Now}] [{severity}]: {msg}");
        }
    }

    public static void Info(string msg) { Debug(msg, Severity.Info); }
    public static void Error(string msg) { Debug(msg, Severity.Error); }
        
    public enum Severity {
        Error,
        Info,
        Debug
    }
}