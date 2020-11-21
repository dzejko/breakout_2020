using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleScript : MonoBehaviour {
	private readonly float boundsX = 3.35f;
	private readonly float boundsY = -4.5f;
	private Vector2 playerPosition;
    Rigidbody2D body;

	private void Awake() {
        body = GetComponent<Rigidbody2D>();
		playerPosition = gameObject.transform.position;
	}

	private void FixedUpdate() {
		Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		playerPosition = new Vector2(mouse.x, boundsY);
		if (playerPosition.x < -boundsX) {
			playerPosition.x = -boundsX;
		}

		if (playerPosition.x > boundsX) {
			playerPosition.x = boundsX;
		}

		body.position = playerPosition;
	}
}
