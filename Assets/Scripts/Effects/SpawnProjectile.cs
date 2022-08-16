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

    //Vari�veis Privadas
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

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButton (0)){ SpawnFx(); }
    }

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
            //
        }
        
    }
}
