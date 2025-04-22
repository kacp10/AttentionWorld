using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 20f;

    void Update()
    {
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}
