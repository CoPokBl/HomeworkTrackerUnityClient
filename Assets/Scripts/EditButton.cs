using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EditButton : MonoBehaviour {
    [FormerlySerializedAs("Id")] public Text id;
    
    // Tet the variable to tell the edit task script what to edit and then load that scene
    public void Edit() {
        Data.EditTaskId = id.text;
        SceneManager.LoadScene("EditTask");
    }
    
}
