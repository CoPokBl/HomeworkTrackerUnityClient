using System;
using System.Collections;
using HomeworkTrackerClient;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    public InputField username;
    public InputField password;
    public InputField ip;
    public Text errorText;
    public Text loginFailed;

    public Button loginButton;
    public Button registerButton;
    public Button connectButton;
    
    public Text loginButtonText;
    public Text registerButtonText;
    public Text connectButtonText;

    private void Start() {
        loginButton.interactable = false;
        registerButton.interactable = false;
        FileLogging.Debug("Started login process");
        StartCoroutine(StartFuncCo());
    }

    private IEnumerator StartFuncCo() {

        ip.text = PlayerPrefs.HasKey("ip") ? PlayerPrefs.GetString("ip") : "http://homeworktrack.serble.net:9898";
        APIShit.Url = ip.text;
        
        UnityWebRequest ping = APIShit.CreateRequest("api", APIShit.HttpVerb.GET, "", APIShit.Auth.None);
        yield return ping.SendWebRequest();
        if (ping.responseCode != 200) {
            // Server is down, oh no
            // Time to commit murder
            
            // log the error
            FileLogging.Error(ping.error);
            
            FileLogging.Debug("Ping failed: " + ping.downloadHandler.text);
            
            // change error text
            errorText.text = "Server is down, please try again later or choose a different server";
            errorText.enabled = true;
            
            // grey out login and register buttons
            loginButton.interactable = false;
            registerButton.interactable = false;
            
            // make ip text red
            ip.textComponent.color = Color.red;

            yield break;
        }
        
        loginButton.interactable = true;
        registerButton.interactable = true;
        ip.textComponent.color = Color.green;

        // Server is on
        // separate version from "Homework Tracker API, made by CoPokBl using ASP.NET. Version 1.1.1."
        string text = ping.downloadHandler.text;
        FileLogging.Info($"Ping returned {text}");
        string[] split = text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        FileLogging.Info(split[split.Length - 1]);
        string version = split[split.Length - 1].Remove(split[split.Length - 1].Length - 1);
        Version serverVersion = Version.Parse(version);
        if (APIShit.APIVer.Major < serverVersion.Major) {
            // This client is rly out of date and might not work
            errorText.text = "Warning: Outdated Client";
            ip.textComponent.color = Color.yellow;
            errorText.enabled = true;
        }
        
        // Is there a saved login?
        if (!PlayerPrefs.HasKey("token")) {
            yield break; // Let them login
        }
        
        // They have logged in before
        APIShit.Token = PlayerPrefs.GetString("token");
        UnityWebRequest loginReq = APIShit.CreateRequest("api/tasks", APIShit.HttpVerb.GET);
        yield return loginReq.SendWebRequest();
        if (loginReq.responseCode == 200) {
            // Login success
            SceneManager.LoadScene("GUI");
        }
        
        // Their login didn't work
    }

    public void ConnectPress() {
        StartCoroutine(ConnectFuncCo());
    }
    
    private IEnumerator ConnectFuncCo() {
        
        connectButtonText.text = "Connecting...";
        connectButton.interactable = false;
        
        // set ip
        if (ip.text != "") {
            APIShit.Url = ip.text;
        } else {
            yield break;
        }
        
        PlayerPrefs.SetString("ip", ip.text);
        PlayerPrefs.Save();

        UnityWebRequest ping = APIShit.CreateRequest("api", APIShit.HttpVerb.GET, "", APIShit.Auth.None);
        yield return ping.SendWebRequest();
        connectButtonText.text = "Connect";
        connectButton.interactable = true;
        if (ping.responseCode != 200) {
            // Server is down, oh no
            // Time to commit murder
            
            // log the error
            FileLogging.Error(ping.error);

            FileLogging.Debug("Ping failed: " + ping.downloadHandler.text);
            
            // change error text
            errorText.text = "Server is down, please try again later or choose a different server";
            errorText.enabled = true;
            
            // grey out login and register buttons
            loginButton.interactable = false;
            registerButton.interactable = false;
            
            // make ip text red
            ip.textComponent.color = Color.red;
            
            yield break;
        }
        
        // change error text
        errorText.enabled = false;
            
        // ungrey out login and register buttons
        loginButton.interactable = true;
        registerButton.interactable = true;
            
        // make ip text red
        ip.textComponent.color = Color.green;
        
        // separate version from "Homework Tracker API, made by CoPokBl using ASP.NET. Version 1.1.1."
        string text = ping.downloadHandler.text;
        FileLogging.Info($"Ping returned {text}");
        string[] split = text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        FileLogging.Info(split[split.Length - 1]);
        string version = split[split.Length - 1].Remove(split[split.Length - 1].Length - 1);
        Version serverVersion = Version.Parse(version);
        if (APIShit.APIVer.Major < serverVersion.Major) {
            // This client is rly out of date and might not work
            errorText.text = "Warning: Outdated Client";
            ip.textComponent.color = Color.yellow;
            errorText.enabled = true;
        }

        // Server is on
        
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
                    errorText.text = "Invalid login/password";
                    errorText.enabled = true;
                    yield break;
                }
                APIShit.Token = loginReq.downloadHandler.text;
                PlayerPrefs.SetString("token", APIShit.Token);
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
                    errorText.text = "Register failed: Username taken";
                    errorText.enabled = true;
                    yield break;
                }
                // login
                loginReq = APIShit.CreateRequest(
                    "auth", APIShit.HttpVerb.GET, "", APIShit.Auth.Basic, $"{username.text}:{password.text}");
                yield return loginReq.SendWebRequest();
                APIShit.Token = loginReq.downloadHandler.text;
                PlayerPrefs.SetString("token", APIShit.Token);
                PlayerPrefs.Save();
                SceneManager.LoadScene("GUI");
                break;
        }
    }

}
