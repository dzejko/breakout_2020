using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.ParticleSystem;

public class Brick : MonoBehaviour {
    private SpriteRenderer brickRenderer;
    private BoxCollider2D brickBody;
    public ParticleSystem destroyEffect;
    public static event UnityAction<Brick> OnBrickDestruction;
    public int hitPoints = 1;
    private int oldHitPoints = 1;

    private float scaleSpeed = 2.0f;
    private float hitIterator = 0.0f;
    private Vector3 newScale;
    private bool hit;
    private float timeTracker;


    private void Awake() {
        this.brickRenderer = this.GetComponent<SpriteRenderer>();
        this.brickBody = this.GetComponent<BoxCollider2D>();

        Ball.OnTitanBallEnable += OnTitanBallEnable;
        Ball.OnTitanBallDisable += OnTitanBallDisable;
    }

    private void OnDisable() {
        Ball.OnTitanBallEnable -= OnTitanBallEnable;
        Ball.OnTitanBallDisable -= OnTitanBallDisable;
    }

    private void OnTitanBallDisable(Ball obj) {
        if (this != null) {
            // this.brickBody.isTrigger = false;
        }
    }

    private void OnTitanBallEnable(Ball obj) {
        if (this != null) {
            // this.brickBody.isTrigger = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        bool powerHit = false;

        // If you collided with the ball, take damage.
        if (col.collider.tag == "Ball") {
            // Check if ball is hyperBall
            Ball ball = col.gameObject.GetComponent<Ball>();
            powerHit = ball.isTitanBall;
        }

        this.TakeDamage(powerHit);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        bool powerHit = false;

        // If you collided with the ball, take damage.
        if (col.tag == "Ball") {
            // Check if ball is hyperBall
            Ball ball = col.gameObject.GetComponent<Ball>();
            powerHit = ball.isTitanBall;
        }

        this.TakeDamage(powerHit);
    }

    public void TakeDamage(bool powerHit) {
        // Takes away hitpoints
        if (powerHit) {
            this.hitPoints -= 2;
        } else {
            this.hitPoints -= 1;
        }

        if (this.hitPoints <= 0 || powerHit) {
            this.hit = false;
            // This destroys any given brick.
            BrickManager.Instance.RemainingBricks.Remove(this);
            OnBrickDestruction?.Invoke(this);

            OnBrickDestroy();
            SpawnDestroyEffect();
            Destroy(this.gameObject);
        } else {
            // Initialize the tracker and mark the given brick as `hit`
            this.timeTracker = 0.0f;
            this.hit = true;
        }
    }

    private void OnBrickDestroy() {
        // Increment score counter based on the number of hitpoints
        InterfaceManager.Instance.score += this.oldHitPoints * 20;

        // Generate a powerup
        float buffChance = Random.Range(0, 100f);

        if (buffChance <= PowerupManager.Instance.BuffChance) {
            Powerup newBuff = this.SpawnBuff();
        }
    }

    private Powerup SpawnBuff() {
        // Grab the list of available buffs
        List<Powerup> collection = PowerupManager.Instance.AvailableBuffs;

        // Instantiate a collectible powerup based on the given index
        int buffIndex = Random.Range(0, collection.Count);
        Powerup prefab = collection[buffIndex];
        Powerup newPowerup = Instantiate(prefab, this.transform.position, Quaternion.identity) as Powerup;

        return newPowerup;
    }

    public void Update() {
        // Adds a small bouncing animation to the brick after colliding with the ball
        // and not being instantly destroyed
        if (this.hit) {
            this.timeTracker = Time.time;
            Vector3 originalScale = this.transform.localScale;
            this.hitIterator = Mathf.Abs(Mathf.Sin(Mathf.PI * scaleSpeed * this.timeTracker) * 0.3f) + 1;

            float x = this.hitIterator;
            float y = this.hitIterator * 2;
            this.newScale = new Vector2(x, y);
            this.transform.localScale = newScale;

            if (Mathf.RoundToInt(this.timeTracker) % 3 == 0) {
                this.hit = false;
            }
        } else {
            this.timeTracker = 0.0f;
        }
    }

    public void Start() {
        this.hit = false;
    }

    public void SpawnDestroyEffect() {
        // Spawn the brick destroy effect at a given coordinate.
        // Gets its color from the brick itself
        Vector3 brickPos = gameObject.transform.position;
        Vector3 spawnPos = new Vector3(brickPos.x, brickPos.y, brickPos.z - 0.2f);
        GameObject effect = Instantiate(destroyEffect.gameObject, spawnPos, Quaternion.identity);

        MainModule mainModule = effect.GetComponent<ParticleSystem>().main;
        mainModule.startColor = this.brickRenderer.color;

        Destroy(effect, destroyEffect.main.startLifetime.constant);
    }

    // Used for spawning new bricks in the brick manager method.
    public void InitBrick(Transform container, Sprite sprite, Color color, int hitPoints) {
        this.transform.SetParent(container);
        this.brickRenderer.sprite = sprite;
        this.brickRenderer.color = color;
        this.hitPoints = hitPoints;
        this.oldHitPoints = hitPoints;
    }
}
