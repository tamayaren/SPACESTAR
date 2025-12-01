using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EnemyRendering : MonoBehaviour
{
    [SerializeField] private GameObject cubes;
    [SerializeField] private AudioClip[] audios;
    private Entity entity;
    
    private float framerate = 8f;
    private float time;
    
    private AudioSource audioSource;

    private Vector3 positionVectorRandom;
    private Vector3 sizeVectorRandom;

    private Vector3 seed;

    private bool underAnimate = false;
    private void Start()
    {
        this.audioSource = this.gameObject.GetComponent<AudioSource>();
        this.entity = this.gameObject.GetComponent<Entity>();
        
        this.positionVectorRandom = new Vector3(
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f)
        );
        
        this.sizeVectorRandom = new Vector3(
            Random.Range(-2f, 2f), 
            Random.Range(-2f, 2f), 
            Random.Range(-2f, 2f));

        float lastHealth = this.entity._health;
        this.entity.HealthChanged.AddListener(health =>
        {
            Debug.Log($"{lastHealth} {health}");
            if (lastHealth > health)
                StartCoroutine(DamageSelf());

            if (health <= 0f)
                StartCoroutine(Die());
            
            lastHealth = health;
        });
    }

    private IEnumerator Die()
    {
        this.audioSource.pitch = Random.Range(0.3f, 2f);
        this.audioSource.PlayOneShot(this.audios[0], .2f);
        
        this.underAnimate = true;
        for (int i = 0; i < 30f; i++)
        {
            foreach (Transform obj in this.cubes.GetComponentsInChildren<Transform>())
            {
                this.seed = new Vector3(
                    Random.Range(-6000f, 6000f),
                    Random.Range(-6000f, 6000f),
                    Random.Range(-6000f, 6000f));
                
                obj.localPosition +=
                    (new Vector3(
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f)));
                
                obj.localRotation = Quaternion.Euler(
                    (new Vector3(
                        Random.Range(-360f, 360f),
                        Random.Range(-360f, 360f),
                        Random.Range(-360f, 360f))
                    ) * Time.deltaTime);

                obj.DOScale(Vector3.zero, 1f);
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        Destroy(this.gameObject);
        this.underAnimate = false;
    }

    private IEnumerator DamageSelf()
    {
        Debug.Log("OnDamage");
        
        this.audioSource.pitch = Random.Range(0.3f, 2f);
        this.audioSource.PlayOneShot(this.audios[1], .2f);
        this.underAnimate = true;
        for (int i = 0; i < 15f; i++)
        {
            foreach (Transform obj in this.cubes.GetComponentsInChildren<Transform>())
            {
                this.seed = new Vector3(
                    Random.Range(-6000f, 6000f),
                    Random.Range(-6000f, 6000f),
                    Random.Range(-6000f, 6000f));
                
                obj.localScale = 
                    (new Vector3(Random.Range(-2f, 2f),
                        Random.Range(-2f, 2f),
                        Random.Range(-2f, 2f)));
                
                obj.localRotation = Quaternion.Euler(
                    (new Vector3(
                        Random.Range(-360f, 360f),
                        Random.Range(-360f, 360f),
                        Random.Range(-360f, 360f))
                    ) * Time.deltaTime);
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        this.underAnimate = false;
    }
    
    private void Animate()
    {
        if (this.underAnimate) return;
        
        this.time += Time.deltaTime;
        if (this.time >= 1 / this.framerate)
        {
            foreach (Transform obj in this.cubes.GetComponentsInChildren<Transform>())
            {
                this.seed = new Vector3(
                    Random.Range(-6000f, 6000f),
                    Random.Range(-6000f, 6000f),
                    Random.Range(-6000f, 6000f));
                
                obj.localPosition +=
                    (new Vector3(
                        Mathf.Cos(Time.time + this.seed.x) * this.positionVectorRandom.x, 
                        Mathf.Sin(Time.time + this.seed.y) * this.positionVectorRandom.y, 
                        Mathf.Cos(Time.time + this.seed.z) * this.positionVectorRandom.z) * Time.deltaTime);
                
                obj.localScale = 
                    (Vector3.one + new Vector3(
                        Mathf.Cos(Time.time + this.seed.x) * this.sizeVectorRandom.x, 
                        Mathf.Sin(Time.time + this.seed.y) * this.sizeVectorRandom.y, 
                        Mathf.Cos(Time.time + this.seed.z) * this.sizeVectorRandom.z) * Time.deltaTime);
                
                obj.localRotation = Quaternion.Euler(
                    (new Vector3(
                        Mathf.Cos(Time.time + this.seed.x) * 720f, 
                        Mathf.Sin(Time.time + this.seed.y) * 720f, 
                        Mathf.Cos(Time.time + this.seed.z) * 720f)
                    ) * Time.deltaTime);
            }
            
            this.time = 0f;
        }
    }
    
    private void FixedUpdate()
    {
        Vector3 inView = Camera.main.WorldToViewportPoint(this.cubes.transform.position);
        
        if (inView is { x: >= 0f and <= 1f, y: >= 0f and <= 1f }) Animate();
    }
}
