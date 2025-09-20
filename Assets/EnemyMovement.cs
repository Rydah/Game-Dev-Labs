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
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
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
            foreach (ContactPoint2D contact in col.contacts)
            {
                // Player landed from above
                if (contact.normal.y < -0.3f)
                {
                    GameManager.Instance.AddScore(100);

                    // Make player bounce
                    float bounceForce = 10f;
                    Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce); // adjust bounce

                    Destroy(gameObject); // enemy dies
                    break;
                }
                else
                {
                    GameManager.Instance.ShowGameOverScreen();
                    break;
                }
            }
        }
    }
}