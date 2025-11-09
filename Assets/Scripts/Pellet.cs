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
            GameManager.instance.AddScore(scoreValue);

            if (eatSound != null)
            {
                AudioManager.instance?.PlaySFX(eatSound);
            }

            BonusTimeManager.instance?.NotifyPelletEaten();

            gameObject.SetActive(false);
        }
    }

}
