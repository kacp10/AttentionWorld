using UnityEngine;
using System.Collections.Generic;

public class ConstellationGenerator : MonoBehaviour
{
    public static ConstellationGenerator Instance;

    public GameObject starPrefab;
    private GameObject constellationParent;
    private int currentStarCount;
    private float currentRotationSpeed = 20f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GenerateRandom()
    {
        if (constellationParent != null)
            Destroy(constellationParent);

        currentStarCount = Random.Range(3, 7);
        constellationParent = new GameObject("Constellation");
        constellationParent.transform.position = Vector3.zero;

        float radius = 3f;
        List<Vector3> starPositions = new List<Vector3>();

        for (int i = 0; i < currentStarCount; i++)
        {
            float angle = 2 * Mathf.PI / currentStarCount * i;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            GameObject star = Instantiate(starPrefab, pos, Quaternion.identity, constellationParent.transform);
            starPositions.Add(pos);
        }

        // Crear LineRenderer
        LineRenderer line = constellationParent.AddComponent<LineRenderer>();
        line.positionCount = currentStarCount + 1;
        line.widthMultiplier = 0.02f;  // Hacer la línea más delgada
        line.useWorldSpace = false;

        // Ajustar el grosor aún más fino
        line.startWidth = 0.02f;  // Hacer la línea más delgada
        line.endWidth = 0.02f;    // Hacer la línea más delgada




        for (int i = 0; i < currentStarCount; i++)
            line.SetPosition(i, starPositions[i]);

        // Cierra la figura (último punto igual al primero)
        line.SetPosition(currentStarCount, starPositions[0]);

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.yellow;
        line.endColor = Color.yellow;

        // Añadir rotación
        Rotator rotator = constellationParent.AddComponent<Rotator>();
        rotator.speed = currentRotationSpeed;
    }

    public int GetCurrentStarCount()
    {
        return currentStarCount;
    }

    public void IncreaseRotationSpeed()
    {
        currentRotationSpeed += 10f;
    }
}
