using System;

namespace FinalProjectWPF.Projects.MyCalender.Models
{
    internal class MyTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime TaskStartTime { get; set; }
        public DateTime TaskEndTime { get; set; }

        public MyTask() { }
        public MyTask(int id, string title, string description, string location, DateTime taskStartTime, DateTime taskEndTime)
        {
            Id = id;
            Title = title;
            Description = description;
            Location = location;
            TaskStartTime = taskStartTime;
            TaskEndTime = taskEndTime;
        }
    }
    public class TaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string TaskTime { get; set; }
        public string TaskEndTime { get; set; }
    }
}

