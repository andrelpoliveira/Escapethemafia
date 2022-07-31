using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private bool is_move;
    private bool is_fire;

    private Vector3 moviment;
    private Vector3 target_position;

    private int id_point = 1;

    private float life_max = 100;
    private float convert_float = 100;
    private float bulet_max = 100;

    [HideInInspector]
    public float life_current;
    [HideInInspector]
    public float bulet_current;

    [Header("---Coletaveis---")]
    public Transform power_sensor;
    public Transform colletable_target;

    [Header("---Movimento---")]
    public float speed;
    public float speed_change;
    public float jump_force;
    public Transform ground_check;
    public LayerMask wait_is_ground;
    public Transform[] way_points;

    [Header("---Tiro/Vida---")]
    public Image bulet_bar;
    public Image life_bar;
    public int fire_value;
    public int damage_enemy;
    public float delay_fire;
    public GameObject bullet_prefab;
    public Transform bullet_position;
        
    [HideInInspector]
    public Rigidbody plr_rb;
    [HideInInspector]
    public Vector3 mouse_position;

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
        life_current = life_max;
        bulet_current = bulet_max;

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
            MoveX(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveX(-1);
        }
        
        GameManager.instance.UpdateHUD();
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Collectable")
        {
            Collectable temp = other.GetComponent<Collectable>();

            switch (temp.item_type)
            {
                case ItemType.COIN:
                    GameManager.instance.coin += temp.item_value;
                    other.transform.gameObject.SetActive(false);
                    break;

                case ItemType.BULLET:
                    bulet_current += temp.item_value;
                    ShootAlteration("Recovery", temp.item_value);
                    other.transform.gameObject.SetActive(false);
                    break;

                case ItemType.LIFE:
                    LifeAlteration("Recovery", temp.item_value);
                    other.transform.gameObject.SetActive(false);
                    break;
            }
        }

        if(other.tag == "Enemy")
        {
            LifeAlteration("Damage", damage_enemy);
        }
    }

    // Meus metodos

    public void Jump()
    {
        // if (is_ground == false) { return; }

        //AudioManager.instance.PlayFx(AudioManager.instance.fx_jump);
        plr_rb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
    }

    public void MoveX(int value)
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

    // arrasto de tela
    public void DragMove()
    {
        // esquerda 
        if ((mouse_position.x - Input.mousePosition.x) >= 20 && is_move == false)
        {
            is_move = true;
            MoveX(-1);
        }
        // direita
        if ((mouse_position.x - Input.mousePosition.x) <= -20 && is_move == false)
        {
            is_move = true;
            MoveX(1);
        }

        // cima
        if((mouse_position.y - Input.mousePosition.y) <= -20 && is_move == false && is_ground == true)
        {
            is_move = true;
            Jump();
        }
    }

    // quando tira dedo da tela
    public void Up()
    {
        is_move = false;
    }

    // quando toca na tela
    public void Click()
    {
        mouse_position = Input.mousePosition;
    }

    // quando atira
    public void Fire()
    {
        if(is_fire == false && bulet_current > 0)
        {
            is_fire = true;
            ShootAlteration("Fire", fire_value);
            Instantiate(bullet_prefab, bullet_position.position, Quaternion.identity);
            StartCoroutine(Delay());
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay_fire);
        is_fire = false;
    }

    // gerenciamento de tiro
    public void ShootAlteration(string type, int value)
    {
        if(type == "Fire")
        {
            bulet_current -= value;
        }
        else if(type == "Recovery")
        {
            bulet_current += value;
        }

        if (bulet_current <= 0)
        {
            bulet_current = 0;
        }

        if(bulet_current > bulet_max)
        {
            bulet_current = bulet_max;
        }
        
        bulet_bar.fillAmount = bulet_current / convert_float;
    }

    // gerenciamento de vida
    public void LifeAlteration(string type, int value)
    {
        if(type == "Damage")
        {
            life_current -= value;
        }
        else if(type == "Recovery")
        {
            life_current += value;
        }

        if(life_current <= 0)
        {
            life_current = 0;
            GameManager.instance.GameOver();
        }

        if(life_current > convert_float)
        {
            life_current = convert_float;
        }

        life_bar.fillAmount = life_current / convert_float;
    }
}
