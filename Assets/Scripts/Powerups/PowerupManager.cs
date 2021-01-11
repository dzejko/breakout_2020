using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour {
    #region Singleton
    private static PowerupManager instance;
    
    public static PowerupManager Instance => instance;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }
    #endregion

    public List<Powerup> AvailableBuffs;

    [Range(0, 100)]
    public float BuffChance;
}