using UnityEngine;

public class CubeSpawn : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;

    private int rate = 6;
    private float elapsed;

    private float rateTime = .1f;

    [SerializeField] private Vector3 radius;
    public void Update()
    {
        if (this.elapsed >= this.rateTime)
        {
            for (int i = 0; i < this.rate; i++)
            {
                GameObject cube = Instantiate(this.cubePrefab);
                
                cube.transform.position = this.transform.position + new Vector3(
                    Random.Range(-this.radius.x, this.radius.x), Random.Range(-this.radius.y, this.radius.y), Random.Range(-this.radius.z, this.radius.z)
                    );
                
                cube.GetComponent<CubeRender>().Initialize();
            }
            
            this.rate = Random.Range(7, 12);
            this.rateTime = Random.Range(.3f, 1f);
            this.elapsed = 0f;
        }
        this.elapsed += Time.deltaTime;
    }
}
