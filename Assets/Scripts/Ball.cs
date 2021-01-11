using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Ball : MonoBehaviour {
    // What happens when the ball dies?
    public static event UnityAction<Ball> OnBallDeath;

    // Different sound clips for bouncing
    public AudioClip bounceSfx;
    public AudioClip paddleSfx;
    public AudioClip destroySfx;

    // Audio pitch
    private float audioPitch;

    // Audio source 
    private AudioSource audioSource;

    // Ball body
    Rigidbody2D body;

    // Powerups
    public bool isTitanBall;
    public ParticleSystem titanBallEffect;

    public bool isHyperBall;

    // Events
    public static UnityAction<Ball> OnTitanBallEnable;
    public static UnityAction<Ball> OnTitanBallDisable;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        titanBallEffect = GetComponent<ParticleSystem>();

        audioPitch = 1.0f;

        this.isTitanBall = false;
        this.isHyperBall = false;
        this.titanBallEffect.Stop();
    }

    // Hyperball powerup
    public void StartHyperBall(float duration) {
        if (!this.isHyperBall) {
            this.isHyperBall = true;
            BallManager.Instance.initialBallSpeed += 300;

            var pm = GetComponent<TrailRenderer>();
            pm.startColor = new Color(1, 1, 0, 0.6f);
            pm.endColor = new Color(1, 1, 1, 0);

            StartCoroutine(StopHyperBallAfter(duration));
        }
    }

    private IEnumerator StopHyperBallAfter(float duration) {
        yield return new WaitForSeconds(duration);

        this.StopHyperBall();
    }

    public void StopHyperBall() {
        if (this.isHyperBall) {
            this.isHyperBall = false;
            BallManager.Instance.initialBallSpeed -= 300;

            var pm = GetComponent<TrailRenderer>();
            pm.startColor = new Color(0, 0.649f, 1, 0.6f);
            pm.endColor = new Color(1, 1, 1, 0);
        }
    }

    // Titan ball powerup
    public void StartTitanBall(float duration) {
        if (!this.isTitanBall) {
            this.audioPitch = 0.85f;
            this.audioSource.pitch = this.audioPitch;
            this.isTitanBall = true;
            this.titanBallEffect.Play();
            StartCoroutine(StopTitanBallAfter(duration));

            OnTitanBallEnable?.Invoke(this);
        }
    }

    private IEnumerator StopTitanBallAfter(float duration) {
        yield return new WaitForSeconds(duration);

        this.StopTitanBall();
    }

    private void StopTitanBall() {
        if (this.isTitanBall) {
            this.audioPitch = 1.0f;
            this.audioSource.pitch = this.audioPitch;
            this.isTitanBall = false;
            this.titanBallEffect.Stop();

            OnTitanBallDisable?.Invoke(this);
        }
    }

    // Plays a sound effect depending on the thing it collided with
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Block")) {
            this.audioSource.PlayOneShot(destroySfx);
        } else if (col.gameObject.CompareTag("Wall")) {
            this.audioSource.PlayOneShot(bounceSfx);
        } else if (col.gameObject.CompareTag("Paddle")) {
            this.audioSource.PlayOneShot(paddleSfx);
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Block")) {
            this.audioSource.PlayOneShot(destroySfx);
        } else if (col.gameObject.CompareTag("Wall")) {
            this.audioSource.PlayOneShot(bounceSfx);
        } else if (col.gameObject.CompareTag("Paddle")) {
            this.audioSource.PlayOneShot(paddleSfx);
        }
    }

    public void Die() {
        OnBallDeath?.Invoke(this);
        Destroy(gameObject, 1);
    }
}
