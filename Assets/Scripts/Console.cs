using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.AssetImporters;
using UnityEngine;

public class Console : MonoBehaviour {
    private const float Height = 150f;
    private static string _text = "Console:\n";
    private Vector2 _scrollPosition = new Vector2(0,0);
    private bool _showConsole;
    private bool _hasKeyGoneUp = true;
    private DateTime _nextLogUpdate = DateTime.Now;
    private const int MaxLength = 48;

    private void OnGUI() {
        if (!_showConsole) { return; }
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(Screen.width),
            GUILayout.Height(Screen.height));
        GUILayout.TextArea(_text, GUILayout.MinHeight(Screen.height));
        GUILayout.EndScrollView();
    }

    private void Update() {
        
        // Update the log every second
        if (DateTime.Now > _nextLogUpdate && _showConsole) {
            _nextLogUpdate = DateTime.Now.AddSeconds(1);
            string[] log = File.ReadAllLines(FileLogging.LogFile);
            List<string> logList = new List<string>();
            logList.AddRange(log.Length > MaxLength ? log.Skip(log.Length - MaxLength) : log);
            string ttd = logList.Aggregate("Console:\n", (current, logItem) => current + (logItem + "\n"));
            _text = ttd;
        }
        
        if (Input.GetKeyUp(KeyCode.BackQuote)) { _hasKeyGoneUp = true; }
        if (!Input.GetKeyDown(KeyCode.BackQuote) || !_hasKeyGoneUp) return;
        _hasKeyGoneUp = false;
        _showConsole = !_showConsole;
        FileLogging.Info("Console toggled: " + _showConsole);
    }

    public static void Add(string line) {
        _text = _text + line + "\n";
    }
}