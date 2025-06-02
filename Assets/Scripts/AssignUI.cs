using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AssignUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public ScrollRect availableScroll;
    public ScrollRect selectedScroll;
    public GameObject gameRowPrefab;
    public TMP_Text categoryTitle;

    public string CurrentStudentId { get; private set; } = "";
    private Transform selectedRow;
    private string currentCategory = "Atención";

    private readonly Dictionary<string, string[]> gamesByCategory = new()
    {
        { "Atención", new[]{ "BallScene", "StarScene", "MoleScene" } },
        { "Memoria",    new[]{ "PairsScene" } },
        { "Lógica",     new[]{ "PuzzleScene" } },
        { "Cálculo",      new[]{ "GameSceneMath" } }
    };

    private void Start()
    {
        ShowAttention();
    }

    public void ShowAttention() => ShowAvailable("Atención");
    public void ShowMemory() => ShowAvailable("Memoria");
    public void ShowLogic() => ShowAvailable("Lógica");
    public void ShowMath() => ShowAvailable("Cálculo");

    private void ShowAvailable(string category)
    {
        currentCategory = category;
        if (categoryTitle) categoryTitle.text = $"Juegos {category}";

        Clear(availableScroll.content.transform);

        foreach (var g in gamesByCategory[category])
        {
            InstantiateRow(g, availableScroll.content.transform);
        }
    }

    public void SetCurrentStudent(StudentRowMeta meta)
    {
        CurrentStudentId = meta.playerId;
        Clear(selectedScroll.content.transform);
    }

    private void Clear(Transform parent)
    {
        foreach (Transform c in parent)
            Destroy(c.gameObject);
    }

    private void InstantiateRow(string game, Transform parent)
    {
        var row = Instantiate(gameRowPrefab, parent).transform;
        row.name = game;

        var text = row.GetComponentInChildren<TMP_Text>();
        if (text) text.text = game;

        var button = row.GetComponent<Button>();
        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => RowClicked(row));
        }
    }

    public void RowClicked(Transform row)
    {
        if (selectedRow)
            selectedRow.GetComponent<Image>().color = Color.white;

        selectedRow = row;
        selectedRow.GetComponent<Image>().color = new Color(0.8f, 0.9f, 1f);

        Debug.Log($"✔ Juego seleccionado: {selectedRow.name}");
    }

    public void AddSelected()
    {
        if (!selectedRow || selectedRow.parent != availableScroll.content.transform)
        {
            Debug.LogWarning("⚠️ No hay juego seleccionado en disponibles para añadir.");
            return;
        }

        // ✅ SOLUCIÓN CORRECTA: clonar nuevo objeto
        InstantiateRow(selectedRow.name, selectedScroll.content.transform);

        Destroy(selectedRow.gameObject);
        selectedRow = null;
    }

    public void RemoveSelected()
    {
        if (!selectedRow || selectedRow.parent != selectedScroll.content.transform)
        {
            Debug.LogWarning("⚠️ No hay juego seleccionado en seleccionados para quitar.");
            return;
        }

        Destroy(selectedRow.gameObject);
        selectedRow = null;
    }

    public List<string> GetSelectedGames()
    {
        var list = new List<string>();
        foreach (Transform t in selectedScroll.content.transform)
        {
         
            list.Add(t.name);
        }
        return list;
    }
}
