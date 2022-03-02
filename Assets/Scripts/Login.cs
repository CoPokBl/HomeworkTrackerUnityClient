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
        UnityWebRequest ping = APIShit.CreateRequest("ping", new Dictionary<string, string>());
        yield return ping.SendWebRequest();
        if (ping.result != UnityWebRequest.Result.Success) {
            // Server is down, oh no
            // Time to commit murder
            SceneManager.LoadScene("ServerDown");
            yield return null;
        }
        
        // Server is on
        UnityWebRequest verReq = APIShit.CreateRequest("getVersion", new Dictionary<string, string>());
        yield return verReq.SendWebRequest();
        Version serverVersion = Version.Parse(verReq.downloadHandler.text);
        if (APIShit.apiVer.Major < serverVersion.Major) {
            // This client is rly out of date and might not work
            outdatedText.enabled = true;
        }
        
        // Is there a saved login?
        if (!PlayerPrefs.HasKey("token")) {
            yield return null; // Let them login
        }
        
        // They have logged in before
        APIShit.token = PlayerPrefs.GetString("token");
        UnityWebRequest loginReq = APIShit.CreateRequest("checkLogin", new Dictionary<string, string>() { });
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
                UnityWebRequest loginReq = APIShit.CreateRequest("login", new Dictionary<string, string>() {
                    { "username", username.text },
                    { "password", password.text }
                });
                yield return loginReq.SendWebRequest();
                if (loginReq.responseCode != 200) {
                    // failed
                    loginFailed.enabled = true;
                    yield return null;
                }
                APIShit.token = loginReq.downloadHandler.text;
                PlayerPrefs.SetString("token", APIShit.token);
                PlayerPrefs.Save();
                SceneManager.LoadScene("GUI");
                break;
            
            case 1:
                // Register
                UnityWebRequest regReq = APIShit.CreateRequest("login", new Dictionary<string, string>() {
                    { "username", username.text },
                    { "password", password.text }
                });
                yield return regReq.SendWebRequest();
                if (regReq.responseCode != 200) {
                    // failed
                    loginFailed.enabled = true;
                    yield return null;
                }
                UnityWebRequest login2Req = APIShit.CreateRequest("login", new Dictionary<string, string>() {
                    { "username", username.text },
                    { "password", password.text }
                });
                yield return login2Req.SendWebRequest();
                APIShit.token = login2Req.downloadHandler.text;
                PlayerPrefs.SetString("token", APIShit.token);
                PlayerPrefs.Save();
                SceneManager.LoadScene("GUI");
                break;
        }
    }

}
