using UnityEngine;

public class TriggerTile : MonoBehaviour
{
    [HideInInspector] public MapGenerator mapGenerator;
    [HideInInspector] public int gridX;
    [HideInInspector] public int gridY;
    [HideInInspector] public GameObject floorPrefab;
    [HideInInspector] public Transform floorParent;

    [Header("Prefabs to Spawn")]
    public GameObject[] spawnPrefabs; // กำหนดใน Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"ผู้เล่นเหยียบช่อง {gameObject.name}");

        // วางพื้นกลับ ถ้ามี floorPrefab และ floorParent
        if (floorPrefab != null && floorParent != null)
        {
            GameObject floor = Instantiate(floorPrefab, transform.position, Quaternion.identity, floorParent);
            floor.name = $"Floor_{gridX},{gridY}";
        }

        if (mapGenerator != null && spawnPrefabs != null && spawnPrefabs.Length > 0)
        {
            // สุ่มเลือก prefab
            GameObject prefabToSpawn = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

            // ตรวจสอบรอบๆ บน ล่าง ซ้าย ขวา
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0, 1),  // บน
                new Vector2Int(0, -1), // ล่าง
                new Vector2Int(-1, 0), // ซ้าย
                new Vector2Int(1, 0)   // ขวา
            };

            bool spawned = false;
            foreach (var dir in directions)
            {
                int nx = gridX + dir.x;
                int ny = gridY + dir.y;

                if (mapGenerator.IsEmpty(nx, ny))
                {
                    Vector3 pos = new Vector3(nx * mapGenerator.columnSpacing, ny * mapGenerator.rowSpacing, 0);
                    GameObject obj = Instantiate(prefabToSpawn, pos, Quaternion.identity);
                    obj.name = $"Spawn_{nx},{ny}";
                    spawned = true;
                    break;
                }
            }

            // ถ้าไม่มีที่ว่างรอบๆ วางตรงตำแหน่งเดิม
            if (!spawned)
            {
                Vector3 pos = transform.position;
                GameObject obj = Instantiate(prefabToSpawn, pos, Quaternion.identity);
                obj.name = $"Spawn_{gridX},{gridY}";
            }
        }

        // ลบตัวเองออกจาก mapData
        if (mapGenerator != null && mapGenerator.mapData != null)
        {
            if (gridX >= 0 && gridX < mapGenerator.columns &&
                gridY >= 0 && gridY < mapGenerator.rows)
            {
                mapGenerator.mapData[gridX, gridY] = null;
            }

            // สุ่ม Trigger ใหม่
            mapGenerator.SpawnTrigger();
        }
        else
        {
            Debug.LogWarning("TriggerTile: mapGenerator ยังไม่ได้กำหนดค่า!");
        }

        // ทำลบ Trigger ตัวเอง
        Destroy(gameObject);
    }
}
