using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Variaveis do Player")]
    public CharacterController  _characterController;
    public float                speed;
    public float                jumpHeight;
    public float                gravity;
    private float               jumpVelocity;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    
    void Update()
    {
        Vector3 direction = Vector3.forward * speed;

        if (_characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpVelocity = jumpHeight;
            }
        }
        else
        {
            jumpVelocity -= gravity;
        }

        direction.y = jumpVelocity;

        _characterController.Move(direction * Time.deltaTime);
    }
}
