using System.Collections;
using System.Collections.Generic;
using API;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    public Text infoText;
    
    private bool _deleteAccountConfirmation;

    public void Logout() {
        // Just delete the token amd go back to the login screen, logging out is entirely handled by the client
        // There is no actual way to "log out"
        FileLogging.Info("Logging out...");
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.Save();
        FileLogging.Info("Logged out");
        SceneManager.LoadScene("Login");
    }

    public void DeleteAccount(Text label) {
        // Confirmation
        if (!_deleteAccountConfirmation) {
            _deleteAccountConfirmation = true;
            label.text = "Are you sure?";
            infoText.text = "This will delete your account and all of your data. This action cannot be undone.\n" +
                            "Press the button again to confirm.";
            return;
        }


        _deleteAccountConfirmation = false;
        StartCoroutine(DeleteAccountCo(label));
    }

    private IEnumerator DeleteAccountCo(Text label) {
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
        
        
        UnityWebRequest deleteUserReq = APIShit.CreateRequest($"api/users/{userId}", APIShit.HttpVerb.Delete);
        yield return deleteUserReq.SendWebRequest();

        label.text = "Delete Account";
        if (deleteUserReq.responseCode != 204) {  // No content response code
            FileLogging.Error("Error deleting user: " + deleteUserReq.responseCode);
            infoText.text = $"Error deleting account. Please try again later. (Error {deleteUserReq.responseCode})";
            yield break;
        }
        
        // success
        SceneManager.LoadScene("Login");
    }
    
}
