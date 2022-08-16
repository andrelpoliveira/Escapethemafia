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
    public float minSpeed = 9f;
    public float maxSpeed = 20f;
    public float invencibleTime;
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
    private int coin;
    private int amunnition;
    // Start is called before the first frame update
    void Start()
    {
        rbPlayer = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        runAudio = GetComponent<AudioSource>();
        spawnProjectile = GetComponent<SpawnProjectile>();
        currentLife = maxLife;
        speed = minSpeed;
        blinkingValue = Shader.PropertyToID("_BlinkingValue");
        uiManager = FindObjectOfType<UiManager>();
        //collects = FindObjectOfType<Collects>();
    }

    // Update is called once per frame
    void Update()
    {
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
        if(Input.touchCount == 1)
        {
            if (isSwipe)
            {
                Vector2 diff = Input.GetTouch(0).position - startTouch;
                diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);
                if(diff.magnitude > 0.01f)
                {
                    if(Mathf.Abs(diff.y)> Mathf.Abs(diff.x))
                    {
                        if(diff.y > 0)
                        {
                            Jump();
                        }
                    }
                    else
                    {
                        if(diff.x < 0) { ChangeLane(-2); } else { ChangeLane(2); }
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
            if(ratio >= 1f)
            {
                jumping = false;
                anim.SetBool("Jump", false);
            }
            else
            {
                verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
            }
        }
        else
        {
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0,5f * Time.deltaTime);
        }
        /* ------ Jump Fim -----*/
        //Movimentação Player
        Vector3 targetPostion = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPostion, laneSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        rbPlayer.velocity = Vector3.forward * speed;
        if (!jumping) { anim.SetBool("Idle", false); anim.SetBool("Run", true); smokeRun.SetActive(true); runAudio.mute = false; }
    }
    //Change Lanes
    void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;
        if(targetLane < 0 || targetLane > 4)
        {
            return;
        }
        currentLane = targetLane;
        verticalTargetPosition = new Vector3((currentLane -2), 0,0);
    }
    //Jump
    void Jump()
    {
        if (!jumping)
        {
            smokeRun.SetActive(false);
            anim.SetBool("Run", false);
            jumpStart = transform.position.z;
            anim.SetFloat("JumpSpeed", speed / jumpLength);
            anim.SetBool("Jump", true);
            jumping = true;
            runAudio.mute = true;
        }
    }
    //Verificação das Colisões
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Coin")
        {
            coin++;
            uiManager.UpdateCoins(coin);
            other.gameObject.SetActive(false);
        }

        if(other.tag == "Heart")
        {
            if(currentLife >= 4)
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
        if(other.tag == "Ammunition")
        {
            if(spawnProjectile.currentProjectile >= 5)
            {
                spawnProjectile.currentProjectile = 5;
            }
            else
            {
                spawnProjectile.currentProjectile++;
                
                uiManager.UpdateProjectile(spawnProjectile.currentProjectile);
                other.gameObject.SetActive(false);
            }
        }

        if (invencible) { return; }

        if(other.tag == "Obstacle")
        {
            currentLife--;
            uiManager.UpdateLife(currentLife);
            anim.SetBool("Idle", true);
            anim.SetBool("Run", false);
            runAudio.mute = true;
            speed = 0;
            if(currentLife <= 0)
            {
                //
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
        yield return new WaitForSeconds(1f);
        speed = minSpeed;
        while(timer < time && invencible)
        {
            Shader.SetGlobalFloat(blinkingValue, currentBlink);
            yield return null;
            timer += Time.deltaTime;
            lastBlink += Time.deltaTime;
            if(blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1f - currentBlink;
            }
        }
        Shader.SetGlobalFloat(blinkingValue, 0);
        invencible = false;
    }
}
