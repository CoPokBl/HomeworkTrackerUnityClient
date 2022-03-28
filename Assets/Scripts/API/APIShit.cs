using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace API {
    public static class APIShit {
        public static string Token = "";
        public static readonly Version APIVer = new Version(0, 8, 0);
        public static string Url = "http://homeworktrack.serble.net:9898/";

        public enum Auth { None, Token, Basic }
        public enum HttpVerb { Get, Post, Put, Delete, Patch }

        public static UnityWebRequest CreateRequest(string path, HttpVerb verb, Dictionary<string, string> dick,
            Auth auth = Auth.Token, string usrPwd = "") {
            string reqContent = JsonConvert.SerializeObject(dick); 
            return CreateRequest(path, verb, reqContent, auth, usrPwd);
        }
        
        // overload for CreateRequest that has a string instead of a dictionary
        public static UnityWebRequest CreateRequest(string path, HttpVerb verb, string dick = "", Auth auth = Auth.Token, string usrPwd = "") {
            
            // add debug message
            FileLogging.Info($"Creating {verb.ToString()} request for {path} with content {dick} and auth {auth.ToString()}");

            // create a switch for all the http verbs
            UnityWebRequest req;
            switch (verb) {
                case HttpVerb.Get:
                    req = UnityWebRequest.Get($"{Url}/{path}");
                    break;
                case HttpVerb.Post:
                    req = UnityWebRequest.Post($"{Url}/{path}", dick);
                    break;
                case HttpVerb.Put:
                    req = UnityWebRequest.Put($"{Url}/{path}", dick);
                    break;
                case HttpVerb.Delete:
                    req = UnityWebRequest.Delete($"{Url}/{path}");
                    break;
                case HttpVerb.Patch:
                    req = UnityWebRequest.Put($"{Url}/{path}", dick);
                    req.method = "PATCH";
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
                case Auth.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(auth), auth, null);
            }
            req.SetRequestHeader("User-Agent", "Homework Tracker Unity Client By CoPokBl");
            req.timeout = 10;
            return req;
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

        public static Color ColorFromStr(string str) {
            if (str == "-1.-1.-1") {
                return Color.Empty;
            }

            string[] strs = str.Split('.');
            return new Color(Convert.ToInt32(strs[0]), Convert.ToInt32(strs[1]), Convert.ToInt32(strs[2]));
        }
        
        public static string StrFromColor(Color col) => $"{col.R}.{col.G}.{col.B}";
        
        }

}
