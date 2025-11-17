using UnityEngine;
using UnityEngine.Serialization;

public class PlayerFirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform cameraHeadX;
    [SerializeField] private Transform cameraHeadY;
    
    private MeshRenderer meshRenderer;
    
    private InputManager inputManager;

    public bool isActive = true;
    public float sensitivity = 2.5f;
    public Vector2 cameraRotation;
    public float pitchLock = 90f;

    private void Start()
    {
        this.inputManager = GetComponent<InputManager>();
        this.meshRenderer = GetComponent<MeshRenderer>();
        
        this.playerHead = GameObject.FindGameObjectWithTag("PlayerHead").transform;
        
        this.cameraHeadX = GameObject.FindGameObjectWithTag("CameraHead").transform;
        this.cameraHeadY = this.cameraHeadX?.Find("Y");
    }

    private void Look()
    {
        this.meshRenderer.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Vector2 lookInput = this.inputManager.lookInput * this.sensitivity;
        
        this.cameraHeadX.position = this.playerHead.position;
        this.cameraHeadX.Rotate(Vector3.up * (lookInput.x * Time.deltaTime));
        
        this.cameraRotation.y -= (lookInput.y * 2f) * Time.deltaTime;
        this.cameraRotation.y = Mathf.Clamp(this.cameraRotation.y, -this.pitchLock, this.pitchLock);
        this.cameraHeadY.localRotation = Quaternion.Euler(this.cameraRotation.y, 0f, 0f);
    }

    private void Update()
    {
        if (!this.isActive) return;

        Look();
    }
}
