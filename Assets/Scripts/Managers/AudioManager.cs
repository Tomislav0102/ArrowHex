using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Random = UnityEngine.Random;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    PersistentDataManager _dm;
    [SerializeField] AudioSource audioSfx;
    [SerializeField] AudioSource audioMusic;
    [SerializeField] Transform posMainMenu, posGame, posIntro;

    [Title("Hex")]
    public AudioData hexHit;
    public AudioData hexCounter;
    [Title("Bow")]
    public AudioData bowDraw;
    public AudioData bowRelease;
    public AudioData arrowHit;
    [Title("Game flow")]
    public AudioData hoverEnter;
    public AudioData uiButton;
    public AudioData gameStarted;
    public AudioData win;
    public AudioData lose;
    public AudioData draw;
    public AudioData fireworkExplosion;

    [Title("Music")]
    [SerializeField] AudioData[] musicMainMenu;
    [SerializeField] AudioData[] musicIntro;
    [SerializeField] AudioData[] musicGame;
    AudioData[] _currentMusicGroup;
    float _timer;
    int _counter;
    float _yAngle;


    [Title("DJ")]
    enum PanelState { MainMenu, Game, None }
    PanelState PState
    {
        set => Utils.ActivateOneArrayElement(panels, (int)value);
    }
    [SerializeField] Canvas canvasDj;
    [SerializeField] GameObject[] panels;
    [SerializeField] TextMeshProUGUI[] artistNameText, songNameText;
    [SerializeField] FakeButtonUi[] playButton, pauseButton, prevButton, nextButton;
    enum AutoPlayControls { First, Prev, Next, Random }

    
    void Awake()
    {
        Instance = this;
        _dm = Launch.Instance.persistentDataManager;
    }



    void Start()
    {
        DontDestroyOnLoad(this);
        for (int i = 0; i < panels.Length; i++)
        {
            playButton[i].InitializeMe(() => PlayButtonMethod(AudioControls.Play));
            pauseButton[i].InitializeMe(() => PlayButtonMethod(AudioControls.Pause));
            prevButton[i].InitializeMe(() => PlayButtonMethod(AudioControls.Previous));
            nextButton[i].InitializeMe(() => PlayButtonMethod(AudioControls.Next));
        }
        PState = PanelState.None;
        
        SceneManager.sceneLoaded += CallEv_SceneLoaded;
    }

    public void GameSceneStarted()
    {
        SceneLoadShared();
        
        PState = PanelState.Game;
        _currentMusicGroup = musicGame;
        canvasDj.transform.position = posGame.position;
        if (!NetworkManager.Singleton.IsServer) _yAngle = -90f;
        canvasDj.transform.localEulerAngles = new Vector3(0, _yAngle, 0);
        canvasDj.worldCamera = PlayerManager.Instance?.playerRigRef.mainCam;
        PlayButtonMethod(AudioControls.Play);
    }
    
   void CallEv_SceneLoaded(Scene arg0, LoadSceneMode arg1)
   {
       SceneLoadShared();
       if (arg0.name == _dm.gameData.scene_MainMenu)
       {
           PState = PanelState.MainMenu;
           _currentMusicGroup = musicMainMenu;
           canvasDj.transform.position = posMainMenu.position;
           canvasDj.worldCamera = MainMenuManager.Instance.cameraPlayer;
           PlayButtonMethod(AudioControls.Play);
       }
       else if (arg0.name == _dm.gameData.scene_Intro)
       {
           PState = PanelState.MainMenu;
           _currentMusicGroup = musicIntro;
           canvasDj.transform.position = posIntro.position;
           if (!NetworkManager.Singleton.IsServer) _yAngle = -90f;
           canvasDj.transform.localEulerAngles = new Vector3(0, _yAngle, 0);
           canvasDj.worldCamera = IntroManager.Instance.cameraPlayer;
           PlayButtonMethod(AudioControls.Play);
       }
   }
   void SceneLoadShared()
   {
       _counter = 0;
       _timer = Mathf.Infinity;
       _yAngle = 90f;
       canvasDj.transform.localEulerAngles = Vector3.zero;
       Utils.Activation(canvasDj, GenActivation.On);
   }

    void Update()
    {
        if (!audioMusic.isPlaying) return;
        
        _timer += Time.deltaTime;
        if (_timer >= audioMusic.clip.length)
        {
            _timer = 0f;
            SelectMusic(AutoPlayControls.Random);
            audioMusic.Play();
        }
    
    }

    void PlayButtonMethod(AudioControls audioControls)
    {
        switch (audioControls)
        {
            case AudioControls.Play:
                for (int i = 0; i < panels.Length; i++)
                {
                    Utils.Activation(playButton[i].gameObject, GenActivation.Off);
                    Utils.Activation(pauseButton[i].gameObject, GenActivation.On);
                }
                if (!audioMusic.isPlaying)
                {
                    SelectMusic(AutoPlayControls.First);
                    audioMusic.Play();
                }
                else
                {
                    audioMusic.UnPause();
                }
                break;
            
            case AudioControls.Pause:
                for (int i = 0; i < panels.Length; i++)
                {
                    Utils.Activation(playButton[i].gameObject, GenActivation.On);
                    Utils.Activation(pauseButton[i].gameObject, GenActivation.Off);
                }
                audioMusic.Pause();
                break;
            
            case AudioControls.Previous:
                for (int i = 0; i < panels.Length; i++)
                {
                    Utils.Activation(playButton[i].gameObject, GenActivation.Off);
                    Utils.Activation(pauseButton[i].gameObject, GenActivation.On);
                }
                _timer = 0;
                SelectMusic(AutoPlayControls.Prev);
                audioMusic.Play();
                break;
            
            case AudioControls.Next:
                for (int i = 0; i < panels.Length; i++)
                {
                    Utils.Activation(playButton[i].gameObject, GenActivation.Off);
                    Utils.Activation(pauseButton[i].gameObject, GenActivation.On);
                }
                _timer = 0;
                SelectMusic(AutoPlayControls.Next);
                audioMusic.Play();
                break;
        }
        
    }

    void SelectMusic(AutoPlayControls autoPlayControls)
    {
        switch (autoPlayControls)
        {
            case AutoPlayControls.First:
                _counter = 0;
                break;
            case AutoPlayControls.Prev:
                _counter--;
                if (_counter < 0) _counter = _currentMusicGroup.Length - 1;
                break;
            case AutoPlayControls.Next:
                _counter = (1 + _counter) % _currentMusicGroup.Length;
                break;
            case AutoPlayControls.Random:
                _counter = Random.Range(0, _currentMusicGroup.Length);
                break;
        }
        audioMusic.clip = _currentMusicGroup[_counter].clip;
        audioMusic.volume = _currentMusicGroup[_counter].vol;
        audioMusic.pitch = _currentMusicGroup[_counter].pitch;
        for (int i = 0; i < panels.Length; i++)
        {
            songNameText[i].text = $"{audioMusic.clip.name}";
        }
    }


    #region EXTERNAL CALLS
    
    public void PlaySfx(AudioData aData, bool forEveryone = true, Vector3? pos = null)
    {
        if (aData == null || aData.clip == null) return;
        if (forEveryone) PlaySfx_EveryoneRpc(aData, pos);
        else PlaySfx_Content(aData, pos);
    }

    [Rpc(SendTo.Everyone)]
    void PlaySfx_EveryoneRpc(AudioData aData, Vector3? pos = null) => PlaySfx_Content(aData, pos);

    void PlaySfx_Content(AudioData aData, Vector3? pos)
    {
        float vol = aData.vol;
        audioSfx.volume = vol;
        audioSfx.pitch = aData.pitch;
        if(pos == null)
        {
            audioSfx.clip = aData.clip;
            audioSfx.Play();
        }
        else
        {
            AudioSource.PlayClipAtPoint(aData.clip, (Vector3)pos, vol);
        }
    }

    public void PlayOnSpecificAudioSource(AudioSource source, AudioData aData, bool forEveryone = true)
    {
        if (aData == null || aData.clip == null) return;
        if (forEveryone) PlayOnSpecificAudioSource_EveryoneRpc(source, aData);
        else PlayOnSpecificAudioSource_Content(source, aData);
    }

    [Rpc(SendTo.Everyone)]
    void PlayOnSpecificAudioSource_EveryoneRpc(AudioSource source, AudioData aData) => PlayOnSpecificAudioSource_Content(source, aData);

    void PlayOnSpecificAudioSource_Content(AudioSource source, AudioData aData)
    {
        if (source.isPlaying) source.Stop();
        
        source.volume = aData.vol;
        source.pitch = aData.pitch;
        source.clip = aData.clip;
        source.Play();
    }
    #endregion



}

[System.Serializable]
public class AudioData
{
    [HideLabel, HorizontalGroup(Width = 0.5f)]
    public AudioClip clip;
    [HorizontalGroup(Width = 0f, PaddingLeft = 0.05f), LabelWidth(20)]
    public float vol = 1f;
    [HorizontalGroup(Width = 0f, PaddingLeft = 0.05f), LabelWidth(30)]
    public float pitch = 1f;
}

