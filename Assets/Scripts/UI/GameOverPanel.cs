using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public GameObject gameoverPanel;
    public TextMeshProUGUI scoreTMPUI;
    [SerializeField] private AudioClip gameOverSound;

    private void OnEnable()
    {
        gameoverPanel.SetActive(false);
        GameManager.OnGameEnd += CallGameoverPanel;
    }

    private void OnDisable()
    {
        GameManager.OnGameEnd -= CallGameoverPanel;
    }

    void CallGameoverPanel()
    {
        gameoverPanel.SetActive(true);

        var score = GameManager.instance.Getscore();

        var scoreTxt = string.Format("{0}", score);
        scoreTMPUI.text = scoreTxt;
        if (gameOverSound != null)
        {
            AudioManager.instance.PlaySFX(gameOverSound);
        }
    }

    public void PlayAgain()
    {
        GameManager.instance.BeginNewGame();
    }
}
