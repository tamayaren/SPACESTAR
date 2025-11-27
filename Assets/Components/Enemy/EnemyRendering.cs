using UnityEngine;

public class EnemyRendering : MonoBehaviour
{
    [SerializeField] private GameObject cubes;

    private float framerate = 8f;
    private float time;

    private Vector3 positionVectorRandom;
    private Vector3 sizeVectorRandom;

    private Vector3 seed;

    private void Start()
    {
        this.positionVectorRandom = new Vector3(
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f)
        );
        
        this.sizeVectorRandom = new Vector3(
            Random.Range(-2f, 2f), 
            Random.Range(-2f, 2f), 
            Random.Range(-2f, 2f));

    }
    
    private void Animate()
    {
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
                
                obj.localScale += 
                    (new Vector3(
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
