using UnityEngine;
using UnityEngine.Serialization;

public class PlayerFirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform cameraHeadX;
    [SerializeField] private Transform cameraHeadY;
    
    private CharacterController characterController;
    private MeshRenderer meshRenderer;
    private MovementMain movementMain;
    
    private InputManager inputManager;

    public bool isActive = true;
    public float sensitivity = 2.5f;
    public Vector2 cameraRotation;
    public float pitchLock = 90f;

    public Vector3 bobbingOffset;
    public Vector3 bobbingRotation;

    private Vector3 center;
    private void Start()
    {
        this.inputManager = GetComponent<InputManager>();
        this.meshRenderer = GetComponent<MeshRenderer>();
        this.characterController = GetComponent<CharacterController>();
        
        this.movementMain = GetComponent<MovementMain>();
        this.playerHead = GameObject.FindGameObjectWithTag("PlayerHead").transform;
        
        this.cameraHeadX = GameObject.FindGameObjectWithTag("CameraHead").transform;
        this.cameraHeadY = this.cameraHeadX?.Find("Y");
        
        this.center = this.playerHead.localPosition;
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

                this.bobbingRotation = Vector3.Lerp(
                    this.bobbingRotation,

                    new Vector3(
                        Mathf.Sin(Time.time * 2f) * (.3f + Mathf.PerlinNoise1D(Time.time * .2f)), 0f, Mathf.PerlinNoise1D(Time.time * .3f) * 2f), 4f * Time.deltaTime);
            }
            else
            {
                float strafeDir = this.inputManager.movementInput.x * velocity.sqrMagnitude;
                
                this.bobbingOffset = Vector3.Lerp(this.bobbingOffset, new Vector3(
                    Mathf.Cos((Time.time + 12f) * 3f) * .2f,
                    Mathf.Cos(Time.time * 12f) * .4f,
                    Mathf.Sin((Time.time + 6f) * 6f) * .7f
                ), 2f * Time.deltaTime) * Mathf.Clamp(velocity.magnitude/this.movementMain.speed, .1f, 1f);
                
                this.bobbingRotation = Vector3.Lerp(this.bobbingRotation, 
                    new Vector3(
                        Mathf.Cos(Time.time * 12f) * 1f, 
                        0f, Mathf.Sin((Time.time + 6f) * 6f) * -1f + -Mathf.Clamp(strafeDir, -5f, 5f)
                        ), 4f * Time.deltaTime);
            }
            
            this.bobbingRotation = Vector3.Lerp(
                this.bobbingRotation, 
                new Vector3(0f, 0f, this.inputManager.lookInput.x * -1f)
                , 1f * Time.deltaTime);
        }
        
        this.playerHead.localPosition = this.center + this.bobbingOffset;
        this.playerHead.localRotation = Quaternion.Euler(this.bobbingRotation);
    }

    private void Look()
    {
        this.meshRenderer.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Vector2 lookInput = this.inputManager.lookInput * (this.sensitivity * Time.deltaTime);
        
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
