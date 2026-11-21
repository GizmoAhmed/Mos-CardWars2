using UnityEngine;

public class DestroyTracer : MonoBehaviour
{
    private void OnDestroy()
    {
        Debug.Log($"{name} destroyed\n{System.Environment.StackTrace}");
    }
}