using System.Collections;
using System.Collections.Generic;
using API;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeDetails : MonoBehaviour {

    public Text errorText;
    public InputField username;
    public InputField password;

    public void UpdateDetails() {
        StartCoroutine(UpdateDetailsCo());
    }

    private IEnumerator UpdateDetailsCo() {
        
        // get username
        string username = PlayerPrefs.GetString("username");
        
        // get user id
        UnityWebRequest getUserReq = APIShit.CreateRequest($"api/users?username={username}", APIShit.HttpVerb.Get);
        yield return getUserReq.SendWebRequest();
        string result = getUserReq.downloadHandler.text;
        
        // serialize result into a Dictionary<string, string>
        Dictionary<string, string> resultDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
        
        // get user id
        string userId = resultDict["guid"];

        bool changedUsername = false;
        bool changedPassword = false;
        
        // create patch data
        string data = "[";
        if (this.username.text != "") {
            changedUsername = true;
            data += $"{{\"op\":\"replace\",\"path\":\"/username\",\"value\":\"{this.username.text}\"}}";
        }
        if (password.text != "") {
            changedPassword = true;
            if (data != "[") {
                data += ",";
            }
            data += $"{{\"op\":\"replace\",\"path\":\"/password\",\"value\":\"{password.text}\"}}";
        }
        data += "]";

        if (data == "[]") {
            // they didn't change anything
            SceneManager.LoadScene("Settings");
            yield break;
        }

        UnityWebRequest patchUserReq = APIShit.CreateRequest($"api/users/{userId}", APIShit.HttpVerb.Patch, data);
        yield return patchUserReq.SendWebRequest();

        if (patchUserReq.responseCode != 200) {
            // fail
            FileLogging.Error("Failed to update user details");
            errorText.text = "Failed to update user details, try again later. (Error code: " + patchUserReq.responseCode + ")";
        }

        if (!changedPassword) {
            // I can't log them in because the password isn't stored
            SceneManager.LoadScene("Login");
            yield break;
        }

        string currentUsername = changedUsername ? this.username.text : PlayerPrefs.GetString("username");
        
        // then login because we need to update the session
        UnityWebRequest loginReq = APIShit.CreateRequest( 
            "auth", APIShit.HttpVerb.Get, "", APIShit.Auth.Basic, $"{currentUsername}:{password.text}");
        yield return loginReq.SendWebRequest();
        APIShit.Token = loginReq.downloadHandler.text;
        PlayerPrefs.SetString("token", APIShit.Token);
        PlayerPrefs.SetString("username", currentUsername);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Settings");
    }
    
}
