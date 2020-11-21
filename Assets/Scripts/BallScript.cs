using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BallScript : MonoBehaviour {
	public AudioClip bounceSfx;
	public AudioClip destroySfx;
	public AudioSource audioSource;
	Rigidbody2D body;
	Rigidbody2D paddleBody;


	public float speed = 20.0f;
	private readonly float bounds = 6;
	float blockPitch;
	float blockTime;
	bool launched;

	private void Awake() {
		body = GetComponent<Rigidbody2D>();
		paddleBody = GameObject.Find("Paddle").GetComponent<Rigidbody2D>();
		audioSource = GetComponent<AudioSource>();

		audioSource.pitch = 1.0f;
	}

	void Start() {
		Reset();
	}

	private void Reset() {
		launched = false;
		GetComponent<TrailRenderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
	}

	private void Launch() {
		body.velocity = Vector2.up * speed;

		launched = true;
		GetComponent<TrailRenderer>().enabled = true;
		GetComponent<Collider2D>().enabled = true;
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.name == "Paddle") {
			float x = HitFactor(transform.position, col.transform.position, col.collider.bounds.size.x);
			Vector2 dir = new Vector2(x, 1).normalized;

			body.velocity = dir * speed;
		} else {
			// TODO: Fix the ball getting stuck on extreme angles
			// and the paddle double collision hitray accelerating the ball.
		}

		if (col.gameObject.CompareTag("Block")) {
			PlayBlockSound();
			Destroy(col.gameObject);
		} else {
			audioSource.pitch = Random.Range(0.75f, 1.5f);
			audioSource.clip = bounceSfx;

			audioSource.Play();
		}

	}

	private void FixedUpdate() {
		if (!launched) {
			body.velocity = Vector2.zero;
			body.position = new Vector2(paddleBody.position.x, paddleBody.position.y + 0.7f);
			
			if (Input.GetKey(KeyCode.Space)) {
				Launch();
			}
		} else {
			if (Mathf.Abs(body.velocity.y) < Mathf.Epsilon) {
				body.velocity = Vector2.up * speed;
			}

			if (body.position.y <= -bounds) {
				Reset();
			}
		}
	}

	void PlayBlockSound() {
		audioSource.clip = destroySfx;
		if (Time.time - blockTime < 1f) {
			blockPitch += 0.15f;
		} else {
			blockPitch = 1f;
		}

		audioSource.pitch = blockPitch;
		audioSource.Play();

		blockTime = Time.time;
	}

	private float HitFactor(Vector2 ball_pos, Vector2 paddle_pos, float paddle_width) {
		return (ball_pos.x - paddle_pos.x) / paddle_width;
	}
}
