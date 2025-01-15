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
    public GameObject objective;

    public Task(string taskName, Availability availability)
    {
        this.taskName = taskName;
        this.availability = availability;
    }
}
