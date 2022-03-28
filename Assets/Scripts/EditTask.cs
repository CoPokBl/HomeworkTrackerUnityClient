using System;
using System.Collections;
using System.Collections.Generic;
using API;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = API.Color;

public class EditTask : MonoBehaviour {
    public InputField @class;
    public Dropdown   classColour;
    public InputField type;
    public Dropdown   typeColour;
    public InputField task;
    public Toggle     enableDueDate;
    public InputField dueDateD;
    public InputField dueDateM;
    public InputField dueDateY;

    private void Start() {
        try {
            StartCoroutine(StartCo());
        }
        catch (Exception e) {
            FileLogging.Error(e.ToString());
            // Go back to main menu, if the error was a server problem then it will filter back to saying disconnected
            SceneManager.LoadScene("GUI");
        }
    }

    private IEnumerator StartCo() {
        FileLogging.Info("Obtaining task data to edit");
        UnityWebRequest getTasksReq = APIShit.CreateRequest("api/tasks/" + Data.EditTaskId, APIShit.HttpVerb.Get);
        yield return getTasksReq.SendWebRequest();

        string result = getTasksReq.downloadHandler.text;
        FileLogging.Debug("Obtained task that they wanna edit: " + result);
        Dictionary<string, string> ucon = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
        TaskItem item = new TaskItem(ucon["task"],
            new ColouredString(ucon["class"], APIShit.ColorFromStr(ucon["classColour"])),  // CLASS COLOUR
            new ColouredString(ucon["type"], APIShit.ColorFromStr(ucon["typeColour"])),   // TYPE COLOUR
            ucon["id"]);
        
        // DUE DATE
        if (ucon["dueDate"] != "0") {
            item.dueDate = DateTime.FromBinary(long.Parse(ucon["dueDate"]));
        }

        // || Set dropdown values ||
        // CLASS COLOUR
        int classColourValue = ColorToIntDropdown(item.Class.Color);
        @class.text = item.Class.Text;
        classColour.value = classColourValue;
        classColour.RefreshShownValue();
        
        // TYPE COLOUR
        int typeColourValue = ColorToIntDropdown(item.Type.Color);
        type.text = item.Type.Text;
        typeColour.value = typeColourValue;
        typeColour.RefreshShownValue();
        
        // Just some nice debugging
        FileLogging.Debug("Class colour value: " + classColourValue);
        FileLogging.Debug("Type colour value: " + typeColourValue);
        
        // Set task
        task.text = item.Task;
        
        // Set due date
        enableDueDate.isOn = item.dueDate != null;
        if (item.dueDate != null) {
            dueDateD.text = item.dueDate.Day.ToString();
            dueDateM.text = item.dueDate.Month.ToString();
            dueDateY.text = item.dueDate.Year.ToString();
        }
    }
    
    
    public void Save() {
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
        
        // CREATE ITEM OBJECT FOR EASE OF ACCESS
        TaskItem item = new TaskItem {
            Class = new ColouredString(@class.text, cColour),  // CLASS
            Type = new ColouredString(type.text, tColour),     // TYPE
            Task = task.text,
            dueDate = due
        };

        // SEND ADD ITEM REQUEST TO SERVER, this is a put request so it will overwrite the old task
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
            FileLogging.Error("Failed to put item: " + addTaskReq.responseCode + addTaskReq.downloadHandler.text);
        }

        // Go back to the main menu
        SceneManager.LoadScene("GUI");
    }

    // Convert colour to int for the value for the colour dropdowns
    private int ColorToIntDropdown(Color color) {
        // black, blue, red, green, yellow, red, orange, purple, cyan, brown, lime, gold, grey, violet, deep pink
        FileLogging.Debug("Color RGB: " + color.R + ", " + color.G + ", " + color.B);
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