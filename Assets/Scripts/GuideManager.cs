// Assets/Scripts/GuideManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;   // ← para cambiar de escena
using TMPro;

public class GuideManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;

    /*───────────────  CARGA DEL TEXTO  ───────────────*/
    private void Start()
    {
        string key = GuideData.SelectedTopic;
        titleText.text = GetTitleFor(key);
        bodyText.text = GetBodyFor(key);
    }

    /*───────────────  TÍTULOS Y CUERPOS  ─────────────*/
    private string GetTitleFor(string k)
    {
        return k switch
        {
            "about" => "Acerca del Juego",
            "daily" => "Ejercicios Diarios",
            "stats" => "Estadísticas",
            "profile" => "Perfil",
            "single" => "Ejercicio Único",
            "messages" => "Mensajes",
            _ => "Guía"
        };
    }

    private string GetBodyFor(string k)
    {
        switch (k)
        {
            /*──────────────────  ACERCA DEL JUEGO  ──────────────────*/
            case "about":
                return
    @"¿Qué es?
Este prototipo fue diseñado como proyecto de grado para apoyar a niños con Trastorno por Déficit de Atención (TDA) en el aula y en casa.

Áreas cognitivas trabajadas
1. Atención  
2. Memoria  
3. Lógica  
4. Cálculo  

Rol del maestro y los padres
- Docente: asigna los minijuegos y revisa el avance global del curso.  
- Padres: pueden consultar, desde la app, el progreso individual de sus hijos.";

            /*──────────────────  EJERCICIOS DIARIOS  ─────────────────*/
            case "daily":
                return
    @"Aquí aparecen los 4 minijuegos asignados para hoy.
1. Toca Iniciar para ver una breve descripción.  
2. Completa el reto dentro del límite de tiempo.  
3. Repite hasta terminar los cuatro.

> Consejo docente: usa la pestaña de asignación diaria para seleccionar
> qué áreas cognitivas entrenará tu clase mañana.";

            /*──────────────────  ESTADÍSTICAS  ──────────────────────*/
            case "stats":
                return
    @"En esta pantalla verás:
- Gráfico de barras: puntos obtenidos por área cognitiva.  
- Historial: evolución de aciertos / fallos de los últimos 7 días.  
- Progreso diario: porcentaje de minijuegos completados hoy.

Los datos provienen de la tabla GameResults de DynamoDB.";

            /*──────────────────  PERFIL  ────────────────────────────*/
            case "profile":
                return
    @"Muestra la información del jugador activo:
• Nombre del usuario.  
• Medallas desbloqueadas (descargadas desde Amazon S3) rol (niño).  
• Últimas 4 actividades.

(En versiones futuras aparecerá la foto de perfil).";

            /*──────────────────  EJERCICIO ÚNICO  ───────────────────*/
            case "single":
                return
    @"Modo de práctica rápida:
1. Elige un minijuego específico.  
2. Juega sin afectar las estadísticas diarias.  
3. Ideal para reforzar un área cognitiva antes de un test.";

            /*──────────────────  MENSAJES  ──────────────────────────*/
            case "messages":
                return
    @"Aquí recibirás:
- Alertas del sistema (nuevas versiones, mantenimiento).  
- Comentarios del docente sobre tu desempeño.  
- Recordatorios para los padres (firmar tareas, etc.).";

            /*──────────────────  DEFAULT  ──────────────────────────*/
            default:
                return "Tema no encontrado.";
        }
    }


    /*───────────────  BOTÓN VOLVER  ───────────────*/
    /// <summary>
    /// Llamado por el botón Back. Carga la escena HelpScene.
    /// </summary>
    public void BackToHelp()
    {
        SceneManager.LoadScene("helpScene");   // Usa exactamente el nombre de tu escena de origen
    }
}
