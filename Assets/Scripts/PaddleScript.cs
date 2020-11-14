using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleScript : MonoBehaviour {
    public float speed = 150;

    private void SetSpeed(float dir) {
        GetComponent<Rigidbody2D>().velocity = Vector2.right * dir * speed;
    }

    void FixedUpdate() {
        float h = Input.GetAxisRaw("Horizontal");
        SetSpeed(h);
    }
}
