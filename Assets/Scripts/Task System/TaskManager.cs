using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class TaskManager : MonoBehaviour
{
    private List<Task> tasks = new List<Task>();
    [SerializeField] private Toggle taskToogleUI;
    [SerializeField] private TextMeshProUGUI TaskListTitle;

}
