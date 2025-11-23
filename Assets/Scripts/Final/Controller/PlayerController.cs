using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gridSize = 1f;
    public float maxMoveTime = 0.3f; // จำกัดเวลา 1 วิ

    private Rigidbody2D rb;

    private bool isMoving = false;
    private Vector3 targetPos;
    private Vector3 startPos;
    private float moveTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        startPos = transform.position;
        targetPos = startPos;
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector2 input = Vector2.zero;

            if (Input.GetKeyDown(KeyCode.W)) input = Vector2.up;
            if (Input.GetKeyDown(KeyCode.S)) input = Vector2.down;
            if (Input.GetKeyDown(KeyCode.A)) input = Vector2.left;
            if (Input.GetKeyDown(KeyCode.D)) input = Vector2.right;

            if (input != Vector2.zero)
            {
                TryMove(input);
            }
        }
        else
        {
            moveTimer += Time.deltaTime;

            // เดินไปตำแหน่งเป้าหมาย
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // ถ้าถึงเป้าหมายแล้ว
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                transform.position = targetPos;
                isMoving = false;
            }

            // 🚨 ถ้าเดินเกินเวลาแล้วยังไม่ถึงช่อง → ย้อนกลับ
            if (moveTimer > maxMoveTime)
            {
                Debug.LogWarning("Move timeout → return to start position");

                transform.position = startPos; // กลับตำแหน่งเดิม
                isMoving = false;
            }
        }
    }

    void TryMove(Vector2 dir)
    {
        Vector3 nextPos = transform.position + (Vector3)dir * gridSize;

        // ตรวจช่องถัดไปว่าติด wall ไหม
        Collider2D hit = Physics2D.OverlapBox(
            nextPos, new Vector2(0.8f, 0.8f),
            0f, LayerMask.GetMask("Wall")
        );

        if (hit == null)
        {
            startPos = transform.position;   // จำช่องปัจจุบัน
            targetPos = nextPos;            // ตั้งเป้าช่องใหม่
            moveTimer = 0f;                 // รีเซ็ต timer
            isMoving = true;
        }
        else
        {
            Debug.Log("Blocked by wall: " + nextPos);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(targetPos, new Vector2(0.8f, 0.8f));
    }
}
