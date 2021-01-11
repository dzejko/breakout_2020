using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D col) {
        // Destroy the ball if it flies below the paddle
        if (col.gameObject.tag == "Ball") {
            Ball ball = col.gameObject.GetComponent<Ball>();
            BallManager.Instance.Balls.Remove(ball);
            ball.Die();
        }

        // And the powerups
        if (col.gameObject.tag == "Powerup") {
            Destroy(col.gameObject);
        }
    }
}
