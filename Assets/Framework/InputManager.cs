using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputSystemMain inputSystemMain;

    public Vector2 movementInput;
    public Vector2 lookInput;
    
    private InputAction movementAction;
    private InputAction lookAction;
    
    private void Awake()
    {
        this.inputSystemMain = new InputSystemMain();

        this.movementAction = this.inputSystemMain.FindAction("Move");
        this.lookAction = this.inputSystemMain.FindAction("Look");
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
