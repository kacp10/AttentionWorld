using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class helpManager : MonoBehaviour
{
    public Button backButton;

    void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBackHomeChild);
        }
        else
        {
            Debug.LogWarning("backButton no está asignado en el inspector.");
        }
    }

    void GoBackHomeChild()
    {
        SceneManager.LoadScene("HomeChildScene");
    }
}
