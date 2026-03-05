using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("# BGM")]
    public AudioClip bgmClip;
    public float bgmVolume = 0.5f;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect; 
    
    [Header("# SFX")]
    public AudioClip[] sfxClips; 
    public float sfxVolume = 0.5f;
    public int channels = 16; 
    AudioSource[] sfxPlayers; 
    int channelIndex;
    
    public enum Sfx { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win }
    
    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
            Init();
        }
        else if (instance != this)
        {
            Destroy(gameObject); 
        }
    }

    void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmEffect = bgmObject.AddComponent<AudioHighPassFilter>();
        
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true; 
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            if (!bgmPlayer.isPlaying) 
            {
                bgmPlayer.Play();
            }
        }
        else 
        {
            bgmPlayer.Stop();
        }
    }
    
    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;
            
            if (sfxPlayers[loopIndex].isPlaying) continue;
            
            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play(); 
            break;
        }
    }

    void OnValidate()
    {
        if (bgmPlayer != null)
        {
            bgmPlayer.volume = bgmVolume;
        }
        
        if (sfxPlayers != null)
        {
            for (int i = 0; i < sfxPlayers.Length; i++)
            {
                if (sfxPlayers[i] != null)
                {
                    sfxPlayers[i].volume = sfxVolume;
                }
            }
        }
    }
}