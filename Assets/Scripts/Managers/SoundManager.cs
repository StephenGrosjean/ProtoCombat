using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    public static SoundManager _instance;
    List<AudioSource> emitters = new List<AudioSource>();

    public enum MusicList
    {
        MENU,
        INGAME,
        LASTGAME,
        NONE
    }
    MusicList currentMusicPlaying = MusicList.NONE;

    public enum SoundList
    {
        FIRE,
        EXPLOSION,
        SHIELD,
        STRIKE
    }

    public struct LoopedSound
    {
        public AudioSource audioSource;
        public float timeUntilStop;
    }
    List<LoopedSound> loopedSoundList = new List<LoopedSound>();

    List<AudioClip> listHitSounds = new List<AudioClip>();

    [Header("VolumeSounds")]
    [SerializeField] AudioMixer audioMixer;

    [Header("Musics")]
    [SerializeField] AudioClip menuMusicClip;
    [SerializeField] AudioClip inGameMusicClip;
    [SerializeField] AudioClip lastMusicClip;

    [Header("Tank Effects")]
    [SerializeField] List<AudioClip> listShieldSoundClip;
    [SerializeField] AudioClip strikSoundClip;

    [Header("Shoot Effects")]
    [SerializeField] AudioClip shootSoundClip;
    [SerializeField] AudioClip explosionSoundClip;

    [Header("Sounds")]
    [SerializeField] AudioClip winSoundClip;
    [SerializeField] AudioClip loseSoundClip;

    [Header("HitClips")]
    [SerializeField] AudioClip hitClip1;
    [SerializeField] AudioClip hitClip2;

    [Header("Emmiters")]
    [SerializeField] GameObject emitterPrefab;
    [SerializeField] int emitterNumber;

    [SerializeField] AudioSource musicEmitter;

    private void Awake()
    {
        if(_instance)
            Destroy(gameObject);
        else
            _instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(gameObject);

        for(int i = 0; i <= emitterNumber;i++)
        {
            GameObject audioObject = Instantiate(emitterPrefab, emitterPrefab.transform.position, emitterPrefab.transform.rotation);
            emitters.Add(audioObject.GetComponent<AudioSource>());
            DontDestroyOnLoad(audioObject);
        }

        listHitSounds = new List<AudioClip> { hitClip1, hitClip2};


        PlayMusic(MusicList.MENU);
    }

    private void Update()
    {
        foreach(LoopedSound loopedSound in loopedSoundList)
        {
            if(Utility.IsOver(loopedSound.timeUntilStop))
            {
                loopedSound.audioSource.Stop();
                loopedSoundList.Remove(loopedSound);
                break;
            }
        }
    }

    public AudioSource PlaySound(SoundList sound, float timeToLoop = 0.0f)
    {
        //return null;
        AudioSource emitterAvailable = null;

        foreach(AudioSource emitter in emitters)
        {
            if(!emitter.isPlaying)
            {
                emitterAvailable = emitter;
            }
        }

        if (emitterAvailable != null)
        {
            emitterAvailable.loop = false;
            int index = 0;
            switch (sound)
            {
                case SoundList.FIRE:
                    emitterAvailable.clip = shootSoundClip;
                    emitterAvailable.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Effect")[0];
                    break;
                case SoundList.EXPLOSION:
                    emitterAvailable.clip = explosionSoundClip;
                    emitterAvailable.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Effect")[0];
                    break;
                case SoundList.SHIELD:
                    index = Random.Range(0, listShieldSoundClip.Count);
                    emitterAvailable.clip = listShieldSoundClip[index];
                    emitterAvailable.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Effect")[0];
                    break;
                case SoundList.STRIKE:
                    emitterAvailable.clip = strikSoundClip;
                    emitterAvailable.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Effect")[0];
                    break;
            }

            if (timeToLoop > 0.0f)
            {
                emitterAvailable.loop = true;
                LoopedSound newLoopSound = new LoopedSound
                {
                    audioSource = emitterAvailable,
                    timeUntilStop = Utility.StartTimer(timeToLoop)
                };
                loopedSoundList.Add(newLoopSound);  
            }
            
            emitterAvailable.Play();
            return emitterAvailable;
        }
        else
        {
            Debug.Log("no emitter available");
            return null;
        }
    }

    public void PlayMusic(MusicList music)
    {
        if (currentMusicPlaying != music)
        {
            musicEmitter.loop = true;

            switch (music)
            {
                case MusicList.MENU:
                    musicEmitter.clip = menuMusicClip;
                    musicEmitter.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
                    musicEmitter.Play();
                    break;

                case MusicList.INGAME:
                    musicEmitter.clip = inGameMusicClip;
                    musicEmitter.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
                    musicEmitter.Play();
                    break;
                case MusicList.NONE:
                    musicEmitter.Stop();
                    break;
            }
            currentMusicPlaying = music;
        }
    }


    public void StopSound(AudioSource source)
    {
        source.Stop();
        foreach(LoopedSound looped in loopedSoundList)
        {
            if(looped.audioSource == source)
            {
                loopedSoundList.Remove(looped);
                break;
            }
        }
    }
}
