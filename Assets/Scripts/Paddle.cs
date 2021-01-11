using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour {
    #region Singleton
    private static Paddle instance;
    public static Paddle Instance => instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }
    #endregion

    // Left-right walls and DeathWall boundary
    private readonly float boundsX = 3.35f;
    private readonly float boundsY = -4.5f;
    private Vector2 playerPosition;
    Rigidbody2D body;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        playerPosition = gameObject.transform.position;
    }

    private void FixedUpdate() {
        // Convert the mouse "crosshair" from the screen coordinates to world coordinates
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerPosition = new Vector2(mouse.x, boundsY);

        // Check the paddle position and make sure you cannot get out of bounds
        if (playerPosition.x < -boundsX) {
            playerPosition.x = -boundsX;
        }
        if (playerPosition.x > boundsX) {
            playerPosition.x = boundsX;
        }

        // Set the paddle's position to the mouse
        body.position = playerPosition;
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Ball") {
            // Get the rigidbody responsible for the ball
            Rigidbody2D ballBody = col.gameObject.GetComponent<Rigidbody2D>();

            // Get the very "point" it collided with on the paddle
            Vector3 hitPoint = col.GetContact(0).point;

            // Calculate the center of the paddle and reset the velocity of the ball.
            Vector3 paddleCenter = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y);
            ballBody.velocity = Vector2.zero;

            // Calculate the difference of the point of collision in relation to the paddle's center
            float difference = paddleCenter.x - hitPoint.x;

            // Apply an equal force (but upwards) depending on where it landed
            if (hitPoint.x < paddleCenter.x) {
                float dir = -(Mathf.Abs(difference * 200));
                ballBody.AddForce(new Vector2(dir, BallManager.Instance.initialBallSpeed));
            } else {
                float dir = (Mathf.Abs(difference * 200));
                ballBody.AddForce(new Vector2(dir, BallManager.Instance.initialBallSpeed));
            }
        }
    }
}
