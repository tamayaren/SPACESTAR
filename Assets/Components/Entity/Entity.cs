using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    public float maxHealthIndex;

    private float health;
    private float maxHealth;

    public float _health
    {
        get => this.health;
        set
        {
            this.health = value;
            
            this.HealthChanged.Invoke(this.health);
            if (this.health <= 0f)
                this.Dead.Invoke();
        }
    }

    public float _maxHealth
    {
        get => this.maxHealth;
        set => this.maxHealth = value;
    }

    public UnityEvent Dead = new UnityEvent();
    public UnityEvent<float> HealthChanged = new UnityEvent<float>();
    
    private void Start() =>
        this.health = this.maxHealthIndex;
}
