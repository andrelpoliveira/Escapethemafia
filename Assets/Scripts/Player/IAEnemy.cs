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

public class IAEnemy : MonoBehaviour
{
    // controle para dificuldade da ia, quanto maior o valor maior chance de executar
    public int percentage_ia;
    public float delay_ia;
    public float rayDistance;
    public float distanceJump;

    private EnemyRun enemyRun;
    private SpawnProjectileEnemy spawnProjectile;
    //controle de dificuldade do inimigo
    private Difficult difficult_enemy;
    private EnemyState current_state;
    //valor da posição que ia precisa ir
    private string side;
    //pegando obj player
    private GameObject plr;
    // olho da ia
    RaycastHit hit_info;
    private bool isChange;
    //lista de obj na colisão

    // Start is called before the first frame update
    void Start()
    {
        enemyRun = GetComponent<EnemyRun>();
        spawnProjectile = GetComponent<SpawnProjectileEnemy>();
        difficult_enemy = Difficult.EASY;
        rayDistance = 10;
    }

    // Update is called once per frame
    void Update()
    {
        RayCastVertical();
        RayCastHorizontal();
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * 5, Color.red);
        Debug.DrawRay(transform.position + Vector3.up, -transform.forward * 5, Color.blue);
        Debug.DrawRay(transform.position + Vector3.up, transform.right * 5, Color.green);
        Debug.DrawRay(transform.position + Vector3.up, -transform.right * 5, Color.green);
        Debug.DrawRay(transform.position + Vector3.up, -transform.up * 2, Color.magenta);
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.tag == "Obstacle")
    //    {
    //        print($"valor da lista {objHit.Count}");
    //        for (int i = 0; i < objHit.Count; i++)
    //        {
    //            if (i + 1 < objHit.Count)
    //            {
    //                print($"distancia dos obj {Vector3.Distance(objHit[i].transform.position, objHit[i + 1].transform.position)}");
    //                if (Vector3.Distance(objHit[i].transform.position, objHit[i + 1].transform.position) > distanceJump && isChange == false)
    //                {
    //                    isChange = true;
    //                    objHit.Clear();
    //                    if (RandIA(50) == true)
    //                    {
    //                        OnStateEnter(EnemyState.JUMP);
    //                    }
    //                    else
    //                    {
    //                        CheckLane();
    //                        OnStateEnter(EnemyState.DIVERT);
    //                    }
    //                }
    //                else
    //                {
    //                    CheckLane();
    //                    OnStateEnter(EnemyState.DIVERT);
    //                }
    //            }
    //            else
    //            {
    //                CheckLane();
    //                OnStateEnter(EnemyState.DIVERT);
    //            }
    //        }

    //        //        if (Vector3.Distance(transform.position, other.transform.position) <= distanceJump && isChange == false)
    //        //        {
    //        //            isChange = true;

    //        //            if (RandIA(50) == true)
    //        //            {
    //        //                OnStateEnter(EnemyState.JUMP);
    //        //            }
    //        //            else
    //        //            {
    //        //                CheckLane();
    //        //                OnStateEnter(EnemyState.DIVERT);
    //        //            }
    //        //        }
    //    }
    //}

    // dectção de colisões frente e trás
    void RayCastVertical()
    {
        //if (!Physics.Raycast(transform.position + Vector3.up, -transform.up, out hit_info, 10))
        //{
        //    enemyRun.Endgame();
        //}

        //frente
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit_info, rayDistance))
        {
            if (hit_info.collider.tag == "Obstacle" && RandIA(percentage_ia) == true)
            {
                if (hit_info.transform.name.Equals("Bus(Clone)"))
                {
                    CheckLane();
                    OnStateEnter(EnemyState.DIVERT);
                }
                else if (Vector3.Distance(hit_info.transform.position, transform.position) <= distanceJump && Vector3.Distance(hit_info.transform.position, transform.position) >= 8f && isChange == false)
                {
                    isChange = true;
                    OnStateEnter(EnemyState.JUMP);
                }
            }
            else
            {
                CheckLane();
                OnStateEnter(EnemyState.DIVERT);
            }

            if ((hit_info.collider.tag == "Player" || hit_info.collider.tag == "Enemy") && 
                RandIA(percentage_ia) == true && hit_info.distance >= 3)
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

        //trás
        if (Physics.Raycast(transform.position + Vector3.up, -transform.forward, out hit_info, 5))
        {
            if (hit_info.collider.tag == "Player" && RandIA(percentage_ia) == true)
            {
                OnStateEnter(EnemyState.JUMP);
            }
        }
    }

    // dectção de colisões esquerda e direita
    void RayCastHorizontal()
    {
        RaycastHit hit_info;

        //direita
        if (Physics.Raycast(transform.position + Vector3.up, transform.right, out hit_info, 5))
        {
            // coleta moeda
            if (hit_info.collider.tag == "Coin" && RandIA(percentage_ia) == true)
            {
                //OnStateEnter(EnemyState.DIVERT);
                side = "right";
            }

            // empura player
            if (hit_info.collider.tag == "Player" && RandIA(percentage_ia) == true)
            {
                OnStateEnter(EnemyState.PUSH);
                plr = hit_info.collider.gameObject;
                side = "right";
            }

            //coleta de munição
            if (hit_info.collider.tag == "Ammunition" && RandIA(percentage_ia) == true)
            {
                //OnStateEnter(EnemyState.DIVERT);
                side = "right";
            }

            //coleta de vida
            if (hit_info.collider.tag == "Heart" && RandIA(percentage_ia) == true)
            {
                //OnStateEnter(EnemyState.DIVERT);
                side = "right";
            }
        }

        //esqueda
        if (Physics.Raycast(transform.position + Vector3.up, -transform.right, out hit_info, 5))
        {
            // coleta moeda
            if (hit_info.collider.tag == "Coin" && RandIA(percentage_ia) == true)
            {
                //OnStateEnter(EnemyState.DIVERT);
                side = "left";
            }

            // empura player
            if (hit_info.collider.tag == "Player" && RandIA(percentage_ia) == true)
            {
                OnStateEnter(EnemyState.PUSH);
                plr = hit_info.collider.gameObject;
                side = "left";
            }

            // coleta munição
            if (hit_info.collider.tag == "Ammunition" && RandIA(percentage_ia) == true)
            {
                //OnStateEnter(EnemyState.DIVERT);
                side = "left";
            }

            // coleta vida
            if (hit_info.collider.tag == "Heart" && RandIA(percentage_ia) == true)
            {
                //OnStateEnter(EnemyState.DIVERT);
                side = "left";
            }
        }
    }

    //maquina de estado
    public void OnStateEnter(EnemyState new_enemy_state)
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
                if (enemyRun.speed > 12)
                {
                    StartCoroutine(PushEstate());
                }
                break;

            case EnemyState.DIVERT:
                StartCoroutine(DivertState());
                break;
        }
    }

    public void CheckLane()
    {
        if (enemyRun.currentLane + 2 < 6 && enemyRun.currentLane - 2 > -2)
        {
            if (RandIA(50) == true)
            {
                side = "right";
            }
            else
            {
                side = "left";
            }
        }
        else if (enemyRun.currentLane - 2 < -2)
        {
            side = "right";
        }
        else if (enemyRun.currentLane + 2 > 6)
        {
            side = "left";
        }
    }

    //funções da ia troca de raia
    IEnumerator DivertState()
    {
        if (side == "right")
        {
            enemyRun.ChangeLane(2);
            yield return new WaitForSeconds(delay_ia);
            isChange = false;
        }

        if (side == "left")
        {
            enemyRun.ChangeLane(-2);
            yield return new WaitForSeconds(delay_ia);
            isChange = false;
        }
    }

    //funções da ia pulo
    IEnumerator JumpState()
    {
        enemyRun.Jump();
        yield return new WaitForSeconds(delay_ia);
        isChange = false;
    }

    //funções da ia empurar player
    IEnumerator PushEstate()
    {
        if (side == "right")
        {
            enemyRun.ChangeLane(2);

            if (plr.GetComponent<PlayerRun>() != null)
            {
                plr.GetComponent<PlayerRun>().Divert(2);
            }
            else if (plr.GetComponent<EnemyRun>() != null)
            {
                plr.GetComponent<EnemyRun>().Divert(2);
            }
            yield return new WaitForSeconds(delay_ia);
            isChange = false;
        }

        if (side == "left")
        {
            enemyRun.ChangeLane(-2);
            if (plr.GetComponent<PlayerRun>() != null)
            {
                plr.GetComponent<PlayerRun>().Divert(-2);
            }
            else if (plr.GetComponent<EnemyRun>() != null)
            {
                plr.GetComponent<EnemyRun>().Divert(-2);
            }
            yield return new WaitForSeconds(delay_ia);
            isChange = false;
        }
    }

    //funções da ia tiro
    IEnumerator FireState()
    {
        spawnProjectile.SpawnFx(enemyRun.speed);
        yield return new WaitForSeconds(delay_ia);
        isChange = false;
    }

    //metodo para decidir o que a ia vai fazer
    bool RandIA(int value)
    {
        int temp = Random.Range(0, 100);
        bool retorno = temp <= value ? true : false;
        return retorno;
    }
}
