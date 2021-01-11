using UnityEngine;

public class HyperBall : Powerup {
    private float effectDuration = 10;

    protected override void ApplyEffect() {
        foreach (Ball ball in BallManager.Instance.Balls) {
            ball.StartHyperBall(this.effectDuration);
        }
    }
}