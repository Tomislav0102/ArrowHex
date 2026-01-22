using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class IntroManager : MonoBehaviour
{
    public static IntroManager Instance;
    [SerializeField] Canvas canvas;
    [Title("Alien")]
    [SerializeField] Transform alienTransform;
    [SerializeField] Animator alienAnimator;
    [SerializeField] RectTransform alienAnswerContainer;
    [Title("Player")]
    public Camera cameraPlayer;
    [SerializeField] VerticalLayoutGroup playerLayoutGroup;
    [SerializeField] RectTransform parPlayerQuestions;
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] RawImage playerImage;
    RectTransform _currentAnswer; 
    QuestionAnswer _currentSet;
    int _talk = Animator.StringToHash("talk");
    int _counter;
    
    [Space]
    [Title("Question and answers")]
    [SerializeField] QuestionAnswer[] qaSets;
    [SerializeField] FakeButtonUi fakeButtonBackToMainMenu, fakeButtonPlay;
   
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerNameText.text = Launch.Instance.persistentDataManager.playerData.namePlayer;
        playerImage.texture = Launch.Instance.myAuthManager.texturePlayer;
        LayoutRebuilder.ForceRebuildLayoutImmediate(playerNameText.rectTransform);
       
        Utils.Activation(canvas, GenActivation.Off);
        alienTransform.DOMoveY(-3.26f, 3f)
            .OnComplete(() =>
            {
                InsertAnswer(0);
                Utils.Activation(canvas, GenActivation.On);
                playerLayoutGroup.enabled = true;
                for (int i = 1; i < qaSets.Length; i++)
                {
                    int index = i;
                    qaSets[i].question.InitializeMe(() => InsertAnswer(index));
                }

                fakeButtonBackToMainMenu.InitializeMe(() =>
                {
                    StartCoroutine(Launch.Instance.persistentDataManager.NewSceneAfterFadeIn("intro1", MainGameType.MainMenu));
                });
                fakeButtonPlay.InitializeMe(() =>
                {
                    StartCoroutine(Launch.Instance.persistentDataManager.NewSceneAfterFadeIn("intro2", MainGameType.Campaign));
                });
            });
    }


    void InsertAnswer(int index)
    {
        StopAllCoroutines();
        Launch.Instance.persistentDataManager.teleType.StopAllCoroutines();
        _counter = 0;
        _currentSet = qaSets[index];
        if (index == 1)
        {
            for (int i = 0; i < parPlayerQuestions.childCount; i++)
            {
                Utils.Activation(parPlayerQuestions.GetChild(i).gameObject, i == 0 ? GenActivation.Off : GenActivation.On);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(parPlayerQuestions);
        }
        
        for (int i = 0; i < alienAnswerContainer.childCount; i++)
        {
            Destroy(alienAnswerContainer.GetChild(i).gameObject);
        }
        _currentAnswer = Instantiate(_currentSet.answers[_counter], alienAnswerContainer);
        StartCoroutine(TeleTypeEnumerator());
    }

    IEnumerator TeleTypeEnumerator()
    {
        while (_counter < _currentSet.answers.Length)
        {
            alienAnimator.SetBool(_talk, true);
            TMP_Text tText = _currentAnswer.GetComponentInChildren<TMP_Text>();
            yield return Launch.Instance.persistentDataManager.teleType.TeleTypeProcess(tText, tText.text);
            alienAnimator.SetBool(_talk, false);
            yield return new WaitForSeconds(1f);
            _counter++;
            if (_counter < _currentSet.answers.Length)
            {
                _currentAnswer = Instantiate(_currentSet.answers[_counter], alienAnswerContainer);
            }
        }
        alienAnimator.SetBool(_talk, false);
    }
    
    [System.Serializable]
    struct QuestionAnswer
    {
        public FakeButtonUi question;
        public RectTransform[] answers;
    }
}

    // void Start()
    // {
    //     playerNameText.text = Launch.Instance.myAuthManager.userName;
    //     playerImage.texture = Launch.Instance.myAuthManager.texturePlayer;
    //     LayoutRebuilder.ForceRebuildLayoutImmediate(playerNameText.rectTransform);
    //    
    //     Utils.Activation(canvas, GenActivation.Off);
    //     alienTransform.DOMoveY(-3.26f, 3f)
    //         .OnComplete(() =>
    //         {
    //             InsertAnswer(0);
    //             Utils.Activation(canvas, GenActivation.On);
    //             playerLayoutGroup.enabled = true;
    //             for (int i = 1; i < qaSets.Length; i++)
    //             {
    //                 int index = i;
    //                 qaSets[i].question.InitializeMe(() => InsertAnswer(index));
    //             }
    //
    //             fakeButtonBackToMainMenu.InitializeMe(() =>
    //             {
    //                 StartCoroutine(Launch.Instance.mySceneManager.NewSceneAfterFadeIn("intro1", MainGameType.MainMenu));
    //             });
    //             fakeButtonPlay.InitializeMe(() =>
    //             {
    //                 StartCoroutine(Launch.Instance.mySceneManager.NewSceneAfterFadeIn("intro2", MainGameType.Campaign));
    //             });
    //         });
    // }
    //
    //
    // void InsertAnswer(int index)
    // {
    //     StopAllCoroutines();
    //     Launch.Instance.myDatabaseManager.teleType.StopAllCoroutines();
    //     _counter = 0;
    //     _currentSet = qaSets[index];
    //     if (index == 1)
    //     {
    //         for (int i = 0; i < parPlayerQuestions.childCount; i++)
    //         {
    //             Utils.Activation(parPlayerQuestions.GetChild(i).gameObject, i == 0 ? GenActivation.Off : GenActivation.On);
    //         }
    //         LayoutRebuilder.ForceRebuildLayoutImmediate(parPlayerQuestions);
    //     }
    //     
    //     SpawnAnswer();
    //     StartCoroutine(TeleTypeEnumerator());
    // }
    //
    // void SpawnAnswer()
    // {
    //     for (int i = 0; i < alienAnswerContainer.childCount; i++)
    //     {
    //         Destroy(alienAnswerContainer.GetChild(i).gameObject);
    //     }
    //     _currentAnswer = Instantiate(_currentSet.answers[_counter], alienAnswerContainer);
    //     _currentAnswer.anchoredPosition += CONST_PosY * Vector2.up;
    // }
    //
    // IEnumerator TeleTypeEnumerator()
    // {
    //     while (_counter < _currentSet.answers.Length)
    //     {
    //         alienAnimator.SetBool(_talk, true);
    //         TMP_Text tText = _currentAnswer.GetComponentInChildren<TMP_Text>();
    //         yield return Launch.Instance.myDatabaseManager.teleType.TeleTypeProcess(tText, tText.text);
    //         alienAnimator.SetBool(_talk, false);
    //         yield return new WaitForSeconds(1f);
    //         _counter++;
    //         if (_counter < _currentSet.answers.Length)
    //         {
    //             SpawnAnswer();
    //         }
    //     }
    //     alienAnimator.SetBool(_talk, false);
    // }
