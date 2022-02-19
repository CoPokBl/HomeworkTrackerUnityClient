using System;
using System.Collections;
using System.Collections.Generic;
using HomeworkTrackerClient;
using HomeworkTrackerServer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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
                return;
            }
        }
        
        TaskItem item = new TaskItem {
            Class = new ColouredString(@class.text, cColour),
            Type = new ColouredString(type.text, tColour),
            Task = task.text,
            dueDate = due
        };

        if (APIShit.SendSetTasksRequest(item.Class, item.Task, item.Type, due).code != 200) {
            // error
            Debug.LogError("Failed to add item");
        }

        SceneManager.LoadScene("GUI");
    }

}
