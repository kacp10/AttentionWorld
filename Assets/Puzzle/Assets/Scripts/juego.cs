using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class juego : MonoBehaviour
{
    public Sprite[] Niveles;
    public GameObject MenuGanar;
    public GameObject PiezaSeleccionada;
    public int PiezasEncajadas = 0;

    private float tiempoMaximo = 300f; // 5 minutos
    private float tiempoRestante;
    private int capa = 1;

    void Start()
    {
        tiempoRestante = tiempoMaximo;

        for (int i = 0; i < 36; i++)
        {
            GameObject.Find("Pieza (" + i + ")").transform.Find("Puzzle").GetComponent<SpriteRenderer>().sprite = Niveles[PlayerPrefs.GetInt("Nivel")];
        }
    }

    void Update()
    {
        tiempoRestante -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.transform != null && hit.transform.CompareTag("Puzzle"))
            {
                if (!hit.transform.GetComponent<pieza>().Encajada)
                {
                    PiezaSeleccionada = hit.transform.gameObject;
                    PiezaSeleccionada.GetComponent<pieza>().Seleccionada = true;
                    PiezaSeleccionada.GetComponent<SortingGroup>().sortingOrder = capa++;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && PiezaSeleccionada != null)
        {
            PiezaSeleccionada.GetComponent<pieza>().Seleccionada = false;
            PiezaSeleccionada = null;
        }

        if (PiezaSeleccionada != null)
        {
            Vector3 raton = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PiezaSeleccionada.transform.position = new Vector3(raton.x, raton.y, 0);
        }

        if (PiezasEncajadas == 36)
        {
            GuardarResultadoFinal(1000); // Puntaje máximo
        }
        else if (tiempoRestante <= 0)
        {
            int puntaje = Mathf.FloorToInt((PiezasEncajadas / 36f) * 1000f); // Puntaje proporcional
            GuardarResultadoFinal(puntaje);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) // salir anticipado
        {
            int puntaje = Mathf.FloorToInt((PiezasEncajadas / 36f) * 1000f);
            GuardarResultadoFinal(puntaje);
        }
    }

    void GuardarResultadoFinal(int score)
    {
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetString("CognitiveArea", "logica");
        PlayerPrefs.SetString("GameName", "Rompecabezas");
        SceneManager.LoadScene("ResultScene");
    }
}
