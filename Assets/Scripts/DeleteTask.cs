using System.Collections;
using HomeworkTrackerClient;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeleteTask : MonoBehaviour {
    public Text Id;
    
    public void Delete() {
        StartCoroutine(DeleteCo());
    }

    private IEnumerator DeleteCo() {
        UnityWebRequest deleteReq = APIShit.CreateRequest("api/tasks/" + Id.text, APIShit.HttpVerb.DELETE);
        yield return deleteReq.SendWebRequest();
        if (deleteReq.responseCode != 200) {
            // failed
            Debug.LogError("Failed to delete task");
        }
        SceneManager.LoadScene("GUI");
    }
}
