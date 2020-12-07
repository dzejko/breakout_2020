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

    // Stores level names and levels themselfs in Lists.
    // Easy to access from the level manager instance.
    //
    // The levels themselfs are stored internally as hashmaps
    // for each level name, there's a corresponding list of brick types and how they are laid out.
    public Dictionary<string, string> levels;

    // How many rows per level are we going to read?
    private readonly int linesPerLevel = 4;

    public (List<string>, List<string>) LoadLevelData() {
        // Currently read names and level data are stored in these variables.
        // They are not ordered or parsed yet.
        List<string> rawLevelNames = new List<string>();
        List<string> rawLevelData = new List<string>();

        // First, load the level data stored in the text file.
        StreamReader input = new StreamReader("Assets/Levels/levels.txt");

        // Read from the stream until it hits the end of the file
        while (!input.EndOfStream) {
            // Read it line-by-line. This makes it easy for parsing.
            string line = input.ReadLine();

            // Get the first character of the line, straight from the file.
            // and decide what to do.
            char firstChar = line.FirstOrDefault();
            switch (firstChar) {
                // Empty line.
                case '\0':
                case '\n':
                    break;
                // It's a comment
                case '#':
                    continue;
                // It's level name
                case '[':
                    // Get rid of square brackets surrounding the level name
                    string trimmed = line.Trim('[', ']');

                    // Add it to our list
                    rawLevelNames.Add(trimmed);
                    break;
                // It's something else
                default:
                    // Check if it's a brick type
                    var num = (int)char.GetNumericValue(firstChar);
                    // Check if it falls in the range o 0-9 when it comes to brick types
                    if (num >= 0 && num <= 9) {
                        // Yes it does. Add the entire line to our list.
                        rawLevelData.Add(line);
                    }
                    break;
            }
        }

        // Close the file stream after we're done reading from it
        input.Close();

        return (rawLevelNames, rawLevelData);
    }

    private Dictionary<string, string> ParseLevelData(List<string> names, List<string> rawData) {
        Dictionary<string, string> processedLevels = new Dictionary<string, string>();
        // Every level in the file is composed of 2 structures
        // The level name and the level definition.
        // Combining these together, if not careful, can lead to huge problems.
        // So, I settled on a constant number of lines per level.
        //
        // Iterate over all level names.
        for (int i = 0; i < names.Count; ++i) {
            // Fetch the current level from the list
            string currentLevelName = names[i];

            // Set the current data of the level to empty.
            string currentLevelData = string.Empty;

            // Since we're adding level names and level data at the same time
            // We need to be able to load 3 lines of data per any given name.
            // This is how it looks like at any given step
            //
            // for i = 0  => get lines 0, 1, 2 from tempLevelData
            // for i = 1  => get lines 3, 4, 5 from tempLevelData 
            // for i = 2  => get lines 6, 7, 8 from tempLevelData
            int offset = i * linesPerLevel;
            for (int j = offset; j < offset + linesPerLevel; ++j) {
                currentLevelData += rawData[j];
            }

            processedLevels.Add(currentLevelName, currentLevelData);
        }

        return processedLevels;
    }

    public void Start() {
        // Load all the level data
        (var names, var rawData) = LoadLevelData();

        // Parse and return it into the levels dictionary
        levels = ParseLevelData(names, rawData);
    }
}
