using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PaddleScript : MonoBehaviour {
    public new AudioSource audio;
    public float speed = 50;

    private void AddSpeed(float dir) {
        Vector2 result = Vector2.right * dir * speed;
        GetComponent<Rigidbody2D>().velocity = result;
    }

    private void MakeMove() {
        float h = Input.GetAxisRaw("Horizontal");
        AddSpeed(h);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
		if (!collision.gameObject.name.Contains("Border")) {
            audio.Play();
		}
	}

    void FixedUpdate() {
        MakeMove();
    }
}
