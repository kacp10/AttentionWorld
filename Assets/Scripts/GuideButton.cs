// Assets/Scripts/GuideButton.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GuideButton : MonoBehaviour
{
    [Tooltip("Etiqueta que identifica el texto que se mostrará en GuideScene")]
    public string topicKey;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OpenGuide);
    }

    private void OpenGuide()
    {
        Debug.Log($"GuideButton clicked: {topicKey}");
        GuideData.SelectedTopic = topicKey;
        SceneManager.LoadScene("GuideScene");   // Usa exactamente el nombre que tiene tu escena
    }
}
