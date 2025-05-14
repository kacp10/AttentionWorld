using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TeacherManager : MonoBehaviour
{
    [Header("Botones Teacher")]
    [SerializeField] private Button helpButtonTeacher;
    [SerializeField] private Button profileButtonTeacher;
    [SerializeField] private Button settingButtonTeacher;
    [SerializeField] private Button progressButtonTeacher;


    void Start()
    {
        // Ayuda
        if (helpButtonTeacher != null)
            helpButtonTeacher.onClick.AddListener(() =>
            {
                Debug.Log("[TeacherManager] Cargando HelpScene...");
                SceneManager.LoadScene("HelpScene");
            });

        // Perfil del profesor
        if (profileButtonTeacher != null)
            profileButtonTeacher.onClick.AddListener(() =>
            {
                Debug.Log("[TeacherManager] Cargando ProfileManagerTeacher...");
                SceneManager.LoadScene("ProfileManagerTeacher");
            });

        // Configuración
        if (settingButtonTeacher != null)
            settingButtonTeacher.onClick.AddListener(() =>
            {
                Debug.Log("[TeacherManager] Cargando SettingsScene...");
                SceneManager.LoadScene("SettingsScene");
            });

        // Progreso alumnos → lista de estudiantes
        if (progressButtonTeacher != null)
            progressButtonTeacher.onClick.AddListener(() =>
            {
                Debug.Log("[TeacherManager] Cargando TeacherStudentListScene...");
                SceneManager.LoadScene("TeacherStudentListScene");
            });

        // assignButtonTeacher queda para más adelante
    }
}
