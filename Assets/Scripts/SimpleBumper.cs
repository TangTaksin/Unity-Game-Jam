using UnityEngine;
using DG.Tweening;

public class SimpleBumper : MonoBehaviour
{
    [Header("Bumper Settings")]
    public float bounceForce = 15f;
    public int scoreValue = 100;

    [Header("Visuals")]
    public Renderer bumperRenderer;
    public Color hitColor = Color.yellow;
    private Color originalColor;

    [Header("Audio")]
    public AudioClip hitSound;

    [Header("Tween Settings")]
    public float colorFlashDuration = 0.25f;
    public float scalePunch = 0.2f;
    public float punchDuration = 0.3f;

    private Vector3 originalScale;

    void Start()
    {
        if (bumperRenderer == null)
            bumperRenderer = GetComponent<Renderer>();

        // Cache originals
        if (bumperRenderer != null)
            originalColor = bumperRenderer.material.color;

        originalScale = transform.localScale;
    }

    // Also update originalScale in editor when values change
    void OnValidate()
    {
        // Prevent altering runtime values in editor, but keep sensible defaults
        if (!Application.isPlaying)
            originalScale = transform.localScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (GameManager.instance != null)
                GameManager.instance.AddScore(scoreValue);

            Debug.Log("Hit! Score: " + scoreValue);

            if (hitSound != null && AudioManager.instance != null)
                AudioManager.instance.PlaySFX(hitSound);

            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                Vector2 bounceDir = -collision.contacts[0].normal;
                ballRb.AddForce(bounceDir * bounceForce, ForceMode2D.Impulse);
            }

            AnimateHitFeedback();
        }
    }

    private void AnimateHitFeedback()
    {
        // 1) Kill any running tweens on the transform and material
        //    (don't complete them — we want to reset)
        transform.DOKill();
        if (bumperRenderer != null)
            bumperRenderer.material.DOKill();

        // 2) Immediately reset transform scale to original before starting new tween
        transform.localScale = originalScale;

        // 3) Start color tween (safe-guard in case renderer is missing)
        if (bumperRenderer != null)
        {
            // tween to hitColor then back to original
            bumperRenderer.material
                .DOColor(hitColor, colorFlashDuration * 0.5f)
                .OnComplete(() =>
                    bumperRenderer.material.DOColor(originalColor, colorFlashDuration * 0.5f)
                );
        }

        // 4) Start punch scale tween (will return to originalScale)
        transform.DOPunchScale(Vector3.one * scalePunch, punchDuration, 8, 0.5f);
    }

    // Safety: ensure scale is restored if object disabled/destroyed (editor/playmode switches etc.)
    void OnDisable()
    {
        ResetVisuals();
    }

    void OnDestroy()
    {
        ResetVisuals();
    }

    private void ResetVisuals()
    {
        // Kill any tweens and restore original values
        transform.DOKill();
        transform.localScale = originalScale;

        if (bumperRenderer != null)
        {
            bumperRenderer.material.DOKill();
            bumperRenderer.material.color = originalColor;
        }
    }
}
