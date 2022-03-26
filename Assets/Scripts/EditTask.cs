using System;
using System.Collections;
using System.Collections.Generic;
using API;
using HomeworkTrackerClient;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = System.Drawing.Color;
using UColor = UnityEngine.Color;

public class EditTask : MonoBehaviour {
    public InputField @class;
    public Dropdown classColour;
    public InputField type;
    public Dropdown typeColour;
    public InputField task;
    public Toggle enableDueDate;
    public InputField dueDateD;
    public InputField dueDateM;
    public InputField dueDateY;

    private void Start() {
        StartCoroutine(StartCo());
    }

    private IEnumerator StartCo() {
        UnityWebRequest getTasksReq = APIShit.CreateRequest("api/tasks/" + Data.EditTaskId, APIShit.HttpVerb.GET);
        yield return getTasksReq.SendWebRequest();

        string result = getTasksReq.downloadHandler.text;
        FileLogging.Debug("Obtained task that they wanna edit: " + result);
        Dictionary<string, string> ucon = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
        TaskItem item = new TaskItem(ucon["task"],
            new ColouredString(ucon["class"], APIShit.ColorFromStr(ucon["classColour"])),
            new ColouredString(ucon["type"], APIShit.ColorFromStr(ucon["typeColour"])),
            ucon["id"]);
        
        if (ucon["dueDate"] != "0") {
            item.dueDate = DateTime.FromBinary(long.Parse(ucon["dueDate"]));
        }

        @class.text = item.Class.Text;
        classColour.value = ColorToIntDropdown(item.Class.Color);
        
        type.text = item.Type.Text;
        typeColour.value = ColorToIntDropdown(item.Type.Color);
        
        task.text = item.Task;
        
        enableDueDate.isOn = item.dueDate != null;
        if (item.dueDate != null) {
            dueDateD.text = item.dueDate.Day.ToString();
            dueDateM.text = item.dueDate.Month.ToString();
            dueDateY.text = item.dueDate.Year.ToString();
        }
    }
    
    
    public void Save() {
        StartCoroutine(AddTaskFuncCo());
    }

    private IEnumerator AddTaskFuncCo() {
        Color c = Color.Aqua;
        
        
        Color cColour = Color.FromName(classColour.options[classColour.value].text.ToLower());
        if (classColour.value == 0) {
            cColour = System.Drawing.Color.Empty;
        }
        System.Drawing.Color tColour = System.Drawing.Color.FromName(typeColour.options[typeColour.value].text.ToLower());
        if (typeColour.value == 0) {
            tColour = System.Drawing.Color.Empty;
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
            { "id", Data.EditTaskId },
            
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
    

    public UColor ToUnityColor(Color color) {
        return new UColor(color.R, color.G, color.B);
    }


    // public int ColorToIntDropdown(UColor color) {
    //     return ColorToIntDropdown(ToUnityColor(color));
    // }

    public int ColorToIntDropdown(Color color) {
        // black, blue, red, green, yellow, red, orange, purple, cyan, brown, lime, gold, grey, violet, deep pink
        if (color == Color.Black) {
            return 0;
        }
        if (color == Color.Blue) {
            return 1;
        }
        if (color == Color.Red) {
            return 2;
        }
        if (color == Color.Green) {
            return 3;
        }
        if (color == Color.Yellow) {
            return 4;
        }
        if (color == Color.Red) {
            return 5;
        }
        if (color == Color.Orange) {
            return 6;
        }
        if (color == Color.Purple) {
            return 7;
        }
        if (color == Color.Cyan) {
            return 8;
        }
        if (color == Color.Brown) {
            return 9;
        }
        if (color == Color.Lime) {
            return 10;
        }
        if (color == Color.Gold) {
            return 11;
        }
        if (color == Color.Gray) {
            return 12;
        }
        if (color == Color.Violet) {
            return 13;
        }
        if (color == Color.DeepPink) {
            return 14;
        }

        return 0;
    }
    
}