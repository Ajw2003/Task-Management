using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TaskUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform listContainer;
    public Transform taskContainer;
    public GameObject listItemPrefab;
    public GameObject taskItemPrefab;
    public TMP_InputField newListInput;
    public TMP_InputField newTaskInput;
    public TextMeshProUGUI currentListTitle;
    public Button backButton;
    public Button addListButton;
    public Button addTaskButton;

    private Stack<TaskList> navigationStack = new Stack<TaskList>();
    private TaskList currentList;

    void Start()
    {
        addListButton.onClick.AddListener(CreateNewList);
        addTaskButton.onClick.AddListener(CreateNewTask);
        backButton.onClick.AddListener(NavigateBack);
        
        RefreshListView();
    }

    void CreateNewList()
    {
        if (string.IsNullOrWhiteSpace(newListInput.text)) return;
        
        TaskManager.Instance.CreateList(newListInput.text, currentList);
        newListInput.text = "";
        
        if (currentList == null)
            RefreshListView();
        else
            RefreshCurrentListView();
    }

    void CreateNewTask()
    {
        if (currentList == null || string.IsNullOrWhiteSpace(newTaskInput.text)) return;
        
        TaskManager.Instance.CreateTask(newTaskInput.text, currentList);
        newTaskInput.text = "";
        RefreshCurrentListView();
    }

    public void RefreshListView()
    {
        ClearContainer(listContainer);
        currentList = null;
        currentListTitle.text = "My Lists";
        backButton.gameObject.SetActive(false);
        newTaskInput.gameObject.SetActive(false);
        addTaskButton.gameObject.SetActive(false);

        foreach (TaskList list in TaskManager.Instance.rootLists)
        {
            CreateListUI(list);
        }
    }

    void RefreshCurrentListView()
    {
        ClearContainer(listContainer);
        ClearContainer(taskContainer);
        
        currentListTitle.text = currentList.name;
        backButton.gameObject.SetActive(true);
        newTaskInput.gameObject.SetActive(true);
        addTaskButton.gameObject.SetActive(true);

        // Show sublists
        foreach (TaskList sublist in currentList.sublists)
        {
            CreateListUI(sublist);
        }

        // Show tasks
        foreach (TaskItem task in currentList.tasks)
        {
            CreateTaskUI(task, taskContainer, currentList, null);
        }
    }

    void CreateListUI(TaskList list)
    {
        GameObject listObj = Instantiate(listItemPrefab, listContainer);
        
        TextMeshProUGUI titleText = listObj.GetComponentInChildren<TextMeshProUGUI>();
        titleText.text = list.name;

        Button openButton = listObj.GetComponent<Button>();
        openButton.onClick.AddListener(() => OpenList(list));

        Button deleteButton = listObj.transform.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(() => DeleteList(list));
    }

    void CreateTaskUI(TaskItem task, Transform parent, TaskList list, TaskItem parentTask, int depth = 0)
    {
        GameObject taskObj = Instantiate(taskItemPrefab, parent);
        
        // Add indentation based on depth
        RectTransform rect = taskObj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(depth * 30, rect.anchoredPosition.y);

        TextMeshProUGUI titleText = taskObj.GetComponentInChildren<TextMeshProUGUI>();
        titleText.text = task.title;
        
        if (task.isCompleted)
        {
            titleText.fontStyle = FontStyles.Strikethrough;
            titleText.color = Color.gray;
        }

        Toggle checkbox = taskObj.GetComponentInChildren<Toggle>();
        checkbox.isOn = task.isCompleted;
        checkbox.onValueChanged.AddListener((value) => ToggleTask(task));

        Button deleteButton = taskObj.transform.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(() => DeleteTask(task, list, parentTask));

        Button addSubtaskButton = taskObj.transform.Find("AddSubtaskButton").GetComponent<Button>();
        addSubtaskButton.onClick.AddListener(() => AddSubtask(task, list));

        // Recursively create subtasks
        foreach (TaskItem subtask in task.subtasks)
        {
            CreateTaskUI(subtask, parent, list, task, depth + 1);
        }
    }

    void OpenList(TaskList list)
    {
        if (currentList != null)
        {
            navigationStack.Push(currentList);
        }
        
        currentList = list;
        RefreshCurrentListView();
    }

    void NavigateBack()
    {
        if (navigationStack.Count > 0)
        {
            currentList = navigationStack.Pop();
            RefreshCurrentListView();
        }
        else
        {
            RefreshListView();
        }
    }

    void ToggleTask(TaskItem task)
    {
        TaskManager.Instance.ToggleTaskCompletion(task);
        RefreshCurrentListView();
    }

    void DeleteTask(TaskItem task, TaskList list, TaskItem parentTask)
    {
        TaskManager.Instance.DeleteTask(task, list, parentTask);
        RefreshCurrentListView();
    }

    void DeleteList(TaskList list)
    {
        TaskList parent = (navigationStack.Count > 0) ? navigationStack.Peek() : null;
        TaskManager.Instance.DeleteList(list, parent);
        
        if (currentList == null)
            RefreshListView();
        else
            RefreshCurrentListView();
    }

    void AddSubtask(TaskItem parentTask, TaskList list)
    {
        // You could open an input dialog here
        // For simplicity, creating a default subtask
        TaskManager.Instance.CreateTask("New Subtask", list, parentTask);
        RefreshCurrentListView();
    }

    void ClearContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}