using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// This manager is going to "manage" all the entities, effects and game logic for the entire scene.
public class GameManager : MonoBehaviour {
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance => instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    #endregion

    public bool HasGameStarted { get; set; }
    public int Lives { get; set; }

    public GameObject gameOverScreen;
    public GameObject victoryScreen;

    void Start() {
        Lives = 1;
        // TODO: BallManager

        Brick.OnBrickDestruction += OnBrickDestruction;
        Ball.OnBallDeath += OnBallDeath;
    }

    private void OnBrickDestruction(Brick obj) {
        // If there are no remaining bricks, reset the game and advance to the next level.
        if (BrickManager.Instance.RemainingBricks.Count <= 0) {
            GameManager.Instance.HasGameStarted = false;
            BallManager.Instance.ResetBalls();
        }
    }

    private void OnBallDeath(Ball obj) {
        // If there are no balls present in the level, the game has to be reset.
        if (BallManager.Instance.Balls.Count <= 0) {
            this.Lives--;

            if (this.Lives < 1) {
                GameManager.Instance.HasGameStarted = false;
                gameOverScreen.SetActive(true);
            } else {
                // TODO: Are we going to have more than 1 life?
            }
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
