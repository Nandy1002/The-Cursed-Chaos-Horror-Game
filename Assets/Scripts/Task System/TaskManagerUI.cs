using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskManagerUI : MonoBehaviour
{
    [SerializeField] private Toggle taskToogleUI;
    [SerializeField] private TextMeshProUGUI TaskListTitle;

    private TaskManager taskManager;

    public void Awake(){
        taskToogleUI.gameObject.SetActive(false);
        
    }
    public void Start(){
        taskManager = new TaskManager();
        taskManager.AddTask(new Task("Drink 2 juice", Task.Availability.Available));
        taskManager.AddTask(new Task("Go to toilet", Task.Availability.Available));
        taskManager.AddTask(new Task("Pick up the call", Task.Availability.Available));
        taskManager.AddTask(new Task("Return to work", Task.Availability.Available));
        Debug.Log(taskManager.TasksCount());
        RefreshTaskItems();
    }

    private void RefreshTaskItems(){
        int y = 0;
        foreach(Task task in taskManager.GetTasks()){
            Toggle newToggle = Instantiate(taskToogleUI, taskToogleUI.transform.parent);
            newToggle.isOn = task.availability == Task.Availability.Completed;
            newToggle.GetComponentInChildren<Text>().text = task.taskName;
            newToggle.transform.localPosition = taskToogleUI.transform.localPosition + new Vector3(0, y, 0);
            y -= 30;
            newToggle.gameObject.SetActive(true);
        }
    }
}
