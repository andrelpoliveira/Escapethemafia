using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dificuldades dos inimigos
public enum Difficult
{
    EASY, MEDIUM, HARD
}

// 
public enum EnemyState
{
    JUMP, FIRE, PUSH, DIVERT
}

public class EnemyRun : MonoBehaviour
{
    [Header("Variaveis do Player")]
    public float speed;
    public float laneSpeed;
    public float grativy;
    public float jumpLength;
    public float jumpHeight;
    public int maxLife = 4;
    public float minSpeed = 10f;
    public float invencibleTime;
    private float invencible_time_start;
    // conttole de para aumento de velocidade
    public float time_max = 12;
    public Transform firepoint;
    // controle para dificuldade da ia, quanto maior o valor maior chance de executar
    public int percentage_ia;
    public float delay_ia;

    [Header("Efeitos")]
    public GameObject smokeRun;

    //controle de dificuldade do inimigo
    private Difficult difficult_enemy;
    private EnemyState current_state;
    //Controle de Anima��o e Audio
    private Animator anim;
    //Controle de Rigibody
    private Rigidbody rbPlayer;
    //Controle de Lanes (3 lanes existentes)
    private int currentLane = 2;
    private Vector3 verticalTargetPosition;
    //Controle de Jump
    private bool jumping = false;
    private float jumpStart;
    //Life atual do player
    public int currentLife;
    private bool invencible = false;
    static int blinkingValue;
    //Script
    public SpawnProjectileEnemy spawnProjectile;
    //Controle de Movimento
    private bool canMove = false;
    // controle de tempo para aumento de velocidade
    private float time_current;
    //valor da posi��o que ia precisa ir
    private string side;
    //pegando obj player
    private GameObject plr;

    // Start is called before the first frame update
    void Start()
    {
        //Chamada dos componentes
        rbPlayer = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        spawnProjectile = GetComponent<SpawnProjectileEnemy>();
        //Seta as vari�veis iniciais
        currentLife = maxLife;
        blinkingValue = Shader.PropertyToID("_BlinkingValue");
        //Start das miss�es
        GameController._gameController.StartMissions();
        //Inicio do game
        smokeRun.SetActive(false);
        invencible_time_start = invencibleTime;
        difficult_enemy = Difficult.EASY;

        StartRun();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
            return;
        RayCastVertical();
        RayCastHorizontal();
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * 5, Color.red);
        Debug.DrawRay(transform.position + Vector3.up, -transform.forward * 5, Color.blue);
        Debug.DrawRay(transform.position + Vector3.up, transform.right * 5, Color.green);
        Debug.DrawRay(transform.position + Vector3.up, -transform.right * 5, Color.green);

        /* ----- Jump Inicio -----*/
        if (jumping)
        {
            float ratio = (transform.position.z - jumpStart) / jumpLength;
            if (ratio >= 1f)
            {
                jumping = false;
                anim.SetBool("Jump", true);
                anim.SetBool("Run", false);
            }
            else
            {
                verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
            }
        }
        else
        {
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, grativy * Time.deltaTime);
        }
        /* ------ Jump Fim -----*/

        //Movimenta��o entre lanes Player
        Vector3 targetPostion = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPostion, laneSpeed * Time.deltaTime);

        // tempo para aumento de velocidade
        time_current += Time.deltaTime;

        if (time_current >= time_max)
        {
            IncreaseSpeed();
            time_current = 0;
        }
    }

    private void FixedUpdate()
    {
        rbPlayer.velocity = Vector3.forward * speed;
        //is_ground = Physics.CheckSphere(transform.position, 0.3f, layer);

        if (speed > 0 && currentLife > 0)

        {
            if (!jumping)
            {
                anim.SetBool("Idle", false);
                anim.SetBool("Jump", false);
                anim.SetBool("Run", true);
                smokeRun.SetActive(true);
            }
        }

    }
    //Start Status
    public void StartRun()
    {
        StartCoroutine(CountStart());
    }
    //Change Lanes
    void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;
        if (targetLane < -2 || targetLane > 6)
        {
            return;
        }
        currentLane = targetLane;
        verticalTargetPosition = new Vector3((currentLane - 2), 0, 0);
    }
    //Jump
    void Jump()
    {
        if (!jumping)
        {
            smokeRun.SetActive(false);
            jumpStart = transform.position.z;
            anim.SetFloat("JumpSpeed", (speed / jumpLength));
            anim.SetBool("Jump", true);
            anim.SetBool("Run", false);
            jumping = true;
        }
    }
    //Verifica��o das Colis�es
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Coin")
        {
            other.gameObject.SetActive(false);
        }
        if (other.tag == "MultiCoin")
        {
            other.gameObject.SetActive(false);
        }
        if (other.tag == "Heart")
        {
            currentLife++;

            if (currentLife >= 4)
            {
                currentLife = 4;
                StartCoroutine(Blinking(invencibleTime));
            }

            other.gameObject.SetActive(false);
        }
        if (other.tag == "Ammunition")
        {
            spawnProjectile.currentProjectile++;

            if (spawnProjectile.currentProjectile >= 5)
            {
                spawnProjectile.currentProjectile = 5;
            }

            other.gameObject.SetActive(false);
        }

        if (invencible) { return; }

        if (other.tag == "Obstacle")
        {
            Damage();

            if (currentLife <= 0)
            {
                Endgame();
            }
            else
            {
                StartCoroutine(Blinking(invencibleTime));
            }
        }
    }
    IEnumerator Blinking(float time)
    {
        invencible = true;
        float timer = 0;
        float currentBlink = 1f;
        float lastBlink = 0;
        float blinkPeriod = 0.1f;
        yield return new WaitForSeconds(0.5f);
        canMove = true;
        while (timer < time && invencible)
        {
            Shader.SetGlobalFloat(blinkingValue, currentBlink);
            yield return null;
            timer += Time.deltaTime;
            lastBlink += Time.deltaTime;
            if (blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1f - currentBlink;
            }
        }
        Shader.SetGlobalFloat(blinkingValue, 0);
        invencible = false;
    }
    IEnumerator CountStart()
    {
        anim.SetBool("Idle", true);
        anim.SetBool("Run", false);
        verticalTargetPosition = transform.position;
        yield return new WaitForSeconds(3f);
        speed = minSpeed;
        canMove = true;
    }
    public void Damage()
    {
        currentLife--;
        //canMove = false;
        speed *= 0.84f;
        invencibleTime *= 1.2f;

        if (speed <= minSpeed)
        {
            speed = minSpeed;
            invencibleTime = invencible_time_start;
        }
    }
    void Endgame()
    {
        Shader.SetGlobalFloat(blinkingValue, 0);
        speed = 0;
        canMove = false;
        anim.SetBool("Idle", true);
        anim.SetBool("Run", false);
        smokeRun.SetActive(false);
    }
    public void IncreaseSpeed()
    {
        speed *= 1.2f;
        invencibleTime *= 0.82f;
    }

    // dect��o de colis�es frente e tr�s
    void RayCastVertical()
    {
        RaycastHit hit_info;

        //frente
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit_info, 5))
        {
            if (hit_info.collider.tag == "Obstacle" && RandIA(percentage_ia) == true)
            {
                OnStateEnter(EnemyState.JUMP);
            }

            if (hit_info.collider.tag == "Player" && RandIA(percentage_ia) == true && hit_info.distance >= 3)
            {
                OnStateEnter(EnemyState.FIRE);
            }

            if (hit_info.collider.tag == "Obstacle" && (hit_info.collider.name.Equals("SafeBoxObj(Clone)") || hit_info.collider.name.Equals("SafeBox")))
            {
                if (RandIA(percentage_ia) == true)
                {
                    if (spawnProjectile.currentProjectile > 0)
                    {
                        OnStateEnter(EnemyState.FIRE);
                    }
                    else
                    {
                        OnStateEnter(EnemyState.JUMP);
                    }
                }
            }
        }

        //tr�s
        if (Physics.Raycast(transform.position + Vector3.up, -transform.forward, out hit_info, 5))
        {
            if (hit_info.collider.tag == "Player" && RandIA(percentage_ia) == true)
            {
                OnStateEnter(EnemyState.JUMP);
            }
        }
    }

    // dect��o de colis�es esquerda e direita
    void RayCastHorizontal()
    {
        RaycastHit hit_info;

        //direita
        if (Physics.Raycast(transform.position + Vector3.up, transform.right, out hit_info, 5))
        {
            // coleta moeda
            if (hit_info.collider.tag == "Coin" && RandIA(percentage_ia) == true)
            {
                OnStateEnter(EnemyState.DIVERT);
                side = "right";
            }

            // empura player
            if (hit_info.collider.tag == "Player" && RandIA(percentage_ia) == true)
            {
                OnStateEnter(EnemyState.PUSH);
                plr = hit_info.collider.gameObject;
                side = "right";
            }

            //coleta de muni��o
            if (hit_info.collider.tag == "Ammunition" && RandIA(percentage_ia) == true)
            {
                OnStateEnter(EnemyState.DIVERT);
                side = "right";
            }
        }

        //esqueda
        if (Physics.Raycast(transform.position + Vector3.up, -transform.right, out hit_info, 5))
        {
            // coleta moeda
            if (hit_info.collider.tag == "Coin" && RandIA(percentage_ia) == true)
            {
                OnStateEnter(EnemyState.DIVERT);
                side = "left";
            }

            // empura player
            if (hit_info.collider.tag == "Player" && RandIA(percentage_ia) == true)
            {
                OnStateEnter(EnemyState.PUSH);
                plr = hit_info.collider.gameObject;
                side = "left";
            }

            // coleta muni��o
            if (hit_info.collider.tag == "Ammunition" && RandIA(percentage_ia) == true)
            {
                OnStateEnter(EnemyState.DIVERT);
                side = "left";
            }
        }
    }
    //maquina de estado
    void OnStateEnter(EnemyState new_enemy_state)
    {
        StopAllCoroutines();
        current_state = new_enemy_state;

        switch (current_state)
        {
            case EnemyState.JUMP:
                StartCoroutine(JumpState());

                break;

            case EnemyState.FIRE:
                StartCoroutine(FireState());
                break;

            case EnemyState.PUSH:
                //StartCoroutine(PushEstate());
                break;

            case EnemyState.DIVERT:
                StartCoroutine(DivertState());
                break;
        }
    }

    //fun��es da ia pulo
    IEnumerator JumpState()
    {
        Jump();
        yield return new WaitUntil(() => verticalTargetPosition.y == 0);
    }

    //fun��es da ia troca de raia
    IEnumerator DivertState()
    {
        if (side == "right")
        {
            ChangeLane(2);
            yield return new WaitForSeconds(delay_ia);
        }

        if (side == "left")
        {
            ChangeLane(-2);
            yield return new WaitForSeconds(delay_ia);
        }
    }

    //fun��es da ia empurar player
    IEnumerator PushEstate()
    {
        if (side == "right")
        {
            ChangeLane(2);

            if (plr.GetComponent<PlayerRun>() != null)
            {
                plr.GetComponent<PlayerRun>().Divert(2);
            }
            else if (plr.GetComponent<EnemyRun>() != null)
            {
                plr.GetComponent<EnemyRun>().Divert(2);
            }
            yield return new WaitForSeconds(delay_ia);
        }

        if (side == "left")
        {
            ChangeLane(-2);
            if (plr.GetComponent<PlayerRun>() != null)
            {
                plr.GetComponent<PlayerRun>().Divert(-2);
            }
            else if (plr.GetComponent<EnemyRun>() != null)
            {
                plr.GetComponent<EnemyRun>().Divert(-2);
            }
            yield return new WaitForSeconds(delay_ia);
        }
    }

    //fun��es da ia tiro
    IEnumerator FireState()
    {
        spawnProjectile.SpawnFx();
        yield return new WaitForSeconds(delay_ia);
    }

    //metodo para decidir o que a ia vai fazer
    bool RandIA(int value)
    {
        int temp = Random.Range(0, 100);
        bool retorno = temp <= value ? true : false;
        return retorno;
    }

    public void Divert(int value)
    {
        ChangeLane(value);
    }
}
