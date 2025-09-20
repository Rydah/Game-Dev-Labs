using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        // get the starting position
        originalX = transform.position.x;
        ComputeVelocity();
    }

    void ComputeVelocity()
    {
        velocity = new Vector2(moveRight * maxOffset / enemyPatroltime, 0);
    }

    void Movegoomba()
    {
        Vector2 direction = new Vector2(moveRight, 0);

        // Half-width of the collider
        float halfWidth = GetComponent<Collider2D>().bounds.extents.x;

        // Start ray from the front edge
        Vector2 rayOrigin = (Vector2)transform.position + direction * halfWidth;

        // Slightly longer than 0.1, to be safe
        float rayDistance = 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, rayDistance, LayerMask.GetMask("Foreground"));

        if (hit.collider != null)
        {
            moveRight *= -1;
            ComputeVelocity();
        }
        else
        {
            enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
        }
    }

    // note that this is Update(), which still works but not ideal. See below.
    void Update()
    {
        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {// move goomba
            Movegoomba();
        }
        else
        {
            // change direction
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            bool stomped = false;

            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal.y < -0.3f)
                {
                    stomped = true;
                    break; // at least one top contact
                }
            }

            if (stomped)
            {
                GameManager.Instance.AddScore(100);

                // Make player bounce
                float bounceForce = 20f;
                Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);

                Destroy(gameObject);
            }
            else
            {
                // No top contact �� Mario dies
                GameManager.Instance.ShowGameOverScreen();
            }
        }
    }
}