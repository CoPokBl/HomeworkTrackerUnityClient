using System;
using System.Collections.Generic;
using HomeworkTrackerClient;
using HomeworkTrackerServer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GUI : MonoBehaviour {
    public ScrollRect viewport;
    public Text noHomework;
    public GameObject taskPrefab;
    public int distanceBetweenTasks;
    [FormerlySerializedAs("maxTasksOnScreen")] public int maxScrollUp;
    public int maxScrollDown;
    public GameObject viewContent;

    public void Logout() {
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("password");
        SceneManager.LoadScene("Login");
    }

    private void Start() {
        // Load everything
        LoadTasks();
    }

    public void LoadTasks() {
        List<TaskItem> tasks = APIShit.SendGetTasksRequest();

        if (tasks.Count == 0) {
            // no homework :)
            noHomework.enabled = true;
            return;
        }
        
        // rip there's homework
        Vector3 cPos = new Vector3(0f - 270f, 160f - 430f, 0f);
        foreach (TaskItem task in tasks) {
            GameObject obj = Instantiate(taskPrefab, viewContent.transform);
            obj.transform.position = cPos;
            GameObject[] children = new GameObject[obj.transform.childCount];
            for (int i = 0; i < obj.transform.childCount; i++) {
                children[i] = obj.transform.GetChild(i).gameObject;
            }
            foreach (GameObject child in children) {
                switch (child.name) {
                    
                    default:
                        Debug.LogError("Child of task object '" + child.name + "' shouldn't exist");
                        break;
                    
                    case "delete":
                    case "Back":
                        // I don't want it to display an error when it gets this
                        break;
                    
                    case "Class":
                        Text classTxt = child.GetComponent<Text>();
                        classTxt.text = task.Class.Text;
                        System.Drawing.Color cs = task.Class.Color;
                        classTxt.color = new Color(cs.R, cs.G, cs.B, cs.A);
                        break;
                    
                    case "Type":
                        Text typeTxt = child.GetComponent<Text>();
                        typeTxt.text = task.Type.Text;
                        System.Drawing.Color ts = task.Type.Color;
                        typeTxt.color = new Color(ts.R, ts.G, ts.B, ts.A);
                        break;
                    
                    case "Task":
                        Text taskTxt = child.GetComponent<Text>();
                        taskTxt.text = task.Task;
                        break;
                    
                    case "DueDate":
                        Text dueTxt = child.GetComponent<Text>();
                        dueTxt.text = task.dueDate == DateTime.MaxValue 
                            ? "No Due Date" : $"Due: {task.dueDate.Year}/{task.dueDate.Month}/{task.dueDate.Day}";
                        break;
                    
                    case "ID":
                        Text idTxt = child.GetComponent<Text>();
                        idTxt.text = task.Id;
                        break;
                }
            }
            cPos.y -= distanceBetweenTasks;
        }
    }

    public void UpdatedViewport() {
        Debug.Log(viewport.verticalNormalizedPosition);
        if (viewport.verticalNormalizedPosition == 1f) {
            Canvas.ForceUpdateCanvases();
            viewport.verticalNormalizedPosition = 0.5f;
            Canvas.ForceUpdateCanvases();
        }
        // if (viewport.verticalNormalizedPosition < maxScrollDown) {
        //     viewport.verticalNormalizedPosition = 0f;
        // }
    }

    public void AddTask() {
        // Load Scene
        SceneManager.LoadScene("AddTask");
    }

}
