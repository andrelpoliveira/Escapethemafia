using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    //controle de HUD
    public Image lifeBar;


    [Header("Efeitos")]
    public GameObject smokeRun;
    //Controle de Animação e Audio
    private Animator anim;
    //Controle de Rigibody
    private Rigidbody rbPlayer;
    //Controle de Lanes (3 lanes existentes)
    [HideInInspector]
    public int currentLane = 2;
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
    private IAEnemy iAEnemy;
    //Controle de Movimento
    private bool canMove = false;
    // controle de tempo para aumento de velocidade
    private float time_current;
    private bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        //Chamada dos componentes
        rbPlayer = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        spawnProjectile = GetComponent<SpawnProjectileEnemy>();
        iAEnemy = GetComponent<IAEnemy>();
        //Seta as variáveis iniciais
        currentLife = maxLife;
        blinkingValue = Shader.PropertyToID("_BlinkingValue");
        //Start das missões
        GameController._gameController.StartMissions();
        //Inicio do game
        smokeRun.SetActive(false);
        invencible_time_start = invencibleTime;
        lifeBar.fillAmount = currentLife / maxLife;
        StartRun();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
            return;

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

        //Movimentação entre lanes Player
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
    public void ChangeLane(int direction)
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
    public void Jump()
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
    //Verificação das Colisões
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Coin" && Vector3.Distance(transform.position, other.transform.position) <= 2)
        {
            other.gameObject.SetActive(false);
        }
        if (other.tag == "MultiCoin" && Vector3.Distance(transform.position, other.transform.position) <= 2)
        {
            other.gameObject.SetActive(false);
        }
        if (other.tag == "Heart" && Vector3.Distance(transform.position, other.transform.position) <= 2)
        {
            currentLife++;

            if (currentLife >= 4)
            {
                currentLife = 4;
                StartCoroutine(Blinking(invencibleTime));
            }
            
            lifeBar.fillAmount = (float)(currentLife) / maxLife;
            other.gameObject.SetActive(false);
        }
        if (other.tag == "Ammunition" && Vector3.Distance(transform.position, other.transform.position) <= 2)
        {
            spawnProjectile.currentProjectile++;

            if (spawnProjectile.currentProjectile >= 5)
            {
                spawnProjectile.currentProjectile = 5;
            }

            other.gameObject.SetActive(false);
        }

        if (invencible) { return; }

        if (other.tag == "Obstacle" && Vector3.Distance(transform.position, other.transform.position) <= 2)
        {
            Damage();
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
            //Shader.SetGlobalFloat(blinkingValue, currentBlink);
            yield return null;
            timer += Time.deltaTime;
            lastBlink += Time.deltaTime;
            if (blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1f - currentBlink;
            }
        }
        //Shader.SetGlobalFloat(blinkingValue, 0);
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
        lifeBar.fillAmount = (float)(currentLife) / maxLife;
        //canMove = false;
        speed *= 0.91f;
        iAEnemy.distanceJump *= .927f;
        invencibleTime *= 2.5f;

        if (speed <= minSpeed)
        {
            speed = minSpeed;
            invencibleTime = invencible_time_start;
            iAEnemy.distanceJump = 9;
        }

        if (currentLife <= 0)
        {
            Endgame();
        }
        else
        {
            StartCoroutine(Blinking(invencibleTime));
        }
    }
    public void Endgame()
    {
        if(isGameOver == true) { return; }

        print("enemy");
        isGameOver = true;
        speed = 0;
        canMove = false;
        anim.SetBool("Idle", true);
        anim.SetBool("Run", false);
        smokeRun.SetActive(false);
        GameController._gameController.GameWin(1);
    }
    public void IncreaseSpeed()
    {
        speed *= 1.2f;
        iAEnemy.distanceJump *= 1.08f;
        invencibleTime *= 0.4f;
    }

    public void Divert(int value)
    {
        ChangeLane(value);
    }
}
