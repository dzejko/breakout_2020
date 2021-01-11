using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

class Clock : MonoBehaviour {
    // How much time spent in total in game?
    public float gameTime;

    // Times on various levels
    public List<int> levelTimers;

    // The real timer which ticks up.
    private Timer initialTimer;

    // How many seconds in a milliseconds?
    private const int interval = 1000;

    public void Start() {
        // Set the time.
        this.gameTime = Time.time;

        // Create and clear the list of level timers
        int levelCount = LevelManager.Instance.LevelNames.Count + 1;
        this.levelTimers = new List<int>(new int[levelCount]);
        this.ResetTimers();

        // Set the initial timer to not run.
        this.initialTimer = new Timer(new TimerCallback(AdvanceClock), null, Timeout.Infinite, Timeout.Infinite);
    }

    // Resets all level timers in the game
    private void ResetTimers() {
        for (int i = 0; i < this.levelTimers.Count; ++i) {
            this.levelTimers[i] = 0;
        }
    }

    // Timer callback, which advanced any given level's clock by 1.
    private void AdvanceClock(object state) {
        this.levelTimers[BrickManager.Instance.CurrentLevel] += 1;
    }

    // Stops the initial timer
    public void Stop() {
        this.initialTimer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    // and restarts it if necessary
    public void Restart() {
        this.initialTimer.Change(1000, 1000);
    }
}

public class InterfaceManager : MonoBehaviour {
    #region Singleton
    private static InterfaceManager instance;
    public static InterfaceManager Instance => instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    #endregion

    // The canvas on which are elements are rendered
    private Canvas canvas;

    // The UI Panels and their Text components
    private Text timer;
    private Text bestTime;
    private Text scoreText;
    private Text stageText;

    // Background image
    private SpriteRenderer backdrop;

    // Level name
    private string levelName;

    private bool clockRunning;

    // This variable keeps track of the score
    public int score;

    // These keep track of the current and starting time.
    private Clock gameClock;

    void Start() {
        // Create a new clock
        this.gameClock = this.gameObject.AddComponent<Clock>();

        // Grab the canvas
        this.canvas = GetComponentInChildren<Canvas>();

        // Grabs all the necessary text transformations.
        this.timer = canvas.transform.Find("StageTimer").GetComponentInChildren<Text>();
        this.scoreText = canvas.transform.Find("Score").GetComponentInChildren<Text>();
        this.stageText = canvas.transform.Find("StageName").GetComponentInChildren<Text>();
        this.backdrop = canvas.transform.Find("Background").GetComponentInChildren<SpriteRenderer>();

        // Disable the clock on init
        this.clockRunning = false;
    }

    public void Restart() {
        this.timer.text = "00:00";

        this.clockRunning = false;
    }

    void Update() {
        // Check if the game has started or not
        if (!GameManager.Instance.HasGameStarted) {
            // Stop the timer
            this.gameClock.Stop();
        } else {
            // Run the timer only if it hasn't been ran already
            // If we don't have this check, we end up calling a delayed callback
            // on every frame, and we never advance the timer.
            if (!this.clockRunning) {
                this.gameClock.Restart();

                this.clockRunning = true;
            }
        }

        // Set the score
        this.scoreText.text = score.ToString("000000");

        // Change stage text to a given level name and the level background.
        this.levelName = LevelManager.Instance.LevelNames[BrickManager.Instance.CurrentLevel];
        this.stageText.text = this.levelName;
        this.backdrop.sprite = Resources.Load($"backgrounds/{this.levelName}", typeof(Sprite)) as Sprite;

        // Set the timer clock
        int currentLevel = BrickManager.Instance.CurrentLevel;
        int time = this.gameClock.levelTimers[currentLevel];
        int seconds = (time % 60);
        int minutes = ((seconds / 60) % 60);

        // Format the timer text as {minutes:seconds} padded with 0's
        this.timer.text = string.Format("{0}:{1}", minutes.ToString("00"), seconds.ToString("00"));
    }
}
