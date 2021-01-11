using System.Linq;

public class MultiBall : Powerup {
    protected override void ApplyEffect() {
        foreach (Ball ball in BallManager.Instance.Balls.ToList()) {
            BallManager.Instance.SpawnBalls(ball.gameObject.transform.position, 2);
        }
    }
}