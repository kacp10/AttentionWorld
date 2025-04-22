using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TMP_Text titleText;         // Título del juego
    public GameObject ageSelectionPanel; // Panel donde se elige el rango de edad

    // Método para iniciar el juego con el rango de edad 6-7 años
    public void StartGameFor6To7Years()
    {
        ageSelectionPanel.SetActive(false);  // Desactiva el panel de selección
        PlayerPrefs.SetInt("AgeRange", 0);   // Guarda el rango de edad 0 para 6-7 años
        SceneManager.LoadScene("GameSceneMath"); // Cargar la escena del juego
    }

    // Método para iniciar el juego con el rango de edad 8-10 años
    public void StartGameFor8To10Years()
    {
        ageSelectionPanel.SetActive(false);  // Desactiva el panel de selección
        PlayerPrefs.SetInt("AgeRange", 1);   // Guarda el rango de edad 1 para 8-10 años
        SceneManager.LoadScene("GameSceneMath"); // Cargar la escena del juego
    }
}
