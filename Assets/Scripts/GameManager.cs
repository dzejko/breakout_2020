using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    // Has the game started yet?
    public bool HasGameStarted { get; set; }

    // How many lives?
    public int Lives { get; set; }

    // What happens when we lost a life?
    public static event UnityAction<int> OnLiveLost;

    public GameObject gameOverScreen;
    // public GameObject victoryScreen;

    void Start() {
        Lives = 1;

        Brick.OnBrickDestruction += OnBrickDestruction;
        Ball.OnBallDeath += OnBallDeath;
    }

    private void OnBrickDestruction(Brick obj) {
        // If there are no remaining bricks, reset the game and advance to the next level.
        if (BrickManager.Instance.RemainingBricks.Count <= 0) {
            this.SoftRestart();
            BrickManager.Instance.LoadNextLevel();
        }
    }

    public void ShowVictoryScreen() { }

    private void OnBallDeath(Ball obj) {
        this.Lives--;
        // If there are no balls present in the level, the game has to be reset.
        if (BallManager.Instance.Balls.Count <= 0) {
            this.gameOverScreen.SetActive(true);
            this.SoftRestart();
            BrickManager.Instance.LoadLevel(BrickManager.Instance.CurrentLevel);
        } else {
            // Reset everything for now.
            this.SoftRestart();
            BrickManager.Instance.LoadLevel(BrickManager.Instance.CurrentLevel);
        }
    }

    public void RestartGame() {
        this.SoftRestart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SoftRestart() {
        InterfaceManager.Instance.Restart();
        GameManager.Instance.HasGameStarted = false;
        BallManager.Instance.ResetBalls();
    }
}
