using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerRun : MonoBehaviour
{
    [Header("Variaveis do Player")]
    public float speed;
    public float laneSpeed;
    public float grativy;
    public float jumpLength;
    public float jumpHeight;
    //public float jump_force;
    //public LayerMask layer;
    public int maxLife = 4;
    public float minSpeed = 10f;
    //public float maxSpeed = 20f;
    private float speedAtual;
    public float invencibleTime;
    private float invencible_time_start;
    // conttole de para aumento de velocidade
    public float time_max = 12;

    [Header("Efeitos")]
    public GameObject smokeRun;
    public GameObject shield;
    public GameObject model;

    //Itens de UI
    private Button btnShield;
    private Button btnRun2;
    //Controle de Animação e Audio
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
    public int currentLife;
    public bool invencible = false;
    private bool is_shield;
    //static int blinkingValue;
    //Script
    public UiManager uiManager;
    public SpawnProjectile spawnProjectile;
    AudioManager audio_manager;
    GameController game_controller;
    //Coletáveis
    //[HideInInspector]
    public int coin;
    //[HideInInspector]
    public float score;
    public int shields;
    public int run;
    //Controle de Movimento
    private bool canMove = false;
    // controle de tempo 
    private float time_current;
    private float time_game_over;
    // controle de empurar
    private bool is_push;
    private int push_value;
    RaycastHit hit_info_test;
    // controle de gameover
    private bool is_gameover;

    // Start is called before the first frame update
    void Start()
    {
        //Chamada dos componentes
        rbPlayer = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        spawnProjectile = GetComponent<SpawnProjectile>();
        uiManager = FindObjectOfType<UiManager>();
        audio_manager = AudioManager.instance;
        game_controller = GameController._gameController;
        btnShield = uiManager.btnShield;
        btnRun2 = uiManager.btnRun2;
        //Seta as variáveis iniciais
        currentLife = maxLife;
        //blinkingValue = Shader.PropertyToID("_BlinkingValue");
        //Start das missões
        game_controller.StartMissions();
        //Inicio do game
        smokeRun.SetActive(false);
        shield.SetActive(false);
        invencible_time_start = invencibleTime;
        StartRun();
        is_gameover = false;
        coin = game_controller.temp_coins;
        score = game_controller.temp_score;
        uiManager.UpdateCoins(coin);
        uiManager.UpdateScore((int)score);
        uiManager.UpdateShield(shields);
        uiManager.UpdateRuns(run);
        //manter tela ligada do celular
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
            return;
        //Score do game e Update UI
        score += Time.deltaTime * speed;
        uiManager.UpdateScore((int)score);

        Debug.DrawRay(transform.position + Vector3.up, transform.forward * 5, Color.red);
        /* -----Inputs para pc Início -----*/
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-2);
            is_push = true;
            push_value = -2;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(2);
            is_push = true;
            push_value = 2;
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
                        if (diff.x < 0) { ChangeLane(-2); } else { ChangeLane(2); }
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

        //RaycastHit hit_info;

        //if (!Physics.Raycast(transform.position + Vector3.up, -transform.up, out hit_info, 5))
        //{
        //    Endgame();
        //}

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
        is_push = false;
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
    //Verificação das Colisões
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Coin")
        {
            coin++;
            uiManager.UpdateCoins(coin);
            other.gameObject.SetActive(false);
            audio_manager.PlayFx(audio_manager.fx_coin);
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
                audio_manager.PlayFx(audio_manager.fx_heart);
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
        if (other.tag == "Run")
        {
            run++;
            uiManager.UpdateRuns(run);
            other.gameObject.SetActive(false);
        }
        if (other.tag == "Shield")
        {
            shields++;
            uiManager.UpdateShield(shields);
            other.gameObject.SetActive(false);
        }

        if (other.tag == "Obstacle" && invencible == false)
        {
            Damage();

            if (currentLife <= 0)
            {
                if (SceneManager.GetActiveScene().name.Equals("TesteGamePlayOnline"))
                {
                    NetworkController.instance.Disconect();
                    Endgame();
                }
                else
                {
                    Endgame();
                }
            }
            else
            {
                StartCoroutine(Blinking(invencibleTime));
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Enemy" && is_push == true)
        {
            collision.transform.GetComponent<EnemyRun>().Divert(push_value);
        }

    }
    IEnumerator Blinking(float time)
    {
        invencible = true;
        float timer = 0;
        float currentBlink = 1f;
        float lastBlink = 0;
        float blinkPeriod = 0.1f;
        bool enabled = false;
        yield return new WaitForSeconds(0.5f);
        canMove = true;
        while (timer < time && invencible)
        {
            model.SetActive(enabled);
            //Shader.SetGlobalFloat(blinkingValue, currentBlink);
            yield return null;
            timer += Time.deltaTime;
            lastBlink += Time.deltaTime;
            if (blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1f - currentBlink;
                enabled = !enabled;
            }
        }

        model.SetActive(true);
        //Shader.SetGlobalFloat(blinkingValue, 0);
        if (is_shield == false)
        {
            invencible = false;
        }
    }
    IEnumerator CountStart()
    {
        anim.SetBool("Idle", true);
        anim.SetBool("Run", false);
        yield return new WaitForSeconds(3f);
        audio_manager.PlayFx(audio_manager.fx_running);
        speed = minSpeed;
        canMove = true;
    }
    IEnumerator Shield()
    {
        invencible = true;
        is_shield = true;
        shield.SetActive(true);
        btnShield.interactable = false;
        yield return new WaitForSeconds(10f);
        invencible = false;
        is_shield = false;
        shield.SetActive(false);
        btnShield.interactable = true;
    }
    IEnumerator RunSpeed()
    {
        speedAtual = speed;
        speed *= 2.0f;
        btnRun2.interactable = false;
        yield return new WaitForSeconds(4f);
        speed = speedAtual;
        btnRun2.interactable = true;
    }
    public void ShieldActive()
    {
        if (shields > 0)
        {
            shields--;
            uiManager.UpdateShield(shields);
            StartCoroutine(Shield());
        }

    }
    public void Damage()
    {
        if (invencible) { return; }
        currentLife--;
        //canMove = false;
        uiManager.UpdateLife(currentLife);
        speed *= 0.84f;
        invencibleTime *= 1.2f;

        if (speed <= minSpeed)
        {
            speed = minSpeed;
            invencibleTime = invencible_time_start;
        }
    }
    public void Endgame()
    {
        if (is_gameover == true) { return; }
        game_controller.temp_coins = coin;
        game_controller.temp_score = score;
        audio_manager.PlayMusic(audio_manager.game_over, false);
        speed = 0;
        canMove = false;
        anim.SetBool("Idle", true);
        anim.SetBool("Run", false);
        smokeRun.SetActive(false);
        uiManager.gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        is_gameover = true;
        game_controller.is_continue = false;
    }

    //public void WinGame()
    //{
    //    game_controller.coins += coin;
    //    audio_manager.PlayMusic(audio_manager.game_over, false);
    //    speed = 0;
    //    canMove = false;
    //    anim.SetBool("Idle", true);
    //    anim.SetBool("Run", false);
    //    smokeRun.SetActive(false);
    //    uiManager.gameWinPanel.SetActive(true);
    //    uiManager.UpdateGameWin(score, coin);
    //    Time.timeScale = 0;
    //}

    public void IncreaseSpeed()
    {
        speed *= 1.2f;
        invencibleTime *= 0.82f;
    }

    public void IncreaseRun()
    {
        if (run > 0)
        {
            run--;
            uiManager.UpdateRuns(run);
            StartCoroutine(RunSpeed());
        }

    }

    public void Divert(int value)
    {
        ChangeLane(value);
    }
}
