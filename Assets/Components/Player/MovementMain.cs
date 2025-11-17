using UnityEngine;

public class MovementMain : MonoBehaviour
{
    private CharacterController controller;
    public InputManager inputManager;

    public float speed = 16f;
    public Vector3 velocity;

    [SerializeField] private float stickGround = -2f;
    private void Start()
    {
        this.controller = GetComponent<CharacterController>();
        this.inputManager = GetComponent<InputManager>();
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
        
        Vector3 computed = (cameraForward * movement.y) + (cameraRight * movement.x);
        if (this.controller.isGrounded)
        {
            if (this.velocity.y < 0f)
                computed.y = this.stickGround;
        }
        else
            this.velocity += Physics.gravity * Time.deltaTime;
        
        computed += this.velocity;
        this.controller.Move(computed * (this.speed * Time.deltaTime));
    }

    private void Update()
    {
        Move();
    }
}
