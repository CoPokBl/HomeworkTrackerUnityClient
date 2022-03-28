using System;
using System.Collections;
using System.Collections.Generic;
using API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = API.Color;

public class GUI : MonoBehaviour {
    public Text       noHomework;
    public GameObject taskPrefab;
    public GameObject viewContent;
    public Button     refreshButton;
    public Text       refreshButtonText;

    // This fixes issues with the tasks scroll position not starting at the top
    private bool      _hasSnapped;
    private bool      _hasFinishedLoadingTasks;

    public void Logout() {
        // Just delete the token amd go back to the login screen, logging out is entirely handled by the client
        // There is no actual way to "log out"
        FileLogging.Info("Logging out...");
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.Save();
        FileLogging.Info("Logged out");
        SceneManager.LoadScene("Login");
    }

    // This is all so that the scroll starts at the top when you refresh, its a bit of a hack but it works
    private void FixedUpdate() {
        if (_hasSnapped || !_hasFinishedLoadingTasks) return;
        Vector3 position = viewContent.transform.position;
        position = new Vector3(position.x, position.y - 10000, position.z);
        viewContent.transform.position = position;
        _hasSnapped = true;
        FileLogging.Info("Snapped view");
    }

    private void Start() => LoadTasks();  // Load everything
    private void LoadTasks() {  // Start to coroutine to load tasks
        try {
            StartCoroutine(LoadTasksCo());
        }
        catch (Exception e) {
            FileLogging.Error(e.ToString());
            ErrorProtection.ErrorLimitCheck();
            SceneManager.LoadScene("Login");  // Goto login screen just in case the server status has changed
        }
    }

    private IEnumerator LoadTasksCo() {
        FileLogging.Info("Loading tasks...");
        refreshButton.interactable = false;
        refreshButtonText.text = "Loading...";
        UnityWebRequest getTasksReq = APIShit.CreateRequest("api/tasks", APIShit.HttpVerb.Get);
        yield return getTasksReq.SendWebRequest();

        string result = getTasksReq.downloadHandler.text;
        FileLogging.Debug("Tasks Data Received: " + result);
        List<TaskItem> tasks;
        try {
            tasks = APIShit.GetTasksResultToList(result);
        }
        catch (Exception e) {
            FileLogging.Error(e.ToString());
            ErrorProtection.ErrorLimitCheck();
            SceneManager.LoadScene("Login");
            yield break;
        }

        if (tasks.Count == 0) {
            // no homework :)
            refreshButton.interactable = true;
            refreshButtonText.text = "Refresh";
            noHomework.enabled = true;
            yield break;
        }
        
        // rip there's homework
        Vector3 cPos = new Vector3(0f, 160f - 430f, 0f);
        foreach (TaskItem task in tasks) {
            FileLogging.Debug($"Adding {task.Task}...");
            GameObject obj = Instantiate(taskPrefab, viewContent.transform);
            obj.transform.position = cPos;
            GameObject[] children = new GameObject[obj.transform.childCount];
            for (int i = 0; i < obj.transform.childCount; i++) {
                children[i] = obj.transform.GetChild(i).gameObject;
            }
            foreach (GameObject child in children) {
                switch (child.name) {
                    
                    default:
                        FileLogging.Error("Child of task object '" + child.name + "' shouldn't exist! Is this client out of date?");
                        break;
                    
                    case "edit":
                    case "delete":
                    case "Back":
                        // I don't want it to display an error when it gets to these
                        break;
                    
                    case "Class":
                        Text classTxt = child.GetComponent<Text>();
                        classTxt.text = task.Class.Text;
                        Color cs = task.Class.Color;
                        classTxt.color = new UnityEngine.Color(cs.R, cs.G, cs.B, cs.A);
                        break;
                    
                    case "Type":
                        Text typeTxt = child.GetComponent<Text>();
                        typeTxt.text = task.Type.Text;
                        Color ts = task.Type.Color;
                        typeTxt.color = new UnityEngine.Color(ts.R, ts.G, ts.B, ts.A);
                        break;
                    
                    case "Task":
                        Text taskTxt = child.GetComponent<Text>();
                        taskTxt.text = task.Task;
                        break;
                    
                    case "DueDate":
                        Text dueTxt = child.GetComponent<Text>();
                        dueTxt.text = task.dueDate == DateTime.MaxValue 
                            ? "No Due Date" : $"Due: {task.dueDate.Year}/{task.dueDate.Month}/{task.dueDate.Day}";
                        if (DateTime.Now > task.dueDate) {
                            dueTxt.color = UnityEngine.Color.red;
                        }
                        else if (DateTime.Now.AddDays(5) > task.dueDate) {
                            dueTxt.color = UnityEngine.Color.yellow;
                        } else {
                            dueTxt.color = UnityEngine.Color.black;
                        }

                        break;
                    
                    case "ID":
                        Text idTxt = child.GetComponent<Text>();
                        idTxt.text = task.Id;
                        break;
                }
            }
            FileLogging.Debug($"Added {task.Task}");
        }

        if (tasks.Count != 0) {
            _hasFinishedLoadingTasks = true;
        }
        
        refreshButton.interactable = true;
        refreshButtonText.text = "Refresh";
        FileLogging.Debug("Finished loading tasks");
        
    }

    public void AddTask() {
        // All the login is handled in this scene
        SceneManager.LoadScene("AddTask");
    }

}
