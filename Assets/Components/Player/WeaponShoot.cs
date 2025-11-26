using System.Collections;
using UnityEngine;

public class WeaponShoot : MonoBehaviour
{
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private Camera camera;
    [SerializeField] private ParticleSystem shootFX;
    
    private InputManager inputManager;
    private readonly float cooldown = .35f;
    private bool onCooldown = false;
    
    private void Start()
    {
        this.inputManager = GetComponent<InputManager>();
        
        this.inputManager.onShoot.AddListener(Shoot);
        this.camera = Camera.main;
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(this.cooldown);
        this.onCooldown = false;
    }

    private void Shoot()
    {
        if (this.onCooldown) return;
        this.weaponAnimator.SetTrigger("Shoot");

        this.shootFX.Play();
        RaycastHit result;
        Physics.Raycast(this.camera.transform.position, this.camera.transform.forward, out result, Mathf.Infinity, LayerMask.GetMask("Enemy"));
        
        Debug.DrawRay(this.camera.transform.position, this.camera.transform.forward * 64f, Color.red);
        this.onCooldown = true;
        StartCoroutine(Cooldown());
    }
}
