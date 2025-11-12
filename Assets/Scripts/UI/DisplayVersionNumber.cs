using TMPro;
using UnityEngine;

public class DisplayVersionNumber : MonoBehaviour
{
// This is the variable that will hold our text component
    public TextMeshProUGUI versionText;

    void Start()
    {
        // Check if the text component has been assigned in the inspector
        if (versionText != null)
        {
            // Set the text to show the version number.
            // Application.version gets the version from Project Settings.
            // We add "v" in front for clarity (e.g., "v1.0.1")
            versionText.text = "v" + Application.version;
        }
        else
        {
            // Log an error to the console if we forgot to link the text
            Debug.LogError("Version Text component is not assigned in the Inspector!");
        }
    }
}
