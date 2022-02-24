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
        if (!PlayerPrefs.HasKey("username")) {
            return; // Let them login
        }
        
        // They have logged in before
        APIShit.SetUsernameAndPasswordHash(PlayerPrefs.GetString("username"), PlayerPrefs.GetString("password"));
        Response res = APIShit.SendCheckLoginRequest();
        if (res.code == 0) {
            // Login success
            SceneManager.LoadScene("GUI");
        }
        
        // Their login didn't work
    }

    private void Update() {
        
    }

    public void LoginScreenFunc(int lr) {
        // Register is 1
        // Login is 0
        PlayerPrefs.SetString("username", username.text);
        PlayerPrefs.SetString("password", APIShit.Hash(password.text));
        PlayerPrefs.Save();
        
        APIShit.SetUsernameAndPassword(username.text, password.text);

        switch (lr) {
            case 0:
                // Login
                Response lReq = APIShit.SendCheckLoginRequest();
                if (lReq.code == 200) {
                    // Login success
                    SceneManager.LoadScene("GUI");
                }
                // Login failed
                loginFailed.enabled = true;
                break;
            
            case 1:
                // Register
                Response res = APIShit.SendRegisterRequest();
                if (res.code != 200) {
                    // It failed
                    loginFailed.enabled = true;
                    return;
                }
                // It worked
                SceneManager.LoadScene("GUI");
                break;
        }
        
    }

}
