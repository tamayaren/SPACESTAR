using UnityEngine;

public class Damage : MonoBehaviour
{
    public static void AttemptDamage(Entity entity, float damage) => entity._health -= damage;
}
