// HomeParentsSceneManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HomeParentsSceneManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button settingButtonParents;
    [SerializeField] private UnityEngine.UI.Button profileButtonParents;  // Added the missing semicolon here

    void Start()
    {
        // Setting button listener
        settingButtonParents.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SettingsScene");
        });

        // Profile button listener
        profileButtonParents.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("ProfileManagerParents");
        });
    }
}
