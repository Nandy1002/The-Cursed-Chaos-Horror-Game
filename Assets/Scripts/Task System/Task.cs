using UnityEngine;

[System.Serializable]
public class Task
{
    public enum Availability
    {
        Available,
        Unavailable,
        Completed
    }
    public Availability availability;
    public string taskName;
    public int id;
    public GameObject objective;

    public Task(string taskName, int id, Availability availability)
    {
        this.taskName = taskName;
        this.id = id;
        this.availability = availability;
    }
    public int getTaskId()
    {
        return id;
    }
}
