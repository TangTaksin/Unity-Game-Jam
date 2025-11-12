using System;
using UnityEngine;

public class DrainAndRespawn : MonoBehaviour
{
    [SerializeField] private int maxTries = 3;
    int remainingTries;

    // ลาก "จุดเกิด" (RespawnPoint) มาใส่
    public Transform respawnPoint;

    public static Action<int> OnTriesChanged;
    // *** ไม่ต้องมี Plunger2D plunger อีกต่อไป ***

    [SerializeField] private AudioClip drainSound;

    private void Start()
    {
        remainingTries = maxTries;
        OnTriesChanged?.Invoke(remainingTries);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            if (respawnPoint == null)
            {
                Debug.LogError("Drain: ยังไม่ได้ตั้งค่า Respawn Point!");
                return;
            }

            remainingTries--;
            OnTriesChanged?.Invoke(remainingTries);

            if (remainingTries <= 0)
                return;

            GameObject ball = other.gameObject;

            // 1. ย้ายลูกบอลกลับไปจุดเกิด
            ball.transform.position = respawnPoint.position;

            // 2. รีเซ็ตความเร็ว
            Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector2.zero;
                ballRb.angularVelocity = 0f;
            }

            // 3. (ถ้ามี) เล่นเสียง
            if (drainSound != null)
            {
                AudioManager.instance.PlaySFX(drainSound);
            }
        }
    }
}