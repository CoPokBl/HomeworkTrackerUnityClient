using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HomeworkTrackerServer;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Color = System.Drawing.Color;

namespace HomeworkTrackerClient {
    public class APIShit {
        private static string username;
        private static string password;
        public static Version apiVer = new Version(0, 4, 0);
        
        public static string Url = "http://homeworktrack.serble.net:9898/api";

        public static void SetUsernameAndPassword(string username, string password) {
            APIShit.username = username;
            APIShit.password = Hash(password);
        }
        
        public static void SetUsernameAndPasswordHash(string username, string password) {
            APIShit.username = username;
            APIShit.password = password;
        }

        private static Response SendSimpleRequest(Dictionary<string, string> dick) {
            string contentString = JsonConvert.SerializeObject(dick);
            NativeArray<char> result = new NativeArray<char>(8192, Allocator.TempJob);
            NativeArray<char> contentData = new NativeArray<char>(contentString.Length, Allocator.TempJob);
            for (int i = 0; i < contentString.Length; i++) {
                contentData[i] = contentString[i];
            }
            SimpleRequestJob jobData = new SimpleRequestJob {
                contentCa = contentData,
                result = result
            };

            // Schedule the job
            JobHandle handle = jobData.Schedule();

            // Wait for the job to complete
            handle.Complete();

            // All copies of the NativeArray point to the same memory, you can access the result in "your" copy of the NativeArray
            char[] codeChars = new char[3];
            for (int i = 0; i < 3; i++) {
                codeChars[i] = result[i];
            }
            char[] responseChars = new char[result.Length - 3];
            int j = 0;

            for (int i = 3; i < result.Length-2; i++) {
                responseChars[j] = result[i];
                j++;
            }
            StringBuilder codeStrBuild = new StringBuilder();
            foreach (char codeStrChar in codeChars) {
                codeStrBuild.Append(codeStrChar);
            }
            StringBuilder contentStrBuild = new StringBuilder();
            foreach (char contentStrChar in responseChars) {
                contentStrBuild.Append(contentStrChar);
            }
            int code = int.Parse(codeStrBuild.ToString());
            Response normalRes = new Response {
                code = code,
                content = contentStrBuild.ToString()
            };
            
            Debug.Log("Code: " + normalRes.code);
            Debug.Log("Content: " + normalRes.content);
            

            // Free the memory allocated by the result array
            result.Dispose();
            contentData.Dispose();
            
            // Return the result
            return normalRes;
        }
        
        private static Response SendRequest(Dictionary<string, string> dick) {
            dick.Add("username", username);
            dick.Add("password", password);
            dick.Add("version", apiVer.ToString());
            
            return SendSimpleRequest(dick);
        }
        
        public static bool SendPingRequest() {
            Dictionary<string, string> dick = new Dictionary<string, string> { { "requestType", "ping" } };
            Response res;
            try {
                res = SendSimpleRequest(dick);
            } catch (Exception e) {
                Debug.LogError(e);
                return false;
            }
            
            return res.code == 200;
        }
        
        public static Version SendGetVerRequest() {
            Dictionary<string, string> dick = new Dictionary<string, string> { { "requestType", "getVersion" } };

            return Version.Parse(SendRequest(dick).content);
        }

        public static Response SendRegisterRequest() {
            Dictionary<string, string> dick = new Dictionary<string, string> { { "requestType", "register" } };

            return SendRequest(dick);
        }
        
        public static Response SendCheckLoginRequest() {
            Dictionary<string, string> dick = new Dictionary<string, string> { { "requestType", "checkLogin" } };

            return SendRequest(dick);
        }
        
        public static List<TaskItem> SendGetTasksRequest() {
            Dictionary<string, string> dick = new Dictionary<string, string> { { "requestType", "getTasks" } };

            Response tasksResponse = SendRequest(dick);
            List<Dictionary<string, string>> ucons = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(tasksResponse.content);
            List<TaskItem> outp = new List<TaskItem>();

            foreach (var ucon in ucons) {
                TaskItem item = new TaskItem(ucon["task"],
                    new ColouredString(ucon["class"], ColorFromStr(ucon["classColour"])),
                    new ColouredString(ucon["type"], ColorFromStr(ucon["typeColour"])),
                    ucon["id"]);

                if (ucon["dueDate"] != "0") {
                    item.dueDate = DateTime.FromBinary(long.Parse(ucon["dueDate"]));
                }

                outp.Add(item);
            }

            return outp;
        }
        
        public static Response SendSetTasksRequest(ColouredString classText, string task, ColouredString type, DateTime due) {
            Dictionary<string, string> dick = new Dictionary<string, string> {
                { "requestType", "addTask" },
                { "task", task },
                { "class", classText.Text },
                { "classColour", StrFromColor(classText.Color) },
                { "type", type.Text },
                { "typeColour", StrFromColor(type.Color) },
                { "dueDate", due.ToBinary().ToString() }
            };

            return SendRequest(dick);
        }
        
        public static Response SendEditTaskRequest(string id, string field, string newValue) {
            Dictionary<string, string> dick = new Dictionary<string, string> {
                { "requestType", "editTask" },
                { "id", id },
                { "field", field },
                { "value", newValue }
            };

            return SendRequest(dick);
        }
        
        public static Response SendDeleteTaskRequest(string id) {
            Dictionary<string, string> dick = new Dictionary<string, string> {
                { "requestType", "removeTask" },
                { "id", id }
            };

            return SendRequest(dick);
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
    
    public struct ResponseCA {
        public char[] content;
        public int code;
    }
    
    public struct SimpleRequestJob : IJob {
        public NativeArray<char> result;
        public NativeArray<char> contentCa;

        public void Execute() {
            StringBuilder str = new StringBuilder();
            foreach (char reqChar in contentCa) {
                str.Append(reqChar);
            }
            StringContent data = new StringContent(str.ToString(), Encoding.UTF8, "text/html");
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.PostAsync(APIShit.Url, data).Result;
            string hresult = response.Content.ReadAsStringAsync().Result;

            char[] codeChars = ((int)response.StatusCode).ToString().ToCharArray();
            for (int i = 0; i < 3; i++) {
                result[i] = codeChars[i];
            }
            char[] responseChars = hresult.ToCharArray();
            int j = 0;
            for (int i = 3; i < hresult.Length+3; i++) {
                result[i] = responseChars[j];
                j++;
            }
        }
    }
}
