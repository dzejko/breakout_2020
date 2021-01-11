using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    #region Singleton
    private static LevelManager instance;

    public static LevelManager Instance => instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }
    #endregion

    public List<string> LevelNames;

    public void Start() {
        // Initializes all the predefined level names we have in our game.
        // TODO: figure out a better way to do this.
        LevelNames = new List<string>() {
            "Earth", "Moon", "Mercury"
        };
    }
}
