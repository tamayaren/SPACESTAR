using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    
    private GameObject player;
    private Entity playerHealth;

    private Transform target;
    private PlayerBlinking blink;

    private Entity entity;

    private bool canDamage = true;

    [SerializeField] private float snapMovement;
    private void Start()
    {
        this.agent = GetComponent<NavMeshAgent>();
        this.blink = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerBlinking>();
        this.entity = GetComponent<Entity>();
        
        this.blink?.OnBlink.AddListener(Move);
    }
    
    private Transform GetTarget() => GameObject.FindGameObjectWithTag("Player").transform;

    private IEnumerator DamagePlayer()
    {
        if (this.target)
        {
            this.canDamage = false;
            Entity playerEntity = this.target.GetComponent<Entity>();
            
            Damage.AttemptDamage(playerEntity, 10f);
            yield return new WaitForSeconds(1f);

            this.canDamage = true;
        }
    }
    
    private void Move()
    {
        Transform targetTransform = GetTarget();

        this.target = targetTransform;
        Vector3 dir = (this.transform.position - this.agent.steeringTarget).normalized;
        float angle = Vector3.Angle(targetTransform.forward, dir);

        RaycastHit hit;
        
        this.agent.SetDestination(targetTransform.position);

        dir.y += 1f;
        this.transform.position -= (dir)* this.snapMovement;
    }

    private void FixedUpdate()
    {
        if (this.target)
        {
            this.agent.SetDestination(this.target.position);
            
            
            if (Vector3.Distance(this.transform.position, this.target.position) <= this.snapMovement / 2f)
                if (this.canDamage && this.entity._health > 0f)
                    StartCoroutine(DamagePlayer());
        }
    }
}
