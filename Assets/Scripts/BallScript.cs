using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {
	public float speed = 20.0f;
	void Start() {
		GetComponent<Rigidbody2D>().velocity = Vector2.up * speed;
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.name == "Paddle") {
			float x = hitFactor(transform.position, col.transform.position, col.collider.bounds.size.x);

			Vector2 dir = new Vector2(x, 1).normalized;

			GetComponent<Rigidbody2D>().velocity = dir * speed;
		}
	}

	private float hitFactor(Vector2 ball_pos, Vector2 paddle_pos, float paddle_width) {
		return (ball_pos.x - paddle_pos.x) / paddle_width;
	}
}
