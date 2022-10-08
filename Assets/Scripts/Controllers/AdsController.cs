using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AdsController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string player_store_id = "4913531";
    private string app_store_id = "4913530";
    private string system;

    public bool is_test_ads;
    private PlayerRun player_run;

    // Start is called before the first frame update
    void Start()
    {
        if (!Advertisement.isInitialized)
        {
            Initialized();
        }
        player_run = FindObjectOfType(typeof(PlayerRun)) as PlayerRun;
    }

    public void Initialized()
    {
        system = (Application.platform == RuntimePlatform.IPhonePlayer) ? app_store_id : player_store_id;
        Advertisement.Initialize(system, is_test_ads, this);
    }

    // falha ao exibir ads
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Time.timeScale = 1;
    }

    // começou a exibir ads
    public void OnUnityAdsShowStart(string placementId)
    {
        Time.timeScale = 0;
    }

    // click no ads
    public void OnUnityAdsShowClick(string placementId)
    {
        throw new System.NotImplementedException();
    }

    // ads exibido com sucesso
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        GameController._gameController.is_continue = true;
        GameController._gameController.RestartGame();
        Time.timeScale = 1;
    }

    // ads carregado
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Advertisement.Show(placementId, this);
    }

    // falha ao carregar ads
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        throw new System.NotImplementedException();
    }

    // ads inicializado 
    public void OnInitializationComplete()
    {
        print("Anuncio carregado com sucesso");
    }

    // falha ao inicializar ads
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        throw new System.NotImplementedException();
    }

    // ---meus metodos---
    public void PlayVideoAds()
    {
        Advertisement.Load("Rewarded_Android", this);
    }
}
