using TMPro;
using UnityEngine;

public class VersionLabel : MonoBehaviour
{
    private static VersionLabel _instance;

    [SerializeField] private TextMeshProUGUI label;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(transform.root.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        label.text = BuildVersionString();
    }

    private static string BuildVersionString()
    {
        string text = $"v{Application.version}";

        if (Application.isEditor)       // Unity editor (including ParrelSync clones)
            text += " (editor)";
        else if (Debug.isDebugBuild)    // Standalone build with Development Build ticked
            text += " (dev)";
        // else                         // Release Build

        return text;
    }
}