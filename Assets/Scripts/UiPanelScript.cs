using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class is used to determine the font size of any given element
// by multiplying the number of letters in a given string by some constant value.
public class UiPanelScript : MonoBehaviour {
    private int textLength;
    private Text textComponent;

    void Start() {
        textComponent = GetComponent<Text>();
    }

    void Update() {
        // Get the number of characters in a string
        textLength = textComponent.text.Length;

        if (textLength <= 6 && textLength > 1) {
            // For any string smaller than 6 characters set it to 52.
            textComponent.fontSize = 52;
        } else if (textLength > 6) {
            // Scale anything longer dynamically
            textComponent.fontSize = textLength * 6;
        } else {
            textComponent.fontSize = 128;
            textComponent.fontStyle = FontStyle.Bold;
        }
    }
}