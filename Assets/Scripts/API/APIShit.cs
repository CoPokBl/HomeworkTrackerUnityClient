using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HomeworkTrackerServer;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;
using Color = System.Drawing.Color;

namespace HomeworkTrackerClient {
    public class APIShit {
        public static string token = "";
        public static Version apiVer = new Version(0, 5, 0);
        
        public static string Url = "http://homeworktrack.serble.net:9898/api";

        public static UnityWebRequest CreateRequest(string type, Dictionary<string, string> dick) {
            dick.Add("requestType", type);
            UnityWebRequest req = UnityWebRequest.Post(Url, JsonConvert.SerializeObject(dick));
            req.SetRequestHeader("x-api-token", token);
            req.SetRequestHeader("User-Agent", "Homework Tracker Unity Client By CoPokBl");
            return req;
        }

        private static void SendSimpleRequest(Dictionary<string, string> dick, Action<AsyncOperation> callback) {
            string contentString = JsonConvert.SerializeObject(dick);
            FileLogging.Debug("Sending Request: " + contentString);

            // FINALLY
            UnityWebRequest req = UnityWebRequest.Post(Url, contentString);
            req.SetRequestHeader("x-api-token", token);
            req.SetRequestHeader("User-Agent", "Homework Tracker Unity Client By CoPokBl");
            UnityWebRequestAsyncOperation asyncOperation = req.SendWebRequest();
            asyncOperation.completed += callback;
            
            // Response normalRes = new Response {
            //     code = (int) req.responseCode,
            //     content = req.downloadHandler.text
            // };
            //
            // FileLogging.Info("Request Result:");
            // FileLogging.Info("Code: " + normalRes.code);
            // FileLogging.Info("Content: " + normalRes.content);
        }
        
        private static void SendRequest(Dictionary<string, string> dick, Action<AsyncOperation> callback) {
            dick.Add("version", apiVer.ToString());
            
            SendSimpleRequest(dick, callback);
        }
        
        public static void SendPingRequest(Action<AsyncOperation> callback) {
            Dictionary<string, string> dick = new Dictionary<string, string> { { "requestType", "ping" } };
            Response res;
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
        
        public static void SendGetTasksRequest(Action<AsyncOperation> callback) {
            Dictionary<string, string> dick = new Dictionary<string, string> { { "requestType", "getTasks" } };

            SendRequest(dick, callback);
            // List<Dictionary<string, string>> ucons = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(tasksResponse.content);
            // List<TaskItem> outp = new List<TaskItem>();
            //
            // foreach (var ucon in ucons) {
            //     TaskItem item = new TaskItem(ucon["task"],
            //         new ColouredString(ucon["class"], ColorFromStr(ucon["classColour"])),
            //         new ColouredString(ucon["type"], ColorFromStr(ucon["typeColour"])),
            //         ucon["id"]);
            //
            //     if (ucon["dueDate"] != "0") {
            //         item.dueDate = DateTime.FromBinary(long.Parse(ucon["dueDate"]));
            //     }
            //
            //     outp.Add(item);
            // }
            //
            // return outp;
        }
        
        public static void SendSetTasksRequest(ColouredString classText, string task, ColouredString type, DateTime due, Action<AsyncOperation> callback) {
            Dictionary<string, string> dick = new Dictionary<string, string> {
                { "requestType", "addTask" },
                { "task", task },
                { "class", classText.Text },
                { "classColour", StrFromColor(classText.Color) },
                { "type", type.Text },
                { "typeColour", StrFromColor(type.Color) },
                { "dueDate", due.ToBinary().ToString() }
            };

            SendRequest(dick, callback);
        }
        
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
            return Color.FromArgb(255, Convert.ToInt32(strs[0]), Convert.ToInt32(strs[1]), Convert.ToInt32(strs[2]));
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
