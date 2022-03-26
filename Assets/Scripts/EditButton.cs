using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditButton : MonoBehaviour {
    public Text Id;
    
    public void Edit() {
        Data.EditTaskId = Id.text;
        SceneManager.LoadScene("EditTask");
    }
    
}
