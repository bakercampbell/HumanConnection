using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class TopDownMovement : MonoBehaviour
{
    CharacterController character;
    Vector3 moveVector;
    [SerializeField, Range(0,20)]
    float speed;
    void Start()
    {
        character = GetComponent<CharacterController>();
    }
    void FixedUpdate()
    {
        character.Move(moveVector * speed * Time.fixedDeltaTime);
    }

    public void OnMoveChanged(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        moveVector = new Vector3(direction.x, 0, direction.y);
    }
}
