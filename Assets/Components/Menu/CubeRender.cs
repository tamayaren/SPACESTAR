using System.Collections;
using UnityEngine;

public class CubeRender : MonoBehaviour
{
    [SerializeField] private Vector3 seed;
    [SerializeField] private float speed;

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(10f);
        Destroy(this.gameObject);
    }

    public void Initialize()
    {
        this.speed = Random.Range(1f, 3f);
        this.seed = new Vector3(
            Random.Range(-6000f, 6000f),
            Random.Range(-6000f, 6000f),
            Random.Range(-6000f, 6000f));
        
        this.transform.localScale = new Vector3(
            Random.Range(-3f, 3f),
            Random.Range(-3f, 3f),
            Random.Range(-3f, 3f));
        
        this.transform.localRotation = Quaternion.Euler(
            (new Vector3(
                Random.Range(-360f, 360f),
                Random.Range(-360f, 360f),
                Random.Range(-360f, 360f))
            ));

        StartCoroutine(Despawn());
    }
    private void Update()
    {
        this.transform.position += Vector3.up  * this.speed * Time.deltaTime;
        
        this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(
            (new Vector3(
                Mathf.Cos(Time.time + this.seed.x) * 720f, 
                Mathf.Sin(Time.time + this.seed.y) * 720f, 
                Mathf.Cos(Time.time + this.seed.z) * 720f)
            )), 5f *  Time.deltaTime);
    }
}
