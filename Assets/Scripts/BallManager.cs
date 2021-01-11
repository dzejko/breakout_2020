using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour {
    #region Singleton
    private static BallManager instance;
    public static BallManager Instance => instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    #endregion
    // A list of ball entities currently active and instantiated
    public List<Ball> Balls { get; set; }
    // The first ball spawned in a given level and its RigidBody
    private Ball initialBall;
    private Rigidbody2D initialBallBody;

    // The ball prefab. Referenced across the file.
    [SerializeField]
    private Ball ballPrefab;

    // TODO: make this per-ball basis
    //private bool launched = false;          // Whether a given ball has been launched already
    private float ballOffsetY = 0.66f;      // Offset from the origin of the paddle.
    public float initialBallSpeed = 400;    // Sets the initial speed of a ball.

    private void Start() {
        Balls = new List<Ball>();
        initialBall = InitBall(initialBall);
        initialBallBody = initialBall.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        // If the game hasn't started yet
        if (!GameManager.Instance.HasGameStarted) {
            // Set the ball velocity to 0
            initialBallBody.velocity = Vector2.zero;

            // Stick it to the paddle
            initialBall.transform.position = StickToPaddle(true);

            // And don't launch unless the spacebar key has been striked.
            if (Input.GetKey(KeyCode.Space)) {
                Launch();
                GameManager.Instance.gameOverScreen.SetActive(false);
            }
        }
    }

    // TODO: will be used to do powerups, like multi-ball spawning.
    public void SpawnBalls(Vector3 position, uint count) {
        for (uint i = 0; i < count; ++i) {
            Ball spawnedBall = Instantiate(ballPrefab, position, Quaternion.identity) as Ball;

            Rigidbody2D spawnBallRb = spawnedBall.GetComponent<Rigidbody2D>();
            spawnBallRb.isKinematic = false;
            spawnBallRb.AddForce(new Vector2(0, initialBallSpeed));

            this.Balls.Add(spawnedBall);
        }
    }

    // Destroy all balls
    private void DestroyBalls() {
        foreach (var ball in this.Balls) {
            Destroy(ball.gameObject);
        }

        // Clear all references to remaining balls.
        this.Balls.Clear();

        // Initialize a new ball and add it to the list.
        initialBall = InitBall(initialBall);
        initialBallBody = initialBall.GetComponent<Rigidbody2D>();
    }

    public void ResetBalls() {
        DestroyBalls();
    }

    // Initializes a ball object.
    // "Initially" on level start, the game will instantiate a single ball
    // and attach it to the paddle. The TrailRenderer and Collisions will be disabled during that point
    // It's also going to add that ball to our ball list.
    private Ball InitBall(Ball ball) {
        Vector3 startingPos = StickToPaddle(false);

        ball = Instantiate(ballPrefab, startingPos, Quaternion.identity);
        ball.GetComponent<TrailRenderer>().enabled = false;
        ball.GetComponent<Collider2D>().enabled = false;

        Rigidbody2D ballBody = ball.GetComponent<Rigidbody2D>();
        ballBody.isKinematic = true;

        Balls.Add(ball);
        return ball;
    }

    // Returns the position of a ball, if it has to stick to the paddle.
    // Adds a random offset to the x position if you want a neater multi-ball effect
    private Vector3 StickToPaddle(bool spawned) {
        Vector3 paddlePos = Paddle.Instance.gameObject.transform.position;

        // Don't set random offset unless a new ball has been spawned.
        if (spawned) {
            return new Vector3(paddlePos.x, paddlePos.y + ballOffsetY);
        } else {
            float xPos = paddlePos.x;
            xPos *= Random.Range(-1.0f, 1.0f);
            return new Vector3(xPos, paddlePos.y + ballOffsetY);
        }
    }

    // Disable kinematic physics, enable renderering and colliding
    // And set the ball flying.
    private void Launch() {
        initialBall.GetComponent<TrailRenderer>().enabled = true;
        initialBall.GetComponent<Collider2D>().enabled = true;

        initialBallBody.isKinematic = false;
        Vector2 dir = new Vector2(Random.Range(-1.0f, 1.0f) * initialBallSpeed, initialBallSpeed);
        initialBallBody.AddForce(dir);

        GameManager.Instance.HasGameStarted = true;
    }
}
