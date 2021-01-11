public class TitanBall : Powerup {
    private float effectDuration = 10;

    protected override void ApplyEffect() {
        foreach (var ball in BallManager.Instance.Balls) {
            ball.StartTitanBall(this.effectDuration);
        }
    }
}