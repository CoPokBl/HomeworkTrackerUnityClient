using System;
using System.Collections;
using System.Collections.Generic;
using API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = API.Color;

public class AddTask : MonoBehaviour {
    public InputField @class;
    public Dropdown   classColour;
    public InputField type;
    public Dropdown   typeColour;
    public InputField task;
    public Toggle     enableDueDate;
    public InputField dueDateD;
    public InputField dueDateM;
    public InputField dueDateY;

    public void AddTaskFunc() {
        try {
            StartCoroutine(AddTaskFuncCo());
        }
        catch (Exception e) {
            FileLogging.Error(e.ToString());
        }
    }

    private IEnumerator AddTaskFuncCo() {
        
        // COLOURS
        Color cColour = Color.FromName(classColour.options[classColour.value].text.ToLower());
        if (classColour.value == 0) {
            cColour = Color.Empty;
        }
        Color tColour = Color.FromName(typeColour.options[typeColour.value].text.ToLower());
        if (typeColour.value == 0) {
            tColour = Color.Empty;
        }

        // DUE DATE
        DateTime due = DateTime.MaxValue;
        if (enableDueDate.isOn) {
            // do due date
            try {
                due = new DateTime(int.Parse(dueDateY.text), int.Parse(dueDateM.text), int.Parse(dueDateD.text)).ToUniversalTime();
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
        
        // CREATE ITEM OBJECT FOR EASE OF ACCESS
        TaskItem item = new TaskItem {
            Class = new ColouredString(@class.text, cColour),  // CLASS
            Type = new ColouredString(type.text, tColour),     // TYPE
            Task = task.text,
            dueDate = due
        };

        // SEND ADD ITEM REQUEST
        UnityWebRequest addTaskReq = APIShit.CreateRequest("api/tasks", APIShit.HttpVerb.Put, new Dictionary<string, string> {
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
            FileLogging.Error("Failed to add item: " + addTaskReq.responseCode + addTaskReq.downloadHandler.text);
        }

        // Go back to the main menu
        SceneManager.LoadScene("GUI");
    }

}
