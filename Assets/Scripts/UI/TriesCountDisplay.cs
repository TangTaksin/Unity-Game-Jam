using TMPro;
using UnityEngine;

public class TriesCountDisplay : MonoBehaviour
{
    TextMeshPro triesTMP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        triesTMP = GetComponent<TextMeshPro>();

        DrainAndRespawn.OnTriesChanged += UpdateDisplay;
    }

    private void OnDisable()
    {
        DrainAndRespawn.OnTriesChanged -= UpdateDisplay;
    }

    void UpdateDisplay(int cur_tries)
    {
        var scoreTxt = string.Format("tries: {0}", cur_tries);
        triesTMP.text = scoreTxt;
    }
}
