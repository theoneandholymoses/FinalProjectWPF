namespace FinalProjectWPF.Projects.MyCalender.Models
{
    internal class MyTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime TaskTime { get; set; }
        public TimeSpan Duration { get; set; }

        public MyTask() { }
        public MyTask(int id, string title, string description, string location, DateTime taskTime, TimeSpan? duration = null)
        {
            Id = id;
            Title = title;
            Description = description;
            Location = location;
            TaskTime = taskTime;
            Duration = duration ?? TimeSpan.FromHours(1);
        }
    }
    public class TaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string TaskTime { get; set; }
        public string Duration { get; set; }
    }
}

