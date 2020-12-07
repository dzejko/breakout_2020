using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceScript : MonoBehaviour {
    #region Singleton
    private static InterfaceScript instance;
    public static InterfaceScript Instance => instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    #endregion

    private Canvas canvas;
    private Text timer;
    private Text bestTime;
    private Text scoreText;

    // This variable keeps track of the score
    public int score;

    // These keep track of the current and starting time.
    private float currentTime;
    private float startTime;

    void Start() {
        canvas = GetComponentInChildren<Canvas>();
        timer = canvas.transform.Find("StageTimer").GetComponentInChildren<Text>();
        scoreText = canvas.transform.Find("Score").GetComponentInChildren<Text>();

        startTime = Time.time;
        currentTime = startTime;
    }

    void Update() {
        // Check if the game has started or not
        if (GameManager.Instance.HasGameStarted) {
            // if it has, advance the local timer
            currentTime += Time.deltaTime;

            // set the score
            scoreText.text = string.Format("{0}", score);
        }

        // Convert the local timer to human readable (Minutes:Seconds)...
        string seconds = Mathf.RoundToInt(currentTime % 60).ToString("00");
        string minutes = Mathf.RoundToInt((currentTime % 60) / 60).ToString("00");

        // ...and set the Interface Timer's text field to that format.
        timer.text = string.Format("{0}:{1}", minutes, seconds);
    }
}
