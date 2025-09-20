using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // assign Player in Inspector
    public float smoothSpeed = 0.125f; // lower = smoother
    public Vector3 offset; // optional offset from player

    [Header("Clamp Boundaries")]
    public float minX, maxX;
    public float minY, maxY;

    void LateUpdate()
    {
        if (player == null) return;

        // Desired camera position
        Vector3 desiredPosition = player.position + offset;

        // Clamp X and Y to level bounds
        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(desiredPosition.y + 5, minY, maxY);

        Vector3 clampedPosition = new Vector3(clampedX, clampedY, transform.position.z);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
    }
}
