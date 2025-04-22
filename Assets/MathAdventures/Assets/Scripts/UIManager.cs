using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TMP_Text titleText;         // T�tulo del juego
    public GameObject ageSelectionPanel; // Panel donde se elige el rango de edad

    // M�todo para iniciar el juego con el rango de edad 6-7 a�os
    public void StartGameFor6To7Years()
    {
        ageSelectionPanel.SetActive(false);  // Desactiva el panel de selecci�n
        PlayerPrefs.SetInt("AgeRange", 0);   // Guarda el rango de edad 0 para 6-7 a�os
        SceneManager.LoadScene("GameSceneMath"); // Cargar la escena del juego
    }

    // M�todo para iniciar el juego con el rango de edad 8-10 a�os
    public void StartGameFor8To10Years()
    {
        ageSelectionPanel.SetActive(false);  // Desactiva el panel de selecci�n
        PlayerPrefs.SetInt("AgeRange", 1);   // Guarda el rango de edad 1 para 8-10 a�os
        SceneManager.LoadScene("GameSceneMath"); // Cargar la escena del juego
    }
}
