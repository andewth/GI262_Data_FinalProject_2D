using UnityEngine;
using UnityEngine.SceneManagement;
using static EndGame;

public class ExitEvent : MonoBehaviour
{
    // เปลี่ยนจาก HideInInspector เป็น Header เพื่อให้เห็นใน Editor ได้ (ถ้าอยากลากใส่)
    // แต่ถ้าไม่ลากใส่ ระบบจะหาเอง
    public MapGenerator mapGenerator; 
    
    [HideInInspector] public int gridX;
    [HideInInspector] public int gridY;
    
    [Header("Settings")]
    public GameObject floorPrefab;
    public Transform floorParent;

    // Delegate & Event
    public delegate void OnPlayerExitHandler();
    public event OnPlayerExitHandler OnPlayerExit;

    // --- ส่วนที่เพิ่มเข้ามาเพื่อแก้ปัญหา Null ---
    private void Start()
    {
        // 1. ถ้ายังไม่ได้กำหนด mapGenerator ให้ลองหาจาก Singleton (ถ้ามี)
        if (mapGenerator == null)
        {
            // ถ้า MapGenerator มีการทำ Singleton (Instance) ไว้ ให้ดึงมาใช้
            // (ต้องแน่ใจว่าใน MapGenerator มี public static MapGenerator Instance)
             /* // ปลดคอมเมนต์บรรทัดด้านล่างนี้ ถ้าคุณทำ Singleton ไว้
             // mapGenerator = MapGenerator.Instance; 
             */
        }

        // 2. ถ้ายังหาไม่เจออีก (หรือไม่ได้ทำ Singleton) ให้ค้นหาทั้งฉาก
        if (mapGenerator == null)
        {
            // สำหรับ Unity 2023 ขึ้นไปใช้ FindFirstObjectByType
            // สำหรับ Unity เก่ากว่าใช้ FindObjectOfType
            mapGenerator = FindFirstObjectByType<MapGenerator>();
            
            if (mapGenerator == null)
            {
                Debug.LogError("CRITICAL: ไม่พบ MapGenerator ในฉาก! กรุณาตรวจสอบว่ามี MapGenerator อยู่ใน Hierarchy หรือไม่");
            }
        }
        
        // 3. หา floorParent อัตโนมัติถ้าไม่ได้ใส่มา (Option เสริม)
        if (floorParent == null && mapGenerator != null)
        {
            floorParent = mapGenerator.transform; 
        }
    }
    // ----------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"Player Exit {gameObject.name}");

        // วางพื้นกลับ
        if (floorPrefab != null)
        {
            // ถ้า floorParent เป็น null ให้วางไว้ที่ root (null)
            Transform targetParent = (floorParent != null) ? floorParent : null;
            
            GameObject floor = Instantiate(floorPrefab, transform.position, Quaternion.identity, targetParent);
            floor.name = $"Floor_{gridX},{gridY}";
        }

        // ลบตัวเองออกจาก mapData และ Spawn ใหม่
        if (mapGenerator != null)
        {
            // ตรวจสอบ Array Bounds เพื่อป้องกัน Error IndexOutOfRange
            if (mapGenerator.mapData != null &&
                gridX >= 0 && gridX < mapGenerator.mapData.GetLength(0) && // เช็คขนาด array โดยตรงปลอดภัยกว่า
                gridY >= 0 && gridY < mapGenerator.mapData.GetLength(1))
            {
                mapGenerator.mapData[gridX, gridY] = null;
            }

            // สุ่ม Trigger ใหม่
            mapGenerator.SpawnTrigger();
        }
        else
        {
            // ถ้ามาถึงตรงนี้แล้วยัง null แสดงว่าไม่มี MapGenerator ในฉากจริงๆ
            Debug.LogError("ExitEvent Failed: mapGenerator is still null.");
        }

        // เรียก Event ให้ผู้ฟังอื่นทราบ
        OnPlayerExit?.Invoke();

        // ลบ Trigger ตัวเอง
        Destroy(gameObject);

        GameResult.isWin = true;
        SceneManager.LoadScene("EndGame");
    }
}