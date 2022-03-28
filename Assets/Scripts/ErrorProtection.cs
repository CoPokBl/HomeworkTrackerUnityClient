using System;
using System.Collections.Generic;
using UnityEngine;

public class ErrorProtection {
    
    private const int ErrorLimit = 3;
    private static readonly TimeSpan ErrorTime = new TimeSpan(0, 0, 30);
    private static readonly List<DateTime> Errors = new List<DateTime>();

    public static void ErrorLimitCheck() {
        FileLogging.Info("Checking error limit to prevent error loop");
        Errors.Add(DateTime.Now);
        
        // Remove errors older than 30 seconds
        while (Errors.Count > 0 && DateTime.Now - Errors[0] > ErrorTime) {
            Errors.RemoveAt(0);
        }
        
        // If errors is more than limit exit program
        if (Errors.Count <= ErrorLimit) return;
        FileLogging.Error("Too many errors occured in the last 30 seconds. Exiting program to prevent loops.");
        Application.Quit(1);
    }

}