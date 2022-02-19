using System;

namespace HomeworkTrackerServer {
    public class TaskItem {
        public string Task;
        public ColouredString Class;
        public ColouredString Type;
        public string Id;
        public DateTime dueDate;

        public TaskItem(string text, ColouredString cls, ColouredString type, string id, DateTime due) {
            Task = text;
            Class = cls;
            Type = type;
            Id = id;
            dueDate = due;
        }
        
        public TaskItem(string text, ColouredString cls, ColouredString type, string id) {
            Task = text;
            Class = cls;
            Type = type;
            Id = id;
            dueDate = DateTime.MaxValue;
        }

        public TaskItem() {
            
        }
        
    }
}