using UnityEngine;

public class PlayerHealthRegen : MonoBehaviour
{
    [SerializeField] private Entity entity;

    private AudioSource audioSource;
    [SerializeField] private AudioClip clip;

    private float elapsed;
    private float regenTime = .3f;

    private float lastHealth;
    
    private void Start()
    {
        this.entity = GetComponent<Entity>();
        this.audioSource = this.gameObject.GetComponent<AudioSource>();

        this.lastHealth = this.entity._health; 
        this.entity.HealthChanged.AddListener(health =>
        {
            if (this.lastHealth > health)
            {
                this.audioSource.pitch = Random.Range(0.8f, 1.3f);
                this.audioSource.PlayOneShot(this.clip, Random.Range(0.2f, .3f));
            }
            
            this.lastHealth = health;
        });
    }

    private void Update()
    {
        if (this.entity._health <= 0f) return;
        if (this.entity._health >= this.entity._maxHealth) return;

        if (this.elapsed >= this.regenTime)
        {
            this.entity._health = Mathf.Clamp(this.entity._health + 1f, 0f, this.entity._maxHealth);
            this.elapsed = 0f; 
        }
        this.elapsed += Time.deltaTime;
    }
}
