using System.Collections;
using System.Collections.Generic;
using HomeworkTrackerClient;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeleteTask : MonoBehaviour {
    public Text Id;
    
    public void Delete() {
        if (APIShit.SendDeleteTaskRequest(Id.text).code != 200) {
            // failed
            Debug.LogError("Failed to delete task");
        }
        SceneManager.LoadScene("GUI");
    }
}
