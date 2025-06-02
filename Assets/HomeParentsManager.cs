// HomeParentsSceneManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HomeParentsSceneManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button settingButtonParents;
    [SerializeField] private UnityEngine.UI.Button profileButtonParents;
    [SerializeField] private UnityEngine.UI.Button helpButtonParents;
    [SerializeField] private UnityEngine.UI.Button logoutButtonParents;

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
        helpButtonParents.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("helpScene");
        });
        logoutButtonParents.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("LoginScene");
        });
    }
}
