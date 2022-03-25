using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using HomeworkTrackerClient;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    public InputField username;
    public InputField password;
    public Text outdatedText;
    public Text loginFailed;

    private void Start() {
        FileLogging.Debug("Started login process");
        StartCoroutine(StartFuncCo());
    }

    private IEnumerator StartFuncCo() {
        UnityWebRequest ping = APIShit.CreateRequest("api", APIShit.HttpVerb.GET, "", APIShit.Auth.None);
        yield return ping.SendWebRequest();
        if (ping.responseCode != 200) {
            // Server is down, oh no
            // Time to commit murder
            
            // log the error
            Debug.LogError(ping.error);
            
            FileLogging.Debug("Ping failed: " + ping.downloadHandler.text);
            SceneManager.LoadScene("ServerDown");
            yield break;
        }

        // Server is on
        // separate version from "Homework Tracker API, made by CoPokBl using ASP.NET. Version 1.1.1."
        string text = ping.downloadHandler.text;
        Debug.Log($"Ping returned {text}");
        string[] split = text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        Debug.Log(split[split.Length - 1]);
        string version = split[split.Length - 1].Remove(split[split.Length - 1].Length - 1);
        Version serverVersion = Version.Parse(version);
        if (APIShit.apiVer.Major < serverVersion.Major) {
            // This client is rly out of date and might not work
            outdatedText.enabled = true;
        }
        
        // Is there a saved login?
        if (!PlayerPrefs.HasKey("token")) {
            yield break; // Let them login
        }
        
        // They have logged in before
        APIShit.token = PlayerPrefs.GetString("token");
        UnityWebRequest loginReq = APIShit.CreateRequest("api/tasks", APIShit.HttpVerb.GET);
        yield return loginReq.SendWebRequest();
        if (loginReq.responseCode == 200) {
            // Login success
            SceneManager.LoadScene("GUI");
        }
        
        // Their login didn't work
    }

    public void LoginScreenFunc(int lr) {
        StartCoroutine(LoginScreenFuncCo(lr));
    }

    private IEnumerator LoginScreenFuncCo(int lr) {
        // Register is 1
        // Login is 0

        switch (lr) {
            case 0:
                // Login
                UnityWebRequest loginReq = APIShit.CreateRequest(
                    "auth", APIShit.HttpVerb.GET, "", APIShit.Auth.Basic, $"{username.text}:{password.text}");
                yield return loginReq.SendWebRequest();
                if (loginReq.responseCode != 200) {
                    // failed
                    loginFailed.enabled = true;
                    yield break;
                }
                APIShit.token = loginReq.downloadHandler.text;
                PlayerPrefs.SetString("token", APIShit.token);
                PlayerPrefs.Save();
                SceneManager.LoadScene("GUI");
                break;
            
            case 1:
                // Register
                UnityWebRequest regReq = APIShit.CreateRequest("api/users", APIShit.HttpVerb.POST, "", APIShit.Auth.Basic,
                    $"{username.text}:{password.text}");
                yield return regReq.SendWebRequest();
                if (regReq.responseCode != 201) {
                    // failed
                    loginFailed.enabled = true;
                    yield break;
                }
                // login
                loginReq = APIShit.CreateRequest(
                    "auth", APIShit.HttpVerb.GET, "", APIShit.Auth.Basic, $"{username.text}:{password.text}");
                yield return loginReq.SendWebRequest();
                APIShit.token = loginReq.downloadHandler.text;
                PlayerPrefs.SetString("token", APIShit.token);
                PlayerPrefs.Save();
                SceneManager.LoadScene("GUI");
                break;
        }
    }

}
