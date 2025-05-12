using System.Collections.Generic;
using UnityEngine;

public class GameAssignment
{
    public string SceneName;
    public string DisplayName;
    public string Area;
    public string Instruction;
}

public class GameSessionData : MonoBehaviour
{
    public static GameSessionData Instance { get; private set; }

    public List<GameAssignment> Assignments { get; private set; } = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetAssignments(List<GameAssignment> assignments)
    {
        Assignments = assignments;
    }

    public GameAssignment GetCurrentAssignment()
    {
        int index = PlayerPrefs.GetInt("CurrentGameIndex", 0);
        return (index >= 0 && index < Assignments.Count) ? Assignments[index] : null;
    }
}
