// HomeParentsSceneManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HomeParentsSceneManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button settingButtonParents;

    void Start()
    {
        settingButtonParents.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SettingsScene");
        });
    }
}

