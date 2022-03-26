using System;
using System.Collections;
using System.Collections.Generic;
using API;
using HomeworkTrackerClient;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = System.Drawing.Color;

public class AddTask : MonoBehaviour {
    public InputField @class;
    public Dropdown classColour;
    public InputField type;
    public Dropdown typeColour;
    public InputField task;
    public Toggle enableDueDate;
    public InputField dueDateD;
    public InputField dueDateM;
    public InputField dueDateY;

    public void AddTaskFunc() {
        StartCoroutine(AddTaskFuncCo());
    }

    private IEnumerator AddTaskFuncCo() {
        Color cColour = Color.FromName(classColour.options[classColour.value].text.ToLower());
        if (classColour.value == 0) {
            cColour = Color.Empty;
        }
        Color tColour = Color.FromName(typeColour.options[typeColour.value].text.ToLower());
        if (typeColour.value == 0) {
            tColour = Color.Empty;
        }

        DateTime due = DateTime.MaxValue;
        if (enableDueDate.isOn) {
            // do due date
            try {
                due = new DateTime(int.Parse(dueDateY.text), int.Parse(dueDateM.text), int.Parse(dueDateD.text));
            }
            catch (Exception) {
                // invalid
                DateTime now = DateTime.Now;
                dueDateY.text = now.Year.ToString();
                dueDateM.text = now.Month.ToString();
                dueDateD.text = now.Day.ToString();
                yield break;
            }
        }
        
        TaskItem item = new TaskItem {
            Class = new ColouredString(@class.text, cColour),
            Type = new ColouredString(type.text, tColour),
            Task = task.text,
            dueDate = due
        };

        UnityWebRequest addTaskReq = APIShit.CreateRequest("api/tasks", APIShit.HttpVerb.PUT, new Dictionary<string, string> {
            { "class", item.Class.Text },
            { "classColour", APIShit.StrFromColor(item.Class.Color) },
            
            { "type", item.Type.Text },
            { "typeColour", APIShit.StrFromColor(item.Type.Color) },
            
            { "task", item.Task },
            
            { "dueDate", item.dueDate.ToBinary().ToString() },
        });
        yield return addTaskReq.SendWebRequest();
        if (addTaskReq.responseCode != 201) {
            // error
            Debug.LogError("Failed to add item: " + addTaskReq.responseCode + addTaskReq.downloadHandler.text);
        }

        SceneManager.LoadScene("GUI");
    }

}
