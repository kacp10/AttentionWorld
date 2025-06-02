using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GraficManager : MonoBehaviour
{
    public Button backButton;

    void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        else
        {
            Debug.LogWarning("Back Button no asignado en el inspector");
        }
    }

    void OnBackButtonClicked()
    {
        SceneManager.LoadScene("ProgressScene");
    }
}
