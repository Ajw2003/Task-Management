using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    public List<TaskList> rootLists = new List<TaskList>();
    private string saveFilePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Application.persistentDataPath + "/tasks.json";
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Create new list
    public TaskList CreateList(string name, TaskList parentList = null)
    {
        TaskList newList = new TaskList(name);
        
        if (parentList == null)
        {
            rootLists.Add(newList);
        }
        else
        {
            parentList.sublists.Add(newList);
        }
        
        SaveData();
        return newList;
    }

    // Create new task
    public TaskItem CreateTask(string title, TaskList parentList, TaskItem parentTask = null)
    {
        TaskItem newTask = new TaskItem(title);
        
        if (parentTask == null)
        {
            parentList.tasks.Add(newTask);
        }
        else
        {
            parentTask.subtasks.Add(newTask);
        }
        
        SaveData();
        return newTask;
    }

    // Toggle task completion
    public void ToggleTaskCompletion(TaskItem task)
    {
        task.isCompleted = !task.isCompleted;
        SaveData();
    }

    // Delete task
    public void DeleteTask(TaskItem task, TaskList list, TaskItem parentTask = null)
    {
        if (parentTask == null)
        {
            list.tasks.Remove(task);
        }
        else
        {
            parentTask.subtasks.Remove(task);
        }
        SaveData();
    }

    // Delete list
    public void DeleteList(TaskList list, TaskList parentList = null)
    {
        if (parentList == null)
        {
            rootLists.Remove(list);
        }
        else
        {
            parentList.sublists.Remove(list);
        }
        SaveData();
    }

    // Save data to JSON
    public void SaveData()
    {
        TaskManagerData data = new TaskManagerData { lists = rootLists };
        string json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(saveFilePath, json);
    }

    // Load data from JSON
    public void LoadData()
    {
        if (System.IO.File.Exists(saveFilePath))
        {
            string json = System.IO.File.ReadAllText(saveFilePath);
            TaskManagerData data = JsonUtility.FromJson<TaskManagerData>(json);
            rootLists = data.lists ?? new List<TaskList>();
        }
    }
}

// Task data structure
[System.Serializable]
public class TaskItem
{
    public string id;
    public string title;
    public bool isCompleted;
    public List<TaskItem> subtasks;
    public DateTime createdDate;
    public DateTime? dueDate;

    public TaskItem(string title)
    {
        this.id = Guid.NewGuid().ToString();
        this.title = title;
        this.isCompleted = false;
        this.subtasks = new List<TaskItem>();
        this.createdDate = DateTime.Now;
    }
}

// Task list structure
[System.Serializable]
public class TaskList
{
    public string id;
    public string name;
    public List<TaskItem> tasks;
    public List<TaskList> sublists;

    public TaskList(string name)
    {
        this.id = Guid.NewGuid().ToString();
        this.name = name;
        this.tasks = new List<TaskItem>();
        this.sublists = new List<TaskList>();
    }
}

// Main task manager

[System.Serializable]
public class TaskManagerData
{
    public List<TaskList> lists;
}