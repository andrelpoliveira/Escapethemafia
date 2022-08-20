using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjs : MonoBehaviour
{
    private Rigidbody rbCar;
    public float speed;
    public float minSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rbCar = GetComponent<Rigidbody>();
        Invoke("StartMove", 4f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rbCar.velocity = Vector3.forward * -speed;
    }

    void StartMove()
    {
        speed = minSpeed;

    }


}
