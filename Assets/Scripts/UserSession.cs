using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance { get; private set; }
    private string loggedInUser = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("❌ UserSession ya existe, destruyendo esta instancia.");
            Destroy(gameObject);
            return;
        }

        Debug.Log("✅ Nueva instancia de UserSession creada.");
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



    // Método para guardar el usuario logueado
    public void SetLoggedInUser(string username)
    {
        if (!string.IsNullOrEmpty(username))
        {
            loggedInUser = username;
            Debug.Log($"✅ Usuario almacenado en UserSession: {loggedInUser}");
        }
        else
        {
            Debug.LogWarning("⚠️ El valor de username es nulo o vacío en SetLoggedInUser.");
        }
    }

    // Método para obtener el nombre del usuario logueado
    public string GetLoggedInUser()
    {
        Debug.Log("Usuario recuperado desde UserSession: " + loggedInUser);
        return loggedInUser;
    }

}
