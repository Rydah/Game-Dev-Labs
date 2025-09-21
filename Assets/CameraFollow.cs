using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // assign Player in Inspector
    public float smoothSpeed = 0.125f; // lower = smoother

    public Transform startLimit; // GameObject that indicates start of map
    public Transform endLimit; // GameObject that indicates end of map
    private float offset; // initial x-offset between camera and Mario
    private float startX; // smallest x-coordinate of the Camera
    private float endX; // largest x-coordinate of the camera
    private float viewportHalfWidth;

    private PlayerMovement playerMovement; // reference to PlayerMovement script

    void Start()
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)); // the z-component is the distance of the resulting plane from the camera 
        viewportHalfWidth = Mathf.Abs(bottomLeft.x - this.transform.position.x);
        offset = this.transform.position.x - player.position.x;
        startX = startLimit.transform.position.x + viewportHalfWidth;
        endX = endLimit.transform.position.x - viewportHalfWidth;

        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }
    void Update()
    {
        // Stop camera if player is dead
        if (playerMovement != null && playerMovement.isDead) return;

        float desiredX = player.position.x + offset;
        desiredX = Mathf.Clamp(desiredX, startX, endX); 

        float desiredY = player.position.y;

        Vector3 desiredPosition = new Vector3(desiredX, desiredY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}
