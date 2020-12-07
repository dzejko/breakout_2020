using System.Collections;
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

    // How many rows and columns of bricks at most we can generate
    // in the playable area
    private readonly int maxRows = 4;
    private readonly int maxCols = 8;

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
    public Sprite[] Sprites;
    public Color[] BrickColors;
    public List<Brick> RemainingBricks { get; set; }
    public int InitialBricksCount { get; set; }

    private void Start() {
        // TODO: Load colors from map
        this.BrickColors = new Color[1];


        this.BrickColors[0] = new Color(0f, 1f, 1f, 1f);
        this.bricksContainer = new GameObject("BricksContainer");
        this.GenerateBricks();
    }

    public void GenerateBricks() {
        // This tracks how many bricks are remaining.
        // If this gets to zero, after all bricks are destroyed, the game will advance to the next level.
        this.RemainingBricks = new List<Brick>();
        float currentSpawnX = initialBricksSpawnX;
        float currentSpawnY = initialBricksSpawnY;
        float zShift = 0;

        // Generate a 2D map of bricks
        // TODO: add levels
        for (int row = 0; row < this.maxRows; row++) {
            for (int col = 0; col < this.maxCols; col++) {

                // Here check for bricktype
                Vector3 brickPos = new Vector3(currentSpawnX, currentSpawnY, 0.0f - zShift);
                Brick newBrick = Instantiate(brickPrefab, brickPos, Quaternion.identity) as Brick;
                newBrick.InitBrick(bricksContainer.transform, this.Sprites[0], this.BrickColors[0], 1);

                // Add the newly generated brick to the list of remaining bricks
                this.RemainingBricks.Add(newBrick);
                zShift += 0.0001f;

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
        // OnLevelLoad
    }


}
