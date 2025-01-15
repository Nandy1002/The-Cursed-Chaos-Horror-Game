using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class TaskManager
{
    private List<Task> tasks;
    
        
    
    public TaskManager()
    {
        tasks = new List<Task>();
    }
    public void AddTasks(List<Task> tasks)
    {
        this.tasks.AddRange(tasks);
    }
    public void AddTask(Task task)
    {
        tasks.Add(task);
    }
    public void TaskCompleted(Task task)
    {
        tasks.Remove(task);
    }
    public List<Task> GetTasks()
    {
        return tasks;
    }
    public int TasksCount()
    {
        return tasks.Count;
    }

}
