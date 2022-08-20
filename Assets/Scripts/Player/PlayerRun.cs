using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRun : MonoBehaviour
{
    [Header("Variaveis do Player")]
    public float speed;
    public float laneSpeed;
    public float jumpLength;
    public float jumpHeight;
    public int maxLife = 4;
    public float minSpeed = 10f;
    //public float maxSpeed = 20f;
    public float invencibleTime;
    private float invencible_time_start;
    // conttole de para aumento de velocidade
    public float time_max = 12;

    [Header("Efeitos")]
    public GameObject smokeRun;

    //Controle de Animação e Audio
    private AudioSource runAudio;
    private Animator anim;
    //Controle de Rigibody
    private Rigidbody rbPlayer;
    //Controle de Lanes (3 lanes existentes)
    private int currentLane = 2;
    private Vector3 verticalTargetPosition;
    //Controle de Jump
    private bool jumping = false;
    private float jumpStart;
    //Swipe para mobile
    private bool isSwipe = false;
    private Vector2 startTouch;
    //Life atual do player
    private int currentLife;
    private bool invencible = false;
    static int blinkingValue;
    //Script
    public UiManager uiManager;
    public SpawnProjectile spawnProjectile;
    //Coletáveis
    [HideInInspector]
    public int coin;
    [HideInInspector]
    public float score;
    //Controle de Movimento
    private bool canMove = false;
    // controle de tempo 
    private float time_current;

    // Start is called before the first frame update
    void Start()
    {
        //Chamada dos componentes
        rbPlayer = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        runAudio = GetComponent<AudioSource>();
        spawnProjectile = GetComponent<SpawnProjectile>();
        uiManager = FindObjectOfType<UiManager>();
        //Seta as variáveis iniciais
        currentLife = maxLife;
        blinkingValue = Shader.PropertyToID("_BlinkingValue");
        //Start das missões
        GameController._gameController.StartMissions();
        //Inicio do game
        smokeRun.SetActive(false);
        runAudio.mute = true;
        invencible_time_start = invencibleTime;
        StartRun();

    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
            return;
        //Score do game e Update UI
        score += Time.deltaTime * speed;
        uiManager.UpdateScore((int)score);

        /* -----Inputs para pc Início -----*/
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-2);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(2);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        /* ----- Inputs para PC FIM -----*/
        /* ----- Inputs para Mobile Início -----*/
        if (Input.touchCount == 1)
        {
            if (isSwipe)
            {
                Vector2 diff = Input.GetTouch(0).position - startTouch;
                diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);
                if (diff.magnitude > 0.01f)
                {
                    if (Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
                    {
                        if (diff.y > 0)
                        {
                            Jump();
                        }
                    }
                    else
                    {
                        if (diff.x < 0) { ChangeLane(-2); } else { ChangeLane(4); }
                    }

                    isSwipe = false;
                }
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startTouch = Input.GetTouch(0).position;
                isSwipe = true;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                isSwipe = false;
            }
        }
        /* ----- Inputs para Mobile Fim -----*/
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
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, 5f * Time.deltaTime);
        }
        /* ------ Jump Fim -----*/

        // tempo para aumento de velocidade
        time_current += Time.deltaTime;

        if(time_current >= time_max)
        {
            IncreaseSpeed();
            time_current = 0;
        }

        //Movimentação entre lanes Player
        Vector3 targetPostion = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPostion, laneSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        rbPlayer.velocity = Vector3.forward * speed;
        if (speed > 0 && currentLife > 0)
        {
            if (!jumping) { anim.SetBool("Idle", false); anim.SetBool("Jump", false); anim.SetBool("Run", true); smokeRun.SetActive(true); runAudio.mute = false; }
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
            anim.SetFloat("JumpSpeed", speed / jumpLength);
            anim.SetBool("Jump", true);
            anim.SetBool("Run", false);
            jumping = true;
            runAudio.mute = true;
        }
    }
    //Verificação das Colisões
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Coin")
        {
            coin++;
            uiManager.UpdateCoins(coin);
            other.gameObject.SetActive(false);
        }
        if (other.tag == "MultiCoin")
        {
            coin += 50;
            uiManager.UpdateCoins(coin);
            other.gameObject.SetActive(false);
        }
        if (other.tag == "Heart")
        {
            if (currentLife >= 4)
            {
                currentLife = 4;
            }
            else
            {
                currentLife++;
                uiManager.UpdateLife(currentLife);
                StartCoroutine(Blinking(invencibleTime));
                other.gameObject.SetActive(false);
            }
        }
        if (other.tag == "Ammunition")
        {
            if (spawnProjectile.currentProjectile >= 5)
            {
                spawnProjectile.currentProjectile = 5;
            }
            else if (spawnProjectile.currentProjectile < 5)
            {
                spawnProjectile.currentProjectile++;
                uiManager.UpdateProjectile(spawnProjectile.currentProjectile);
                other.gameObject.SetActive(false);
            }
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
        yield return new WaitForSeconds(3f);
        speed = minSpeed;
        canMove = true;
    }
    void Damage()
    {
        currentLife--;
        canMove = false;
        uiManager.UpdateLife(currentLife);
        runAudio.mute = true;
        speed *= 0.84f;
        invencibleTime *= 1.2f;

        if(speed <= minSpeed)
        {
            speed = minSpeed;
            invencibleTime = invencible_time_start;
        }
    }
    void Endgame()
    {
        GameController._gameController.coins += coin;
        speed = 0;
        canMove = false;
        anim.SetBool("Idle", true);
        anim.SetBool("Run", false);
        smokeRun.SetActive(false);
        runAudio.mute = true;
        uiManager.gameOverPanel.SetActive(true);
    }
    public void IncreaseSpeed()
    {
        speed *= 1.2f;
        invencibleTime *= 0.84f;
    }
}
