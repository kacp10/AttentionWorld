using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SettingsManager : MonoBehaviour
{
    public Button profileButton;
    public Button backButton;
    public Button saveButton;

    public Toggle notificationToggle;
    public Toggle musicToggle;

    public TMP_Text notificationFeedback;
    public TMP_Text musicFeedback;
    public TMP_Text saveFeedbackText;

    public Image notificationToggleBG;
    public Image musicToggleBG;

    private bool isInitialized = false;

    void Start()
    {
        // Cargar preferencias anteriores
        notificationToggle.isOn = PlayerPrefs.GetInt("NotificationsEnabled", 1) == 1;
        musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        // Agregar listeners
        notificationToggle.onValueChanged.AddListener(OnNotificationChanged);
        musicToggle.onValueChanged.AddListener(OnMusicChanged);
        saveButton.onClick.AddListener(SaveSettings);
        profileButton.onClick.AddListener(OpenProfileScene);
        backButton.onClick.AddListener(ReturnToHomeScene);

        // Ocultar feedback al iniciar
        notificationFeedback.gameObject.SetActive(false);
        musicFeedback.gameObject.SetActive(false);
        saveFeedbackText.gameObject.SetActive(false);

        // Activar cambios visuales después de setup
        isInitialized = true;
    }

    public void OnNotificationChanged(bool isOn)
    {
        if (!isInitialized) return;

        notificationFeedback.text = isOn ? "✔️ Notificaciones activadas al correo" : "❌ Notificaciones desactivadas";
        notificationFeedback.color = isOn ? new Color32(0, 130, 100, 255) : Color.red;
        notificationToggleBG.color = isOn ? new Color32(169, 255, 229, 255) : Color.white;

        StartCoroutine(HideFeedback(notificationFeedback));
    }

    public void OnMusicChanged(bool isOn)
    {
        if (!isInitialized) return;

        musicFeedback.text = isOn ? "✔️ Música activada" : "❌ Música desactivada";
        musicFeedback.color = isOn ? new Color32(0, 130, 100, 255) : Color.red;
        musicToggleBG.color = isOn ? new Color32(169, 255, 229, 255) : Color.white;

        StartCoroutine(HideFeedback(musicFeedback));
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt("NotificationsEnabled", notificationToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("MusicEnabled", musicToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();

        saveFeedbackText.text = "✔️ Cambios guardados";
        saveFeedbackText.color = new Color32(0, 130, 100, 255);
        StartCoroutine(HideFeedback(saveFeedbackText));
    }

    IEnumerator HideFeedback(TMP_Text feedback)
    {
        feedback.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        feedback.gameObject.SetActive(false);
    }

    void OpenProfileScene()
    {
        SceneManager.LoadScene("MyProfileScene");
    }

    void ReturnToHomeScene()
    {
        SceneManager.LoadScene("HomeChildScene");
    }
}
