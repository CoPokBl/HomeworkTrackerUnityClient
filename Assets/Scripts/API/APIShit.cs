using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using API;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Color = System.Drawing.Color;

namespace HomeworkTrackerClient {
    public static class APIShit {
        public static string Token = "";
        public static readonly Version APIVer = new Version(0, 5, 0);

        public static string Url = "http://homeworktrack.serble.net:9898/";

        public enum Auth { None, Token, Basic }
        // create enum for http verbs
        public enum HttpVerb { GET, POST, PUT, DELETE }

        public static UnityWebRequest CreateRequest(string path, HttpVerb verb, Dictionary<string, string> dick, Auth auth = Auth.Token, string usrPwd = "") {
            string reqContent = JsonConvert.SerializeObject(dick);
            return CreateRequest(path, verb, reqContent, auth, usrPwd);
        }
        
        // create overload for CreateRequest that has a string instead of a dictionary
        public static UnityWebRequest CreateRequest(string path, HttpVerb verb, string dick = "", Auth auth = Auth.Token, string usrPwd = "") {
            
            // add debug message
            Debug.Log($"Creating {verb.ToString()} request for {path} with content {dick} and auth {auth.ToString()}");
            
            byte[] reqContent = Encoding.UTF8.GetBytes(dick);

            // create a switch for all the http verbs
            UnityWebRequest req;
            switch (verb) {
                case HttpVerb.GET:
                    req = UnityWebRequest.Get($"{Url}/{path}");
                    break;
                case HttpVerb.POST:
                    req = UnityWebRequest.Post($"{Url}/{path}", dick);
                    break;
                case HttpVerb.PUT:
                    req = UnityWebRequest.Put($"{Url}/{path}", dick);
                    break;
                case HttpVerb.DELETE:
                    req = UnityWebRequest.Delete($"{Url}/{path}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(verb), verb, null);
            }
            
            // set content type is uploadhandler is set
            if (req.uploadHandler != null) {
                req.uploadHandler.contentType = "application/json";
            }

            switch (auth) {
                // set header Authorization to auth type
                case Auth.Token:
                    req.SetRequestHeader("Authorization", "Bearer " + Token);
                    break;
                case Auth.Basic:
                    req.SetRequestHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(usrPwd)));
                    break;
            }
            req.SetRequestHeader("User-Agent", "Homework Tracker Unity Client By CoPokBl");
            req.timeout = 10;
            return req;
        }

        private static void SendSimpleRequest(Dictionary<string, string> dick, Action<AsyncOperation> callback) {
            string contentString = JsonConvert.SerializeObject(dick);
            FileLogging.Debug("Sending Request: " + contentString);

            // FINALLY
            UnityWebRequest req = UnityWebRequest.Put(Url, contentString);
            req.SetRequestHeader("x-api-token", Token);
            req.SetRequestHeader("User-Agent", "Homework Tracker Unity Client By CoPokBl");
            UnityWebRequestAsyncOperation asyncOperation = req.SendWebRequest();
            asyncOperation.completed += callback;
        }
        
        private static void SendRequest(Dictionary<string, string> dick, Action<AsyncOperation> callback) {
            dick.Add("version", APIVer.ToString());
            
            SendSimpleRequest(dick, callback);
        }
        
        public static void SendPingRequest(Action<AsyncOperation> callback) {
            Dictionary<string, string> dick = new Dictionary<string, string> { { "requestType", "ping" } };
            try {
                SendSimpleRequest(dick, callback);
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }
        
        public static void SendGetVerRequest(Action<AsyncOperation> callback) {
            Dictionary<string, string> dick = new Dictionary<string, string> { { "requestType", "getVersion" } };

            SendRequest(dick, callback);
        }

        public static void SendRegisterRequest(string username, string password, Action<AsyncOperation> callback) {
            Dictionary<string, string> dick = new Dictionary<string, string> { { "requestType", "register" }, {"username", username}, {"password", password} };

            SendRequest(dick, callback);
        }
        
        public static void SendCheckLoginRequest(Action<AsyncOperation> callback) {
            Dictionary<string, string> dick = new Dictionary<string, string> { { "requestType", "checkLogin" } };

            SendRequest(dick, callback);
        }
        
        public static void SendLoginRequest(string username, string password, Action<AsyncOperation> callback) {
            Dictionary<string, string> dick = new Dictionary<string, string> { {"requestType", "login"}, {"username", username}, {"password", password} };

            SendRequest(dick, callback);
        }
        
        public static List<TaskItem> GetTasksResultToList(string text) {
            List<Dictionary<string, string>> ucons = 
                JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(text);
            List<TaskItem> tasks = new List<TaskItem>();
        
            foreach (var ucon in ucons) {
                TaskItem item = new TaskItem(ucon["task"],
                    new ColouredString(ucon["class"], APIShit.ColorFromStr(ucon["classColour"])),
                    new ColouredString(ucon["type"], APIShit.ColorFromStr(ucon["typeColour"])),
                    ucon["id"]);
        
                if (ucon["dueDate"] != "0") {
                    item.dueDate = DateTime.FromBinary(long.Parse(ucon["dueDate"]));
                }
        
                tasks.Add(item);
            }

            return tasks;
        }
        
        // public static void SendSetTasksRequest(ColouredString classText, string task, ColouredString type, DateTime due, Action<AsyncOperation> callback) {
        //     Dictionary<string, string> dick = new Dictionary<string, string> {
        //         { "requestType", "addTask" },
        //         { "task", task },
        //         { "class", classText.Text },
        //         { "classColour", StrFromColor(classText.Color) },
        //         { "type", type.Text },
        //         { "typeColour", StrFromColor(type.Color) },
        //         { "dueDate", due.ToBinary().ToString() }
        //     };
        //
        //     SendRequest(dick, callback);
        // }
        
        public static void SendEditTaskRequest(string id, string field, string newValue, Action<AsyncOperation> callback) {
            Dictionary<string, string> dick = new Dictionary<string, string> {
                { "requestType", "editTask" },
                { "id", id },
                { "field", field },
                { "value", newValue }
            };

            SendRequest(dick, callback);
        }
        
        public static void SendDeleteTaskRequest(string id, Action<AsyncOperation> callback) {
            Dictionary<string, string> dick = new Dictionary<string, string> {
                { "requestType", "removeTask" },
                { "id", id }
            };

            SendRequest(dick, callback);
        }

        public static string Hash(string str) {
            StringBuilder builder = new StringBuilder();  
            foreach (byte t in SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(str))) {
                builder.Append(t.ToString("x2"));
            }

            return builder.ToString();
        }
        
        public static Color ColorFromStr(string str) {
            if (str == "-1.-1.-1") {
                return Color.Empty;
            }

            string[] strs = str.Split('.');
            return Color.FromArgb(Convert.ToInt32(strs[0]), Convert.ToInt32(strs[1]), Convert.ToInt32(strs[2]));
        }
        
        public static string StrFromColor(Color col) {
            return $"{col.R}.{col.G}.{col.B}";
        }
    }

    public struct Response {
        public string content;
        public int code;

        public Response(string c, HttpStatusCode c2) {
            content = c;
            code = (int)c2;
        }
    }
    
}
