using UnityEngine;

public class MovementMain : MonoBehaviour
{
    private CharacterController controller;
    public InputManager inputManager;

    public float speed = 16f;
    public Vector3 velocity;

    public float maxStamina = 100f;
    public float stamina = 100f;
    public float staminaRegenRate = 1f;
    public bool isSprinting = false;

    [SerializeField] private float stickGround = -2f;
    private void Start()
    {
        this.controller = GetComponent<CharacterController>();
        this.inputManager = GetComponent<InputManager>();
    }

    private void StaminaRegen()
    {
        if (this.inputManager.sprintKeyHeld) return;
        if (this.stamina < this.maxStamina)
            this.stamina = Mathf.Clamp(this.stamina + (Time.deltaTime * this.staminaRegenRate), 0f, this.maxStamina);
    }

    private void Move()
    {
        Vector2 movement = this.inputManager.movementInput.normalized;
        
        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraForward.Normalize();
        cameraRight.y = 0f;
        cameraRight.Normalize();
        
        this.isSprinting = this.inputManager.sprintKeyHeld;
        if (this.stamina <= 0f) this.isSprinting = false;
        
        Vector3 computed = (cameraForward * movement.y) + (cameraRight * movement.x);
        if (this.controller.isGrounded)
        {
            if (this.velocity.y < 0f)
                computed.y = this.stickGround;
        }
        else
            this.velocity += Physics.gravity * Time.deltaTime;
        
        computed += this.velocity;
        this.controller.Move(computed * (this.speed * Time.deltaTime * (this.isSprinting ? 1.5f : 1f)));
    }

    private void Update()
    {
        Debug.Log(this.inputManager.sprintKeyHeld);
        
        StaminaRegen();
        Move();
    }
}
