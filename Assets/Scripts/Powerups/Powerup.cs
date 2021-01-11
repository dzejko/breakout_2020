using UnityEngine;

public abstract class Powerup : MonoBehaviour {
    // Check what the powerup collided with
    private void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Paddle") {
            // Apply the effect if it's player's paddle
            this.ApplyEffect();
        }

        if (col.tag == "Paddle" || col.tag == "DeathWall") {
            // Instantly destroy after applying the effect or colliding
            // with the deathwall.
            Destroy(this.gameObject);
        }
    }

    protected abstract void ApplyEffect();
}