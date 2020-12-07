using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Ball : MonoBehaviour {
    public static event UnityAction<Ball> OnBallDeath;
    public AudioClip bounceSfx;
    public AudioClip paddleSfx;
    public AudioClip destroySfx;
    private AudioSource audioSource;
    Rigidbody2D body;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        audioSource.pitch = 1.0f;
    }

    // Plays a sound effect depending on the thing it collided with
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Block")) {
            audioSource.PlayOneShot(destroySfx);
        } else if (col.gameObject.CompareTag("Wall")) {
            audioSource.PlayOneShot(bounceSfx);
        } else if (col.gameObject.CompareTag("Paddle")) {
            audioSource.PlayOneShot(paddleSfx);
        }
    }

    public void Die() {
        OnBallDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
