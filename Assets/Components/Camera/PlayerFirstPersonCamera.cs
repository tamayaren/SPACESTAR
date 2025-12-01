using UnityEngine;
using UnityEngine.Serialization;

public class PlayerFirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform cameraHeadX;
    [SerializeField] private Transform cameraHeadY;
    [SerializeField] private Transform flashlight;
    [SerializeField] private Transform viewmodel;
    [SerializeField] private AudioClip[] footsteps;
    private Entity entity;
    
    private CharacterController characterController;
    private MeshRenderer meshRenderer;
    private MovementMain movementMain;
    
    private AudioSource audioSource;
    private InputManager inputManager;

    public bool isActive = true;
    public float sensitivity = 2.5f;
    public Vector2 cameraRotation;
    public float pitchLock = 90f;

    public Vector3 bobbingOffset;
    public Vector3 bobbingRotation;
    public Vector3 viewmodelOffset;
    public Vector3 viewmodelCenter;

    public bool cameraLock = true;

    public bool canStep = false;
    private Vector3 center;
    private void Start()
    {
        this.entity = GetComponent<Entity>();
        this.inputManager = GetComponent<InputManager>();
        this.meshRenderer = GetComponent<MeshRenderer>();
        this.characterController = GetComponent<CharacterController>();
        this.audioSource = GetComponent<AudioSource>();
        this.movementMain = GetComponent<MovementMain>();
        this.playerHead = GameObject.FindGameObjectWithTag("PlayerHead").transform;
        
        this.cameraHeadX = GameObject.FindGameObjectWithTag("CameraHead").transform;
        this.cameraHeadY = this.cameraHeadX?.Find("Y");
        
        this.center = this.playerHead.localPosition;
        this.viewmodelCenter = this.viewmodel.localPosition;
    }

    private void Bobbing()
    {
        Vector3 velocity = this.characterController.velocity;

        if (this.characterController.isGrounded)
        {
            if (velocity.sqrMagnitude < .5f)
            {
                this.bobbingOffset = Vector3.Lerp(this.bobbingOffset, new Vector3(
                    0f,
                    Mathf.Sin(Time.time * 4f) * .06f,
                    0f
                    ), 2f * Time.deltaTime);

                this.viewmodelOffset = Vector3.Lerp(this.viewmodelOffset, new Vector3(
                    Mathf.Sin(Time.time * 2f) * .02f,
                    Mathf.Sin(Time.time * 3f) * .01f,
                    Mathf.Sin(Time.time * 2f) * .015f
                ), 2f * Time.deltaTime);
                
                this.bobbingRotation = Vector3.Lerp(
                    this.bobbingRotation,

                    new Vector3(
                        Mathf.Sin(Time.time * 2f) * (.3f + Mathf.PerlinNoise1D(Time.time * .2f)), 0f, Mathf.PerlinNoise1D(Time.time * .3f) * 2f), 4f * Time.deltaTime);
            }
            else
            {
                float strafeDir = this.inputManager.movementInput.x * velocity.sqrMagnitude;

                Vector3 xyzSpeed = new Vector3(3f, 12f, 6f);
                Vector3 xyzRotation = new Vector3(12f, 0f, 6f);
                if (this.movementMain.isSprinting)
                {
                    xyzSpeed = new Vector3(4f, 16f, 8f);
                    xyzRotation = new Vector3(18f, 0f, 8f);
                }
                
                this.bobbingOffset = Vector3.Lerp(this.bobbingOffset, new Vector3(
                    Mathf.Cos((Time.time + 12f) * xyzSpeed.x) * .2f,
                    Mathf.Cos(Time.time * xyzSpeed.y) * .04f,
                    Mathf.Sin((Time.time + 6f) * xyzSpeed.z) * .7f
                ), 2f * Time.deltaTime) * Mathf.Clamp(velocity.magnitude/this.movementMain.speed, .1f, 1f);

                if (this.viewmodelOffset.z < -0.05f || this.viewmodelOffset.z > 0.05f)
                {
                    if (this.canStep)
                    {
                        Debug.Log("Stepping");
                        
                        AudioClip clip = this.footsteps[Random.Range(0, this.footsteps.Length-1)];
                        this.audioSource.pitch = Random.Range(0.9f, 1.1f);
                        this.audioSource.PlayOneShot(clip, Random.Range(.4f, .7f));
                        
                        this.canStep = false;
                    }
                }
                else
                {
                    this.canStep = true;
                }
                this.viewmodelOffset = Vector3.Lerp(this.viewmodelOffset, 
                    new Vector3(
                        0f,
                        Mathf.Cos(Time.time * xyzSpeed.y * .2f) * .05f,
                        Mathf.Sin((Time.time + 6f) * xyzSpeed.z * 1.25f) * -.1f),
                    .2f);
                
                this.bobbingRotation = Vector3.Lerp(this.bobbingRotation, 
                    new Vector3(
                        Mathf.Cos(Time.time * xyzRotation.x) * 1f, 
                        0f, Mathf.Sin((Time.time + 6f) * xyzRotation.z) * -1f + -Mathf.Clamp(strafeDir, -5f, 5f)
                        ), 4f * Time.deltaTime);
            }
            
            this.bobbingRotation = Vector3.Lerp(
                this.bobbingRotation, 
                new Vector3(0f, 0f, this.inputManager.lookInput.x * -1f)
                , 1f * Time.deltaTime);
        }
        
        this.playerHead.localPosition = this.center + this.bobbingOffset;
        this.playerHead.localRotation = Quaternion.Euler(this.bobbingRotation);

        this.viewmodel.localPosition = this.viewmodelCenter + this.viewmodelOffset;
        this.viewmodel.localRotation = Quaternion.Euler((this.bobbingRotation + new Vector3(0f, 2f - this.bobbingRotation.z, 0f)));
        
        this.flashlight.localPosition = new Vector3(.2f, -.5f, 1f) - (new Vector3(this.bobbingOffset.x, this.bobbingOffset.y, 0f) * 2f);
        this.flashlight.localRotation = Quaternion.Euler(-(this.bobbingRotation + new Vector3(0f, 2f - this.bobbingRotation.z, 0f)) * 2f);
    }

    private void Look()
    {
        if (this.entity._health <= 0f) return;
        this.meshRenderer.enabled = false;
        if (this.cameraLock)
            Cursor.lockState = CursorLockMode.Locked;
        Vector2 lookInput = this.inputManager.lookInput * this.sensitivity;
        
        this.cameraHeadX.position = this.playerHead.position;
        
        Vector3 angles = this.cameraHeadX.localEulerAngles;
        angles.x = this.playerHead.localEulerAngles.x;
        angles.z = this.playerHead.localEulerAngles.z;
        this.cameraHeadX.localEulerAngles = angles;
        
        this.cameraHeadX.Rotate(Vector3.up * (lookInput.x * Time.deltaTime));
        
        this.cameraRotation.y -= (lookInput.y * 2f) * Time.deltaTime;
        this.cameraRotation.y = Mathf.Clamp(this.cameraRotation.y, -this.pitchLock, this.pitchLock);
        this.cameraHeadY.localRotation = Quaternion.Euler(this.cameraRotation.y, 0f, 0f);
    }

    private void Update()
    {
        if (!this.isActive) return;

        Look();
        Bobbing();
    }
}
