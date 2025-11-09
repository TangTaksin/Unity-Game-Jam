using System.Collections;
using UnityEngine;

public class HitTarget : MonoBehaviour
{
public int scoreValue = 250; // คะแนนที่ได้เมื่อชน

    [Header("Feedback (Optional)")]
    public AudioClip hitSound;
    public Color hitColor = Color.cyan;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.material.color;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            HandleHit();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            HandleHit();
        }
    }
    
    private void HandleHit()
    {
        // 1. (Optional) เพิ่มคะแนน
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(scoreValue);
        }

        // 2. (Optional) เล่นเสียง
        if (AudioManager.instance != null && hitSound != null)
        {
            AudioManager.instance.PlaySFXRandomPitch(hitSound);
        }

        if (BonusTimeManager.instance != null)
        {
            BonusTimeManager.instance.NotifyTargetHit();
        }

        // 5. (Optional) กระพริบไฟ
        if (spriteRenderer != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashColor());
        }
    }

    // Coroutine สำหรับกระพริบสี
    private IEnumerator FlashColor()
    {
        spriteRenderer.material.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.color = originalColor;
    }
}
