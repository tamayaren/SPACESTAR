using System.Collections;
using UnityEngine;

public class WeaponShoot : MonoBehaviour
{
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private Camera camera;
    [SerializeField] private ParticleSystem shootFX;

    [SerializeField] private AudioClip[] shoots;
    [SerializeField] private GameObject flashlight;
    private InputManager inputManager;
    private readonly float cooldown = .35f;
    private bool onCooldown = false;

    private AudioSource audioSource;
    private void Start()
    {
        this.inputManager = GetComponent<InputManager>();
        this.audioSource = GetComponent<AudioSource>();
        
        this.inputManager.onShoot.AddListener(Shoot);
        this.camera = Camera.main;

        this.inputManager.flashlightAction.performed += context =>
        {
            this.flashlight.SetActive(!this.flashlight.activeInHierarchy);
        };
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(this.cooldown);
        this.onCooldown = false;
    }

    private void Shoot()
    {
        if (this.onCooldown) return;
        RaycastHit result;
        Physics.Raycast(this.camera.transform.position, this.camera.transform.forward, out result, Mathf.Infinity, LayerMask.GetMask("Entity") | LayerMask.GetMask("Interactable") | LayerMask.GetMask("Win"));

        Debug.DrawRay(this.camera.transform.position, this.camera.transform.forward * 64f, Color.red);
        
        Debug.Log("Delta!");
        if (result.collider != null)
        {
            float dist = Vector3.Distance(this.camera.transform.position, result.point);
            Debug.Log(result.collider.gameObject.layer);
            if (result.collider.gameObject.layer == LayerMask.NameToLayer("Win") && dist < 4f)
            {
                OnWin win = result.collider.gameObject.GetComponent<OnWin>();
                win.EnterMode();
                return;
            }
            
            if (result.collider.gameObject.layer == LayerMask.NameToLayer("Interactable") && dist < 4f)
            {
                TerminalInteraction terminalInteraction = result.collider.gameObject.GetComponent<TerminalInteraction>();
                
                UITerminal.instance.Display(terminalInteraction.message);
                Debug.Log("Delta");
                return;
            }
            
            if (result.collider.gameObject.CompareTag("Enemy"))
            {
                Entity enemyEntity = result.collider.gameObject.GetComponent<Entity>();
                
                Debug.Log("Destroy");
                Damage.AttemptDamage(enemyEntity, 25f);
            }   
        }
        
        this.weaponAnimator.SetTrigger("Shoot");

        AudioClip audio = this.shoots[(int)Random.Range(0, 1)];
        this.audioSource.pitch = Random.Range(0.9f, 1.1f);
        this.audioSource.PlayOneShot(audio, Random.Range(0.8f, 1.2f));
        
        this.shootFX.Play();
        Debug.Log("Ah1");
        this.onCooldown = true;
        StartCoroutine(Cooldown());
    }
}
