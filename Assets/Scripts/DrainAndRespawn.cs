using UnityEngine;

public class DrainAndRespawn : MonoBehaviour
{
    // ลาก "จุดเกิด" (RespawnPoint) มาใส่
    public Transform respawnPoint;
    
    // ลาก "PlungerManager" GameObject มาใส่
    public Plunger2D plunger;

    void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่า "Ball" ตกลงมา (ต้องตั้ง Tag "Ball" ให้ลูกบอล)
        if (other.CompareTag("Ball"))
        {
            if (respawnPoint == null || plunger == null)
            {
                Debug.LogError("Drain: ยังไม่ได้ตั้งค่า Respawn Point หรือ Plunger!");
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

            // 3. บอก Plunger ว่า "พร้อมยิงลูกใหม่"
            plunger.SetBallReady(ball);
        }
    }
}