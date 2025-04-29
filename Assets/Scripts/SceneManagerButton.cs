using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToAssignScene : MonoBehaviour
{
    public void Go()
    {
        SceneManager.LoadScene("AssignScene");
    }
}
