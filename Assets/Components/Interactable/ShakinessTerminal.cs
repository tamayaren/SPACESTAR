using UnityEngine;

public class ShakinessTerminal : MonoBehaviour
{
    private Vector3 center;

    private void Start()
    {
        this.center = this.transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 inView = Camera.main.WorldToViewportPoint(this.transform.position);
        
        if (inView is { x: >= 0f and <= 1f, y: >= 0f and <= 1f })
        {
            this.transform.position = this.center +
                                      new Vector3(
                                          Random.Range(-.1f, .1f),
                                          Random.Range(-.1f, .1f),
                                          Random.Range(-.1f, .1f));
        }
    }
}
