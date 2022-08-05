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

    public int maxLife = 3;
    public int currentLife;
    
    private Rigidbody rb;
    private int currentLane = 1;
    private Vector3 verticalTargetPosition;
    public float laneSpeed;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        currentLife = maxLife;
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

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }
        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
    }
    void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;
        if (targetLane < 0 || targetLane > 2) return;
        currentLane = targetLane;
        verticalTargetPosition = new Vector3((currentLane - 1), 0, 0);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            currentLife--;
            speed = 0;
            if(currentLife <= 0)
            {
                //chamar o game over
            }            
        }
        Debug.Log("Game Over");
    }
}
