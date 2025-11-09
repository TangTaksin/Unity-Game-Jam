using UnityEngine;
using UnityEngine.UI;

public class BonusDisplay : MonoBehaviour
{
    public GameObject bonusPanel;
    public Image timeFillImg;

    private void OnEnable()
    {
        bonusPanel.SetActive(false);

        BonusTimeManager.OnBonusStart += CallBonusPanel;
        BonusTimeManager.OnBonusEnd += CloseBonusTimer;
    }

    private void OnDisable()
    {
        BonusTimeManager.OnBonusStart -= CallBonusPanel;
        BonusTimeManager.OnBonusEnd -= CloseBonusTimer;
    }

    private void Update()
    {
        UpdateTimer();
    }

    void CallBonusPanel()
    {
        bonusPanel.SetActive(true);
    }

    void CloseBonusTimer()
    {
        bonusPanel.SetActive(false);
    }

    void UpdateTimer()
    {
        if (!bonusPanel.activeSelf)
            return;

        var timer = BonusTimeManager.instance.GetTimers();

        var rem_time = timer.Item1;
        var max_time = timer.Item2;

        timeFillImg.fillAmount = (rem_time / max_time);
    }
}
