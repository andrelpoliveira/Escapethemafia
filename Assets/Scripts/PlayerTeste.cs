using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeste : MonoBehaviour
{
    public float speed;    
    private Rigidbody rb;
    private int currentLane = 1;
    private Vector3 verticalTargetPosition;
    public float laneSpeed;


    public int maxLife = 3;
    public int currentLife;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentLife = maxLife;
    }

    // Update is called once per frame
    void Update()
    {
        

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
    private void FixedUpdate()
    {
        rb.velocity = Vector3.forward * speed;
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
            if (currentLife <= 0)
            {
                Debug.Log("Game Over");//chamar o game over
            }
        }
    }
}
