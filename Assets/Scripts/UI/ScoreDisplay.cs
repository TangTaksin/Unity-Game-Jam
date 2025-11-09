using TMPro;
using UnityEngine;


public class ScoreDisplay : MonoBehaviour
{
    TextMeshPro scoreTMP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        scoreTMP = GetComponent<TextMeshPro>();

        GameManager.OnScoreUpdate += UpdateDisplay;
    }

    private void OnDisable()
    {
        GameManager.OnScoreUpdate -= UpdateDisplay;
    }

    void UpdateDisplay(int cur_score)
    {
        var scoreTxt = string.Format("{0}", cur_score);
        scoreTMP.text = scoreTxt;
    }

}
