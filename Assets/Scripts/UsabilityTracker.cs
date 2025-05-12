
using UnityEngine;
using System.Collections.Generic;

public class UsabilityTracker : MonoBehaviour
{
    readonly Dictionary<string, float> t0 = new();

    public void StartTask(string id) => t0[id] = Time.time;
    public void EndTask(string id, bool ok)
    {
        if (!t0.ContainsKey(id)) return;

        float t = Time.realtimeSinceStartup - t0[id];  
        string flag = ok ? "OK" : "FAIL";
        Debug.Log($"[USABILITY] {id} {flag} {t:F3}s");  
        t0.Remove(id);
    }

}
