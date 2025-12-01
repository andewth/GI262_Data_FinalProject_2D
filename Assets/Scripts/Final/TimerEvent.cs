using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;
using static EndGame;

public class TimerEvent : MonoBehaviour
{
    [Header("ตั้งค่าช่วงเวลาสุ่ม (วินาที)")]
    public float minTime = 5f;
    public float maxTime = 10f;

    [Header("UI แสดงเวลา (TextMeshPro)")]
    public TextMeshProUGUI timerText;

    [Header("Event เมื่อเวลาหมด")]
    public UnityEvent onTimerEnd;

    private float countdown;

    void Start()
    {
        // สุ่มเวลานับถอยหลัง
        countdown = Random.Range(minTime, maxTime);
        UpdateTimerUI();
    }

    void Update()
    {
        // ลดเวลาทีละ frame
        countdown -= Time.deltaTime;

        // อัปเดตข้อความบนจอ
        UpdateTimerUI();

        // เมื่อเวลาหมด
        if (countdown <= 0f)
        {
            countdown = 0f;
            UpdateTimerUI();
            onTimerEnd.Invoke();

            // หากต้องการให้จบหลังครั้งเดียว ให้ปิดสคริปต์นี้
            enabled = false;
            GameResult.isWin = false;
            SceneManager.LoadScene("EndGame");
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // แสดงผลทศนิยม 2 หลัก เช่น "เหลือเวลา: 3.25"
            timerText.text = $"{countdown:F2}";
        }
    }
}
