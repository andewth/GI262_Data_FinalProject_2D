using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    public int columns = 18;
    public int rows = 9;

    [Header("Prefabs")]
    public GameObject[] floorPrefabs;
    public GameObject[] wallPrefabs;
    public GameObject[] triggerPrefabs;
    public GameObject exitPrefab;

    [Header("Parents")]
    public Transform floorParent;
    public Transform wallParent;
    public Transform triggerParent;
    public Transform exitParent;

    [Header("Spacing")]
    public float columnSpacing = 6.86f;
    public float rowSpacing = 6.86f;

    [Header("Wall Identity")]
    public GameObject wallIdentity;

    [Header("Random Amounts")]
    public int randomWallCount = 10;
    public int randomTriggerCount = 5;

    [Header("Player")]
    public Transform playerTransform;

    [Header("Safe Zones (Grid Distance)")]
    public float safeDistance = 2f;
    public float exitSafeDistance = 1f;

    [HideInInspector]
    public GameObject[,] mapData;

    private Vector2Int exitPos;
    private bool exitPlaced = false;

    private bool exitSpawned = false;

    [Range(0, 100)]
    public int exitSpawnChancePercent = 30;

    public static object Instance { get; internal set; }

    private void Awake()
    {
        GenerateMap();
        // PlaceExit();
    }

    public void GenerateMap()
    {
        mapData = new GameObject[columns, rows];

        // Floor + Border Walls
        for (int x = -1; x <= columns; x++)
        {
            for (int y = -1; y <= rows; y++)
            {
                Vector3 pos = new Vector3(x * columnSpacing, y * rowSpacing, 0);

                if (x < 0 || x >= columns || y < 0 || y >= rows)
                {
                    // Border Wall
                    GameObject wallObj = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)], pos, Quaternion.identity, wallParent);
                    wallObj.name = $"BorderWall_{x},{y}";
                    continue;
                }

                // Floor Tile
                GameObject floorObj = Instantiate(floorPrefabs[Random.Range(0, floorPrefabs.Length)], pos, Quaternion.identity, floorParent);
                floorObj.name = $"Floor_{x},{y}";
                mapData[x, y] = null;
            }
        }

        PlaceRandomWalls();
        PlaceRandomTriggers();
    }

    public bool IsEmpty(int x, int y)
    {
        if (x < 0 || x >= columns || y < 0 || y >= rows)
            return false;

        return mapData[x, y] == null;
    }

    private bool IsEmptyAndSafe(int x, int y)
    {
        if (!IsEmpty(x, y))
            return false;

        Vector2Int gridPos = new Vector2Int(x, y);

        // Safe around player
        if (playerTransform != null)
        {
            Vector3 playerPos = playerTransform.position;
            Vector2Int pGrid = ToGrid(playerPos);

            if (Vector2Int.Distance(pGrid, gridPos) < safeDistance)
                return false;
        }

        // Safe around exit
        if (exitPlaced)
        {
            if (Vector2Int.Distance(exitPos, gridPos) < exitSafeDistance)
                return false;
        }

        return true;
    }

    public Vector2Int ToGrid(Vector3 worldPos)
    {
        int gx = Mathf.RoundToInt(worldPos.x / columnSpacing);
        int gy = Mathf.RoundToInt(worldPos.y / rowSpacing);
        return new Vector2Int(gx, gy);
    }

    // ------------------------------
    // RANDOM WALL GENERATION
    // ------------------------------
    private void PlaceRandomWalls()
    {
        int placed = 0;
        int attempts = 0;

        while (placed < randomWallCount && attempts < 150)
        {
            attempts++;

            int x = Random.Range(0, columns - 1);
            int y = Random.Range(0, rows - 1);

            bool horizontal = Random.value < 0.5f;

            int x2 = horizontal ? x + 1 : x;
            int y2 = horizontal ? y : y + 1;

            if (x2 >= columns || y2 >= rows) continue;

            // เช็คว่า 2 จุดนี้ว่าง + ปลอดภัย + ไม่ชิดกำแพงอื่น
            if (IsEmptyAndSafe(x, y) &&
                IsEmptyAndSafe(x2, y2) &&
                NoAdjacentWalls(x, y) &&
                NoAdjacentWalls(x2, y2))
            {
                SpawnWall(x, y);
                SpawnWall(x2, y2);
                placed += 2;
            }
        }
    }


    private void SpawnWall(int x, int y)
    {
        Vector3 pos = new Vector3(x * columnSpacing, y * rowSpacing, 0);
        GameObject obj = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)], pos, Quaternion.identity, wallParent);
        obj.name = $"Wall_{x},{y}";
        mapData[x, y] = obj;
    }

    // ------------------------------
    // RANDOM TRIGGERS
    // ------------------------------
    private void PlaceRandomTriggers()
    {
        int placed = 0;
        int attempts = 0;

        while (placed < randomTriggerCount && attempts < 200)
        {
            attempts++;

            int x = Random.Range(0, columns);
            int y = Random.Range(0, rows);

            if (!IsEmptyAndSafe(x, y)) continue;

            // ❗ เพิ่มเช็คเว้นระยะ 3 ช่อง
            if (!NoAdjacentTriggers(x, y)) continue;

            SpawnTriggerAt(x, y);
            placed++;
        }
    }


    private void SpawnTriggerAt(int x, int y)
    {
        Vector3 pos = new Vector3(x * columnSpacing, y * rowSpacing, 0);

        GameObject obj = Instantiate(triggerPrefabs[Random.Range(0, triggerPrefabs.Length)], pos, Quaternion.identity, triggerParent);
        obj.name = $"Trigger_{x},{y}";

        TriggerTile trigger = obj.GetComponent<TriggerTile>();
        if (trigger != null)
        {
            trigger.mapGenerator = this;
            trigger.gridX = x;
            trigger.gridY = y;
            trigger.floorPrefab = floorPrefabs[Random.Range(0, floorPrefabs.Length)];
            trigger.floorParent = floorParent;
        }

        mapData[x, y] = obj;
    }


    private bool NoAdjacentTriggers(int x, int y)
    {
        // ระยะรอบตัว 3 ช่อง (บนล่างซ้ายขวา)
        for (int i = -3; i <= 3; i++)
        {
            // แนวตั้ง (บน–ล่าง)
            int ny = y + i;
            if (i != 0 && ny >= 0 && ny < rows)
            {
                if (mapData[x, ny] != null) return false;
            }

            // แนวนอน (ซ้าย–ขวา)
            int nx = x + i;
            if (i != 0 && nx >= 0 && nx < columns)
            {
                if (mapData[nx, y] != null) return false;
            }
        }

        return true;
    }


    // ------------------------------
    // EXIT GENERATION
    // ------------------------------
    public void PlaceExit()
    {
        if (exitPrefab == null) return;
        if (exitSpawned) return;

        if (Random.Range(0, 100) >= exitSpawnChancePercent) return;

        int attempts = 0;

        exitSpawned = true;

        while (attempts < 150)
        {
            attempts++;

            int x = Random.Range(0, columns);
            int y = Random.Range(0, rows);

            if (IsEmptyAndSafe(x, y))
            {
                Vector3 pos = new Vector3(x * columnSpacing, y * rowSpacing, 0);

                GameObject exitObj = Instantiate(exitPrefab, pos, Quaternion.identity, exitParent);
                exitObj.name = $"Exit_{x},{y}";

                mapData[x, y] = exitObj;
                exitPos = new Vector2Int(x, y);
                exitPlaced = true;
                return;
            }
        }

        Debug.LogWarning("Cannot place Exit: no valid position found.");
    }

    // ------------------------------
    // SPAWN TRIGGER AFTER MAP GENERATED
    // ------------------------------
    public void SpawnTrigger()
    {
        int attempts = 0;

        while (attempts < 150)
        {
            attempts++;

            int x = Random.Range(0, columns);
            int y = Random.Range(0, rows);

            if (!IsEmptyAndSafe(x, y)) continue;

            // Extra check for Player collision
            Vector3 pos = new Vector3(x * columnSpacing, y * rowSpacing, 0);
            if (Physics2D.OverlapCircle(pos, columnSpacing * 0.4f, LayerMask.GetMask("Player")))
                continue;

            SpawnTriggerAt(x, y);
            return;
        }

        Debug.LogWarning("Cannot spawn trigger (map full or unsafe).");
    }


    private bool NoAdjacentWalls(int x, int y)
    {
        int[,] dirs = new int[,]
        {
            { 1, 0 },   // ขวา
            { -1, 0 },  // ซ้าย
            { 0, 1 },   // บน
            { 0, -1 }   // ล่าง
        };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dirs[i, 0];
            int ny = y + dirs[i, 1];

            if (nx < 0 || nx >= columns || ny < 0 || ny >= rows)
                continue;

            if (mapData[nx, ny] != null)
            {
                // เจอ wall หรือ trigger หรือ exit → ห้ามติดกัน
                return false;
            }
        }

        return true;
    }

}
