using System.Collections;
using API;
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
        UnityWebRequest deleteReq = APIShit.CreateRequest("api/tasks/" + Id.text, APIShit.HttpVerb.Delete);
        yield return deleteReq.SendWebRequest();
        if (deleteReq.responseCode != 204) {
            // failed
            FileLogging.Error("Failed to delete task: " + deleteReq.responseCode);
            yield break;
        }
        SceneManager.LoadScene("GUI");
    }
}
