using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    private PlayerBlinking blinking;
    private void Start()
    {
        this.blinking = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBlinking>();
        
        this.blinking.OnBlink.AddListener(() =>
        {
            foreach (Transform spawn in this.gameObject.GetComponentsInChildren<Transform>())
            {
                int a = Random.Range(1, 12);

                if (a == 1)
                    Instantiate(this.enemy, spawn.position, Quaternion.identity);
            }
        });
    }
}
