using UnityEngine;

public class ExitEvent : MonoBehaviour
{
    [HideInInspector] public MapGenerator mapGenerator;
    [HideInInspector] public int gridX;
    [HideInInspector] public int gridY;
    [HideInInspector] public GameObject floorPrefab;
    [HideInInspector] public Transform floorParent;

    // เพิ่ม event delegate สำหรับผู้ฟังอื่น
    public delegate void OnPlayerExitHandler();
    public event OnPlayerExitHandler OnPlayerExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"Player Exit {gameObject.name}");

        // วางพื้นกลับ
        if (floorPrefab != null && floorParent != null)
        {
            GameObject floor = Instantiate(floorPrefab, transform.position, Quaternion.identity, floorParent);
            floor.name = $"Floor_{gridX},{gridY}";
        }

        // ลบตัวเองออกจาก mapData
        if (mapGenerator != null)
        {
            if (mapGenerator.mapData != null &&
                gridX >= 0 && gridX < mapGenerator.columns &&
                gridY >= 0 && gridY < mapGenerator.rows)
            {
                mapGenerator.mapData[gridX, gridY] = null;
            }

            // สุ่ม Trigger ใหม่
            mapGenerator.SpawnTrigger();
        }
        else
        {
            Debug.LogWarning("ExitEvent: mapGenerator is null!");
        }

        // เรียก Event ให้ผู้ฟังอื่นทราบว่า Player Exit
        OnPlayerExit?.Invoke();

        // ลบ Trigger ตัวเอง
        Destroy(gameObject);
    }
}
