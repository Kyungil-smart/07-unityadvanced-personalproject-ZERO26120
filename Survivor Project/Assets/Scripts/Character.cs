using UnityEngine;

public class Character : MonoBehaviour
{
    public static float Speed {
        get {
            switch (GameManager.instance.playerId) {
                case 0: return 1.1f;
                case 1: return 1.3f;
                case 2: return 0.9f;
                default: return 1f;
            }
        }
    }

    public static float MaxHealth {
        get {
            switch (GameManager.instance.playerId) {
                case 0: return 150f;
                case 1: return 80f;
                default: return 100f;
            }
        }
    }

    public static float DamageReduction => GameManager.instance.playerId == 0 ? 0.8f : 1f;
    public static float WeaponRate => GameManager.instance.playerId == 1 ? 0.9f : 1f;
    public static float Damage {
        get {
            switch (GameManager.instance.playerId) {
                case 2: return 1.2f;
                case 3: return 0.9f;
                default: return 1f;
            }
        }
    }

    public static float Count => GameManager.instance.playerId == 3 ? 1 : 0;
    public static float Scale => GameManager.instance.playerId == 2 ? 1.2f : 1f;
    public static float WeaponSpeed => 1f; 
}