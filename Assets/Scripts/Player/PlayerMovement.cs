using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Movimiento
    public float movementSpeed;
    private Rigidbody2D rb2D;

    [HideInInspector]
    public Vector2 moveDirection;
    [HideInInspector]
    public Vector2 fireDirection = Vector2.right; // Dirección por defecto

    public InputActionReference move;
    public InputActionReference attack;
    public InputActionReference aim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();

        Vector2 aimInput = aim.action.ReadValue<Vector2>();
        // Si pulsamos una flecha, actualizamos la dirección y la guardamos
        if (aimInput != Vector2.zero)
        {
            fireDirection = aimInput;
        }
    }

    private void FixedUpdate()
    {
      rb2D.linearVelocity = new Vector2(moveDirection.x * movementSpeed, moveDirection.y * movementSpeed); 
      
    }


    private void OnEnable()
    {
        attack.action.started += Attack;
    }

    private void OnDisable()
    {
        attack.action.started -= Attack;
    }

    private void Attack(InputAction.CallbackContext obj)
    {
        Debug.Log("AL ATAQUEEEEE");
    }

}
