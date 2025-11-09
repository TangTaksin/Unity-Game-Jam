using TMPro;
using UnityEngine;

public class TriesCountDisplay : MonoBehaviour
{
    private TextMeshPro triesTMP;         // สำหรับ TextMeshPro (3D)
    private TextMeshProUGUI triesTMP_UI;  // สำหรับ TextMeshPro (UI)

    void OnEnable()
    {
        // ตรวจหาทั้งสองแบบ
        triesTMP = GetComponent<TextMeshPro>();
        triesTMP_UI = GetComponent<TextMeshProUGUI>();

        // สมัครอีเวนต์
        DrainAndRespawn.OnTriesChanged += UpdateDisplay;
    }

    void OnDisable()
    {
        DrainAndRespawn.OnTriesChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(int curTries)
    {
        // 👉 แสดงข้อความในรูปแบบ "x 3"
        string displayText = $": {curTries}";

        // อัปเดต TMP ที่เจอ
        if (triesTMP != null)
            triesTMP.text = displayText;
        else if (triesTMP_UI != null)
            triesTMP_UI.text = displayText;
        else
            Debug.LogWarning($"[{nameof(TriesCountDisplay)}] No TextMeshPro component found on {name}!");
    }
}
