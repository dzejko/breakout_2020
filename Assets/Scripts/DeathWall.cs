using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Ball") {
            Ball ball = col.gameObject.GetComponent<Ball>();
            BallManager.Instance.Balls.Remove(ball);
            ball.Die();
        }
    }
}
