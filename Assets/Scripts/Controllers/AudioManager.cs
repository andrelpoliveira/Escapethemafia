using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource music;
    public AudioSource fx;

    [Header("Music")]
    public AudioClip title;
    public AudioClip intro_stage;
    public AudioClip loop_stage;
    public AudioClip game_over;
    public AudioClip play_again;
    public AudioClip power_up;

    [Space]
    [Header("Fx")]
    public AudioClip fx_coin;
    public AudioClip fx_jump;

    [Space]
    [Header("Speak")]
    public AudioClip fx_its_me;
    public AudioClip fx_game_over;
    public AudioClip fx_start;
    public AudioClip fx_title;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // MEUS METODOS
    public void PlayMusic(AudioClip clip, bool loop)
    {
        music.loop = loop;
        music.clip = clip;
        music.Play();
    }

    public void PlayFx(AudioClip clip)
    {
        fx.PlayOneShot(clip);
    }

    public IEnumerator ChangeMusic()
    {
        PlayMusic(intro_stage, false);
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => !music.isPlaying);
        PlayMusic(loop_stage, true);
    }
}
