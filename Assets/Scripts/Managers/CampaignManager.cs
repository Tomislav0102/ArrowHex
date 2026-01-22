using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampaignManager : MonoBehaviour
{
    GameManager gm;
    PersistentDataManager _dm;
    SoPlayerData _pData;
    [SerializeField] RawImage botImg, playerImg;
    [SerializeField] RectTransform parentRect;
    [SerializeField] TextMeshProUGUI botNameText, playerNameText, displayEnd;
    SoPlanet _currentPlanet;

    [SerializeField] RectTransform prefabBotBubble, prefabPlayerBubble;
    const int CONST_Xpos = 2500;
    const int CONST_YDiff = 400;
    float _parentStartY;
    
    public GameObject NextLevel() => _currentPlanet?.subLevels[_pData.campProgressCurrent.subLevel].levelGo;
    
    public void Init() //no wind in campaign
    {
        gm = GameManager.Instance;
        _dm = Launch.Instance.persistentDataManager;
        _pData = _dm.playerData;
        if (_pData.campProgressCurrent.IsTutorial()) return; 
        
        _parentStartY = parentRect.anchoredPosition.y;
        
        _currentPlanet = _dm.planets[(int)_pData.campProgressCurrent.planet];
        _dm.gameData.botSp = _currentPlanet.subLevels[_pData.campProgressCurrent.subLevel].environment.bot;
        StartCoroutine(IntroDialogue(_pData.campProgressCurrent.subLevel == 0));
        
        PlayerPrefs.SetInt(_dm.gameData.difficulty_Int, (int)_dm.gameData.botSp.difficulty);
        botNameText.text = _dm.gameData.botSp.botName;
        LayoutRebuilder.ForceRebuildLayoutImmediate(botNameText.rectTransform);
        playerNameText.text = _dm.playerData.namePlayer;
        LayoutRebuilder.ForceRebuildLayoutImmediate(playerNameText.rectTransform);
        botImg.texture = gm.uiManager.playerTextures[1] = _dm.gameData.botSp.botTexture;
        playerImg.texture = gm.uiManager.playerTextures[0];
        
    }

    public CampaignFinishState PlayerVictorious(out bool isRepeating)
    {
        isRepeating = false;
        int prev = IndexFromGetCampProgress(_pData.campProgressCurrent);
        int tot = IndexFromGetCampProgress(_pData.campProgressTotal);

        if (PersistentDataManager.AllLevels.Count - prev == 1)
        {
            displayEnd.text = $"Well done! You've completed the game!";
            isRepeating = tot == prev;
            _pData.campProgressTotal = _pData.campProgressTotal = CampProgressFromIndex(prev);
            return CampaignFinishState.GameDone;
        }

        isRepeating = tot >= prev;
        
        if (prev > tot) tot++;
        prev++;
        _pData.campProgressCurrent = CampProgressFromIndex(prev);
        _pData.campProgressTotal = CampProgressFromIndex(tot);
        
        if (_pData.campProgressCurrent.planet != _currentPlanet.planet)
        {
            displayEnd.text = $"Well done! You've finished planet {_currentPlanet.PlanetName()}!";
            
            return CampaignFinishState.PlanetDone;
        }
        
        displayEnd.text = $"Level {_pData.campProgressCurrent.subLevel} finished successfully! {_currentPlanet.subLevels.Length - _pData.campProgressCurrent.subLevel} levels remain...";
        return CampaignFinishState.SublevelDone;
    }


    IEnumerator IntroDialogue(bool runAll = true)
    {
        if (runAll)
        {
            gm.uiManager.PanelDisplaying(UiVisibleInGame.Camp);
            int xPos = 0;
            RectTransform prefabToInstantiate = null;
            for (int i = 0; i < _currentPlanet.startBubbleTexts.Length; i++)
            {
                if (i > 0) yield return MoveUpwards((i + 1) * CONST_YDiff);
                switch (_currentPlanet.startBubbleTexts[i].characterType)
                {
                    case CharacterType.Bot:
                        xPos = CONST_Xpos;
                        prefabToInstantiate = prefabBotBubble;
                        break;
                    case CharacterType.Player:
                        xPos = -CONST_Xpos;
                        prefabToInstantiate = prefabPlayerBubble;
                        break;
                }
                RectTransform rt = Instantiate(prefabToInstantiate, parentRect);
                rt.anchoredPosition = new Vector2(xPos, -i * CONST_YDiff);
                
                yield return _dm.teleType.TeleTypeProcess(rt.GetComponentInChildren<TextMeshProUGUI>(), 
                    _currentPlanet.startBubbleTexts[i].text);
            }
        }
        yield return new WaitForSeconds(2f);
        gm.uiManager.BtnMethodSpCanStart();
    }

    IEnumerator MoveUpwards(float yTarget)
    {
        while (parentRect.anchoredPosition.y < yTarget + _parentStartY)
        {
            parentRect.position += new Vector3(0, Time.deltaTime, 0);
            yield return null;
        }
    }
    
    public static CampProgress CampProgressFromIndex(int index)
    {
        int sub = 0;
        Planet currPlanet = PersistentDataManager.AllLevels[index];
        for (int i = 0; i < index; i++)
        {
            if (currPlanet == PersistentDataManager.AllLevels[i]) sub++;
        }

        return new CampProgress(currPlanet, sub);
    }

    public static int IndexFromGetCampProgress(CampProgress campProgress)
    {
        int counter = 0;
        for (int i = 0; i < PersistentDataManager.AllLevels.Count; i++)
        {
            if (campProgress.planet == PersistentDataManager.AllLevels[i])
            {
                return i + campProgress.subLevel;
            }
        }

        return counter; //should never happen
    }


}





