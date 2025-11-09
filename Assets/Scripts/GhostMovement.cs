using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
[Header("Waypoint Settings")]
    // (สำคัญ!) ลาก "Waypoint Parent" (Empty GameObject) มาใส่ที่นี่
    public Transform waypointParent;
    
    [Header("Movement Stats")]
    public float moveSpeed = 3f; // ความเร็วของผี
    public float waypointThreshold = 0.1f; // ระยะที่ผีจะ "ยอมรับ" ว่าถึงจุดหมายแล้ว

    // "คลัง" เก็บ Waypoints ทั้งหมด
    private List<Transform> waypoints;
    private int currentTargetIndex = 0; // Waypoint เป้าหมายปัจจุบัน
    private Rigidbody2D rb;

    void Awake()
    {
        // 1. เก็บ Rigidbody2D ไว้ในตัวแปร
        rb = GetComponent<Rigidbody2D>();
        
        // (แนะนำ) ตั้งค่า Rigidbody สำหรับผี
        rb.gravityScale = 0; // ไม่ให้ผีตก
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        // 2. โหลด Waypoints ทั้งหมดจาก Parent
        InitializeWaypoints();
    }

    void InitializeWaypoints()
    {
        waypoints = new List<Transform>();
        if (waypointParent == null)
        {
            Debug.LogError("GhostMovement: ยังไม่ได้ตั้งค่า Waypoint Parent!");
            return;
        }

        // 3. (วิธีเดียวกับ Pellet Spawner)
        // วนลูป "ลูก" ทั้งหมดที่อยู่ใน Parent แล้วเพิ่มลงใน List
        foreach (Transform child in waypointParent)
        {
            waypoints.Add(child);
        }

        Debug.Log(gameObject.name + " found " + waypoints.Count + " waypoints.");
    }

    // --- (สำคัญ!) เราใช้ FixedUpdate เพราะเรายุ่งกับฟิสิกส์ (Rigidbody) ---
    void FixedUpdate()
    {
        // 4. ถ้าไม่มี Waypoints ก็ไม่ต้องทำอะไร
        if (waypoints.Count == 0) return;

        // 5. หา "เป้าหมาย" ปัจจุบัน
        Transform targetWaypoint = waypoints[currentTargetIndex];

        // 6. คำนวณ "ตำแหน่งใหม่" ที่จะขยับไป
        // (เราใช้ Vector2.MoveTowards เพื่อให้มัน "หยุด" เมื่อถึงเป้าหมาย)
        Vector2 newPosition = Vector2.MoveTowards(
            rb.position, // ตำแหน่งปัจจุบัน
            targetWaypoint.position, // ตำแหน่งเป้าหมาย
            moveSpeed * Time.fixedDeltaTime // ความเร็ว
        );

        // 7. สั่งให้ Rigidbody "ย้าย" ไปยังตำแหน่งใหม่ (วิธีนี้ปลอดภัยกับฟิสิกส์)
        rb.MovePosition(newPosition);

        // 8. ตรวจสอบว่า "ถึง" เป้าหมายหรือยัง
        if (Vector2.Distance(rb.position, targetWaypoint.position) < waypointThreshold)
        {
            // 9. ถ้าถึงแล้ว -> "เปลี่ยนเป้าหมาย" เป็นอันถัดไป
            currentTargetIndex++;

            // 10. (สำคัญ!) ถ้าเลย Waypoint สุดท้าย ให้ "วนกลับ" ไปที่ 0
            if (currentTargetIndex >= waypoints.Count)
            {
                currentTargetIndex = 0;
            }
        }
    }
}
