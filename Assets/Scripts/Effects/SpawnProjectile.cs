using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnProjectile : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject firepoint;
    public List<GameObject> vfx = new List<GameObject>();
    public int maxProjectile = 5;

    [Header("UI")]
    private Button btnAction;
    private GameObject effectToSpawn;

    //Variáveis Privadas
    public int currentProjectile;

    //Scripts
    public UiManager _uiManager;
    // Start is called before the first frame update
    void Start()
    {
        btnAction = GameObject.Find("BtnAction").GetComponent<Button>() as Button;
        _uiManager = FindObjectOfType<UiManager>();
        btnAction.onClick.AddListener(SpawnFx);
        effectToSpawn = vfx[0];
        currentProjectile = maxProjectile;
    }
    //Spawn do projétil
    void SpawnFx()
    {
        GameObject vfx;
        currentProjectile--;
        if(currentProjectile >= 0)
        {
            _uiManager.UpdateProjectile(currentProjectile);
            if (firepoint != null)
            {
                vfx = Instantiate(effectToSpawn, firepoint.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.Log("No Fire Point");
            }
        }
        else
        {
            currentProjectile = 0;
            _uiManager.UpdateProjectile(currentProjectile);
        }
        
    }
}
