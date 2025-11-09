using UnityEngine;

public class Pellet : MonoBehaviour
{
    [Header("Settings")]
    public int scoreValue = 10;
    public AudioClip eatSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            // 2. (Optional) เพิ่มคะแนน
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(scoreValue);
            }

            if (AudioManager.instance != null && eatSound != null)
            {
                AudioManager.instance.PlaySFX(eatSound);
            }

            if (BonusTimeManager.instance != null)
            {
                BonusTimeManager.instance.NotifyPelletEaten();
            }

            Destroy(gameObject);
        }
    }

}
