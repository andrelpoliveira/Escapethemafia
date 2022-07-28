using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //[Header("Variaveis do Player")]
    //public CharacterController  _characterController;
    //public float                speed;
    //public float                jumpHeight;
    //public float                gravity;
    //private float               jumpVelocity;
    //Vector3 direction;

    //void Start()
    //{
    //    _characterController = GetComponent<CharacterController>();
    //}


    //void Update()
    //{

    //}

    //private void FixedUpdate()
    //{
    //    direction.z *= speed;
    //    //direction = Vector3.forward * speed;

    //    if (_characterController.isGrounded)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Space))
    //        {
    //            jumpVelocity = jumpHeight;
    //        }
    //    }
    //    else
    //    {
    //        jumpVelocity -= gravity;
    //    }

    //    direction.y = jumpVelocity;

    //    _characterController.Move(direction * Time.deltaTime);
    //}

    public static Player instance;

    private bool is_ground;
    private bool is_walk;
    private Vector3 moviment;
    private int id_point = 1;
    private Vector3 target_position;

    public Transform power_sensor;
    public Transform colletable_target;

    public float speed;
    public float speed_change;
    public float jump_force;
    public Transform ground_check;
    public LayerMask wait_is_ground;
    [HideInInspector]
    public Rigidbody plr_rb;
    public Transform[] way_points;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        plr_rb = GetComponent<Rigidbody>();
        target_position = new Vector3(way_points[id_point].position.x, transform.position.y, transform.position.z);

        //FadeInOut.instance.Fade();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            InputX(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            InputX(-1);
        }
    }

    private void FixedUpdate()
    {
        is_ground = Physics.CheckSphere(ground_check.position, 0.3f, wait_is_ground);

        target_position = new Vector3(way_points[id_point].position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target_position, speed_change * Time.deltaTime);
        //power_sensor.position = transform.position;
        moviment = new Vector3(0, plr_rb.velocity.y, speed);
        plr_rb.velocity = moviment;
    }

    // Meus metodos

    public void Jump()
    {
        if (is_ground == false) { return; }

        //AudioManager.instance.PlayFx(AudioManager.instance.fx_jump);
        plr_rb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
    }

    public void InputX(int value)
    {
        id_point += value;

        if (id_point >= way_points.Length)
        {
            id_point = way_points.Length - 1;
        }
        else if (id_point < 0)
        {
            id_point = 0;
        }
    }
}
