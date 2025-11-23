using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public float SmoothSpeed = 0.125f; // ความลื่นไหลของการติดตาม
    public Vector3 Offset; // ระยะห่างระหว่างกล้องและตัวละคร

    void FixedUpdate()
    {
        if (Target != null)
        {
            // คำนวณตำแหน่งของกล้อง
            Vector3 desiredPosition = Target.position + Offset;

            // กล้อง +3 ในแกน X
            if (Target.GetComponent<Rigidbody2D>().velocity.x > 0)
            {
                desiredPosition.x = Target.position.x + 3f;
            }
            else if (Target.GetComponent<Rigidbody2D>().velocity.x < 0)
            {
                desiredPosition.x = Target.position.x - 3f;
            }
            else
            {
                desiredPosition.x = Target.position.x;
            }

            // ล็อคแกน Z
            desiredPosition.z = transform.position.z;

            // จำกัดค่าของแกน X และ Y
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, -2.75f, 160.5f); // X
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, -1.94f, 4.22f); // Y

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);

            // Update ตำแหน่งกล้อง
            transform.position = smoothedPosition;
        }
    }
}
