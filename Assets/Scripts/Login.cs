using System;
using System.Collections;
using System.Net;
using HomeworkTrackerClient;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    public InputField username;
    public InputField password;
    public Text outdatedText;
    public Text loginFailed;

    private void Start() {
        bool isAlive = APIShit.SendPingRequest();
        if (!isAlive) {
            // Server is down, oh no
            // Time to commit murder
            SceneManager.LoadScene("ServerDown");
        }
        
        // Server is on
        Version serverVersion = APIShit.SendGetVerRequest();
        if (APIShit.apiVer.Major < serverVersion.Major) {
            // This client is rly out of date and might not work
            outdatedText.enabled = true;
        }
        
        // Is there a saved login?
        if (!PlayerPrefs.HasKey("token")) {
            return; // Let them login
        }
        
        // They have logged in before
        APIShit.token = PlayerPrefs.GetString("token");
        Response res = APIShit.SendCheckLoginRequest();
        if (res.code == 200) {
            // Login success
            SceneManager.LoadScene("GUI");
        }
        
        // Their login didn't work
    }

    public void LoginScreenFunc(int lr) {
        // Register is 1
        // Login is 0

        switch (lr) {
            case 0:
                // Login
                Response lRes = APIShit.SendLoginRequest(username.text, password.text);
                if (lRes.code != 200) {
                    // failed
                    loginFailed.enabled = true;
                    return;
                }
                APIShit.token = lRes.content;
                PlayerPrefs.SetString("token", lRes.content);
                PlayerPrefs.Save();
                SceneManager.LoadScene("GUI");
                break;
            
            case 1:
                // Register
                Response res = APIShit.SendRegisterRequest(username.text, password.text);
                if (res.code != 200) {
                    // It failed
                    loginFailed.enabled = true;
                    return;
                }
                lRes = APIShit.SendLoginRequest(username.text, password.text);
                APIShit.token = lRes.content;
                PlayerPrefs.SetString("token", lRes.content);
                PlayerPrefs.Save();
                // It worked
                SceneManager.LoadScene("GUI");
                break;
        }
        
    }

}
