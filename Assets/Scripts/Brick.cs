using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.ParticleSystem;

public class Brick : MonoBehaviour {
    private SpriteRenderer brickRenderer;
    private BoxCollider2D brickBody;
    public ParticleSystem destroyEffect;
    public static event UnityAction<Brick> OnBrickDestruction;
    public int hitPoints = 1;

    private void Awake() {
        this.brickRenderer = GetComponent<SpriteRenderer>();
        this.brickBody = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D col) {
        bool instaKill = false;

        // If you collided with the ball, take damage.
        if (col.collider.tag == "Ball") {
            this.TakeDamage(instaKill);
        }
    }

    private void TakeDamage(bool instaKill) {
        // Take away hitpoints
        this.hitPoints--;

        if (this.hitPoints <= 0 || instaKill) {
            BrickManager.Instance.RemainingBricks.Remove(this);
            OnBrickDestruction?.Invoke(this);

            OnBrickDestroy();
            SpawnDestroyEffect();
            Destroy(this.gameObject);
        }
    }

    private void OnBrickDestroy() {
        // TODO: implement powerups
        InterfaceScript.Instance.score += 20;
    }

    public void SpawnDestroyEffect() {
        // Spawn the brick destroy effect at a given coordinate.
        // Gets its color from the brick itself
        Vector3 brickPos = gameObject.transform.position;
        Vector3 spawnPos = new Vector3(brickPos.x, brickPos.y, brickPos.z - 0.2f);
        GameObject effect = Instantiate(destroyEffect.gameObject, spawnPos, Quaternion.identity);

        MainModule mainModule = effect.GetComponent<ParticleSystem>().main;
        mainModule.startColor = this.brickRenderer.color;

        Destroy(effect, destroyEffect.main.startLifetime.constant);
    }

    public void InitBrick(Transform container, Sprite sprite, Color color, int hitPoints) {
        this.transform.SetParent(container);
        this.brickRenderer.sprite = sprite;
        this.brickRenderer.color = color;
        this.hitPoints = hitPoints;
    }
}
