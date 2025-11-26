using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputSystemMain inputSystemMain;

    public Vector2 movementInput;
    public Vector2 lookInput;
    private InputAction sprintAction;
    private InputAction crouchAction;
    
    
    public bool sprintKeyHeld;
    public UnityEvent onShoot;
    
    private InputAction movementAction;
    private InputAction lookAction;
    private InputAction shootAction;
    
    private void Awake()
    {
        this.inputSystemMain = new InputSystemMain();

        this.movementAction = this.inputSystemMain.FindAction("Move");
        this.lookAction = this.inputSystemMain.FindAction("Look");
        this.sprintAction = this.inputSystemMain.FindAction("Sprint");
        this.crouchAction = this.inputSystemMain.FindAction("Crouch");
        this.shootAction = this.inputSystemMain.FindAction("Attack");
        
        SprintInit();
        ShootInit();
    }

    private void ShootInit()
    {
        this.shootAction.performed += context => this.onShoot.Invoke();
    }

    private void SprintInit()
    {
        this.sprintAction.performed += context => this.sprintKeyHeld = true;
        this.sprintAction.canceled += context => this.sprintKeyHeld = false;
    }

    private void OnEnable()
    {
        this.inputSystemMain ??= new InputSystemMain();
        this.inputSystemMain.Enable();
    }

    private void OnDisable() =>
        this.inputSystemMain.Disable();
    

    private void Update()
    {
        if (this.inputSystemMain == null) return;
        
        this.movementInput = this.movementAction.ReadValue<Vector2>();
        this.lookInput = this.lookAction.ReadValue<Vector2>();
    }
}
