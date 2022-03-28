using System;
using System.IO;

public class FileLogging {
    private const int LoggingLevel = 3;
    public static string LogFile = "/home/copokbl/homeworktrackerunityclient.log";

    public static void Debug(string msg, Severity severity = Severity.Debug) {
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (home[0] == '/') {
            // linux
            LogFile = home + "/homeworktrackerunityclient.log";
        }
        else {
            // windows
            LogFile = home + "\\homeworktrackerunityclient.log";
        }
        if (LoggingLevel < (int) severity) return;
        if (!File.Exists(LogFile)) {
            FileStream a = File.Create(LogFile);
            a.Close();
        }
        string content = File.ReadAllText(LogFile);
        content += $"[{DateTime.Now}] [{severity}]: {msg}\n";
        File.WriteAllText(LogFile, content);
        switch (severity) {  // Use the right unity logging level
            case Severity.Debug:
                UnityEngine.Debug.Log($"[{DateTime.Now}] [{severity}]: {msg}");
                break;
            case Severity.Info:
                UnityEngine.Debug.Log($"[{DateTime.Now}] [{severity}]: {msg}");
                break;
            case Severity.Error:
                UnityEngine.Debug.LogError($"[{DateTime.Now}] [{severity}]: {msg}");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
        }
            
        System.Console.WriteLine($"[{DateTime.Now}] [{severity}]: {msg}");
        Console.Add($"[{severity}]: {msg}");
    }

    public static void Info(string msg) => Debug(msg, Severity.Info);
    public static void Error(string msg) => Debug(msg, Severity.Error);
    
        
    public enum Severity { Error, Info, Debug }
}