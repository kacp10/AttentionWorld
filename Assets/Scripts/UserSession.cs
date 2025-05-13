using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance { get; private set; }

    /* ─────────── Información de login ─────────── */
    private string loggedInUser = "";                  // ID que inició sesión
    private string currentRole = "";                  // "Child" | "Parent" | "Teacher"

    /* ─────────── Vínculo padre → hijo ─────────── */
    private string viewedChildId = null;               // Solo lo usa el padre

    /* ─────────── Singleton ─────────── */
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /* ============ Login ============ */
    public void SetLoggedInUser(string username, string role = "Child")
    {
        if (string.IsNullOrEmpty(username)) return;
        loggedInUser = username;
        currentRole = role;
        viewedChildId = null;               // por si venía de una sesión anterior
        Debug.Log($"[UserSession] Login: {loggedInUser}  (role: {currentRole})");
    }

    public string GetLoggedInUser() => loggedInUser;
    public string GetCurrentRole() => currentRole;

    /* ============ Padre viendo hijo ============ */
    public void SetViewedChild(string childId) => viewedChildId = childId;
    public void ClearViewedChild() => viewedChildId = null;
    public bool HasViewedChild() => !string.IsNullOrEmpty(viewedChildId);
    public string GetViewedChild() => viewedChildId;

    /* ============ NUEVO ============ */
    /// <summary>
    /// ID “activo” para todas las consultas de progreso.
    /// • Si el padre seleccionó un hijo → devuelve el hijo.
    /// • En cualquier otro caso → devuelve el usuario logeado.
    /// </summary>
    public string GetCurrentPlayerId()
    {
        return HasViewedChild() ? viewedChildId : loggedInUser;
    }
}
