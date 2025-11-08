using UnityEngine;

public class DrainAndRespawn : MonoBehaviour
{
// ลาก "จุดเกิด" (RespawnPoint) มาใส่
    public Transform respawnPoint;

    // *** ไม่ต้องมี Plunger2D plunger อีกต่อไป ***

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            if (respawnPoint == null)
            {
                Debug.LogError("Drain: ยังไม่ได้ตั้งค่า Respawn Point!");
                return;
            }

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

            // *** ไม่ต้องเรียก plunger.SetBallReady() อีกต่อไป ***
            // เพราะเดี๋ยว "จุดเกิด" จะไปเรียก OnTriggerEnter2D ของ LauncherTrigger เอง
            
            // (Optional) บอก ScoreManager ให้รีเซ็ตคะแนน
            // if (ScoreManager.instance != null)
            // {
            //     ScoreManager.instance.SaveHighScore();
            //     ScoreManager.instance.ResetScore();
            // }
        }
    }
}