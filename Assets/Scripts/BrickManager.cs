using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour {
    #region Singleton
    private static BrickManager instance;

    public static BrickManager Instance => instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }
    #endregion

    // Raw list of data in our levels.
    private List<int[,]> LevelsData { get; set; }

    // How many rows and columns of bricks at most we can generate
    // in the playable area
    public readonly int maxRows = 4;
    public readonly int maxCols = 8;

    // think of this object as a way to "contain" all the bricks
    // on one plane 
    private GameObject bricksContainer;

    // Where bricks should initially spawn.
    // Think of it as a coordinate where the first brick spawn
    // from the top-left
    private readonly float initialBricksSpawnX = -4.2f;
    private readonly float initialBricksSpawnY = 4f;

    // How much to shift them by after each generation.
    private readonly float shiftAmountY = 0.8f;
    private readonly float shiftAmountX = 1.2f;

    // This is the brick prefab used for generating new bricks
    public Brick brickPrefab;

    // We cycle sprites and colors for each type of a brick.
    public Sprite[] Sprites;
    public Color[] BrickColors;

    // How many bricks are remaining and how many are initialized.
    public List<Brick> RemainingBricks { get; set; }
    public int InitialBricksCount { get; set; }

    // Current level index.
    public int CurrentLevel;

    // Current number of levels;
    public int LevelCount;

    private void Start() {
        // Creates ther new container, loads the data and generates the bricks. 
        this.CurrentLevel = 0;
        this.LevelCount = 0;
        this.bricksContainer = new GameObject("BricksContainer");
        this.LevelsData = this.LoadData();
        this.GenerateBricks();
    }

    // Loads the next level.
    public void LoadNextLevel() {
        this.CurrentLevel++;

        // If the increment level index is bigger than the total count of levels in our game
        // we know, that the player finished our game. Show the victory screen.
        // Otherwise advance to the next level.
        if (this.CurrentLevel >= this.LevelsData.Count) {
            GameManager.Instance.ShowVictoryScreen();
        } else {
            this.LoadLevel(this.CurrentLevel);
        }
    }

    // Loads a given level.
    public void LoadLevel(int level) {
        this.CurrentLevel = level;
        this.ClearRemainingBricks();
        this.GenerateBricks();
    }

    // Clears all the remaining bricks in the level.
    private void ClearRemainingBricks() {
        foreach (Brick brick in this.RemainingBricks.ToList()) {
            Destroy(brick.gameObject);
        }
    }

    public void GenerateBricks() {
        // This tracks how many bricks are remaining.
        // If this gets to zero, after all bricks are destroyed, the game will advance to the next level.
        this.RemainingBricks = new List<Brick>();

        // Load a 2D array of bricks from level data.
        int[,] currentLevelData = this.LevelsData[this.CurrentLevel];

        // Set the initial spawn coordinates.
        float currentSpawnX = initialBricksSpawnX;
        float currentSpawnY = initialBricksSpawnY;
        float zShift = 0;

        // Generate a 2D map of bricks
        for (int row = 0; row < this.maxRows; row++) {
            for (int col = 0; col < this.maxCols; col++) {
                int brickType = currentLevelData[row, col];

                // Skip empty spaces
                if (brickType > 0) {
                    // Instantiate the new bricks.
                    Vector3 brickPos = new Vector3(currentSpawnX, currentSpawnY, 0.0f - zShift);
                    Brick newBrick = Instantiate(brickPrefab, brickPos, Quaternion.identity) as Brick;
                    newBrick.InitBrick(bricksContainer.transform, this.Sprites[brickType - 1], this.BrickColors[brickType - 1], brickType);

                    // Add the newly generated brick to the list of remaining bricks
                    this.RemainingBricks.Add(newBrick);
                    zShift += 0.0001f;

                }
                // Shift to the right
                currentSpawnX += shiftAmountX;

                // If the next column is the max that can fit in the playable area
                // instead of shifting further, just reset the X position to the start.
                if (col + 1 == this.maxCols) {
                    currentSpawnX = initialBricksSpawnX;
                }
            }
            // Shift to the bottom
            currentSpawnY -= shiftAmountY;
        }

        this.InitialBricksCount = this.RemainingBricks.Count;
    }

    public List<int[,]> LoadData() {
        if (this.LevelCount >= 8) {
            // Start procedurally processing the levels
            List<int[,]> levelData = new List<int[,]>();

            // Create a new 2D array that will represent our level.
            int[,] currentLevel = new int[this.maxRows, this.maxCols];
            int currentRow = 0;

            // Generate all the rows
            List<string> rows = new List<string>();
            string rawLine = string.Empty;
            for (int i = 0; i <= this.maxRows; ++i) {
                for (int j = 0; j < this.maxCols; ++j) {

                    // Generate a random brick type
                    int brickType = Random.Range(0, 7);
                    rawLine += $"{brickType},";

                    if (j + 1 == this.maxCols) {
                        rawLine += $"{brickType}";
                    }
                }

                // Add '--' on the last row to signify loading the next level
                if (i == this.maxRows) {
                    rawLine = "--";
                }

                rows.Add(rawLine);
                rawLine = string.Empty;
            }

            var rowsProcessed = rows.ToArray();

            // Iterate over the rows and columns.
            for (int row = 0; row < rowsProcessed.Length; ++row) {
                string line = rowsProcessed[row];

                // If our line *does not* start with `--`, then we're still loading data from our current level.
                if (line.IndexOf("--") == -1) {
                    // Split the line again by using commas.
                    string[] bricks = line.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                    // Iterate over all the columns and parse the characters as integers.
                    for (int col = 0; col < bricks.Length; ++col) {
                        currentLevel[currentRow, col] = int.Parse(bricks[col]);
                    }

                    currentRow++;
                } else {
                    // We're loading in a new level.
                    // Clear the row, add all the data to the list and reinitialize the array.
                    currentRow = 0;
                    levelData.Add(currentLevel);
                    currentLevel = new int[this.maxRows, this.maxCols];
                    this.LevelCount++;
                }
            }

            return levelData;
        } else {
            // Load raw level data from the `Resources` folder.
            TextAsset rawData = Resources.Load("levels") as TextAsset;

            // Split it to rows by using existing newlines and initialize an empty list.
            string[] rows = rawData.text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
            List<int[,]> levelData = new List<int[,]>();

            // Create a new 2D array that will represent our level.
            int[,] currentLevel = new int[this.maxRows, this.maxCols];
            int currentRow = 0;

            // Iterate over the rows and columns.
            for (int row = 0; row < rows.Length; ++row) {
                string line = rows[row];

                // If our line *does not* start with `--`, then we're still loading data from our current level.
                if (line.IndexOf("--") == -1) {
                    // Split the line again by using commas.
                    string[] bricks = line.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                    // Iterate over all the columns and parse the characters as integers.
                    for (int col = 0; col < bricks.Length; ++col) {
                        currentLevel[currentRow, col] = int.Parse(bricks[col]);
                    }

                    currentRow++;
                } else {
                    // We're loading in a new level.
                    // Clear the row, add all the data to the list and reinitialize the array.
                    currentRow = 0;
                    levelData.Add(currentLevel);
                    currentLevel = new int[this.maxRows, this.maxCols];
                    this.LevelCount++;
                }
            }

            return levelData;
        }
    }
}
