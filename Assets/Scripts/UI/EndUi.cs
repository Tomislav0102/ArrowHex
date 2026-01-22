using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EndUi : MonoBehaviour
{
    PersistentDataManager _dm;
    [SerializeField] Image[] mainOutlines, circleOutlines;
    [SerializeField] Image circleImg;
    [SerializeField] TextMeshProUGUI circleCenterName, circleName, endInfo;
    [SerializeField] RectTransform prevItemFrame, nextItemFrame;
    [SerializeField] TextMeshProUGUI prevItemName, nextItemName;
    [SerializeField] Image prevItemBkgImg, prevItemImg, nextItemBkgImg, nextItemImg;
    Vector2Int _itemYpositions = new Vector2Int(-108, 67);

    [SerializeField] Transform parArrows;
    Transform[] _arrows;
    Vector3[] _arrowsStartPos;
    float _timer;
    int _counter;
    bool _isAnimating;


    public void InitializeMe(UiVisibleEndGame visibleEndGame)
    {
        _dm = Launch.Instance.persistentDataManager;
        
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
        
        _arrows = new Transform[parArrows.childCount];
        _arrowsStartPos = new Vector3[parArrows.childCount];
        for (int i = 0; i < parArrows.childCount; i++)
        {
            _arrows[i] = parArrows.GetChild(i);
            _arrowsStartPos[i] = _arrows[i].position;
        }
        _isAnimating = true;

        switch (visibleEndGame)
        {
            case UiVisibleEndGame.Campaign: //advancement in camp is done in CampaignManager.cs
                SoPlanet finishedPlanet = _dm.planets[(int)_dm.playerData.campProgressCurrent.planet - 1];
                SoPlanet currentPlanet = _dm.planets[(int)_dm.playerData.campProgressCurrent.planet];
                switch (PlayerRewards.Data.campaignFinishState)
                {
                    case CampaignFinishState.PlanetDone:
                        endInfo.text = "New planet unlocked!\n<size=60%><color=#8482AF>You reached a new planet!";
                        FillItems(currentPlanet.planetSprite, currentPlanet.PlanetName(), finishedPlanet.planetSprite, finishedPlanet.PlanetName());
                        break;
                    case CampaignFinishState.GameDone:
                        //UI design is not defined
                        endInfo.text = "Game finished!\n<size=60%><color=#8482AF>You completed all planets!";
                        break;
                }
                break;
            case UiVisibleEndGame.Xp: //advancement in xp is done in UiManager.cs
                PlayerRewards.CalculateLevelFromXp(out int currentLevel, out _);
                currentLevel++;
                endInfo.text = "Level up!\n<size=60%><color=#8482AF>You reached a new level!";
                circleCenterName.text = currentLevel.ToString();
                FillItems(_dm.gameData.LevelIcon(currentLevel), $"LV. {currentLevel}",_dm.gameData.LevelIcon(currentLevel - 1), $"LV. {currentLevel - 1}");
                break;
            case UiVisibleEndGame.League: //advancement in league is done in LeagueProgressionUi.cs and UiManager.cs
                int currentLeague = _dm.playerData.league;
                int prevLeague = 0;
                switch (PlayerRewards.Data.leagueChange)
                {
                    case GenChange.Increase:
                        endInfo.text = "New league!\n<size=60%><color=#8482AF>You reached a new league!";
                        prevLeague = currentLeague - 1;
                        FillItems(_dm.gameData.leagueSprites[currentLeague], _dm.gameData.LeagueName(currentLeague),_dm.gameData.leagueSprites[prevLeague], _dm.gameData.LeagueName(prevLeague));
                        break;
                    case GenChange.Decrease:
                        Utils.Activation(prevItemBkgImg, GenActivation.On);
                        Utils.Activation(nextItemBkgImg, GenActivation.Off);
                        prevItemFrame.anchoredPosition = new Vector2(prevItemFrame.anchoredPosition.x, _itemYpositions.y);
                        nextItemFrame.anchoredPosition = new Vector2(nextItemFrame.anchoredPosition.x, _itemYpositions.x);
                        parArrows.localScale = new Vector3(-1, 1, 1);
                        endInfo.text = "Demoted to a lower league!\n<size=60%><color=#8482AF>Looks like you’ve dropped to a lower league\nbased on recent performance.";
                        prevLeague = currentLeague + 1;
                        FillItems(_dm.gameData.leagueSprites[currentLeague], _dm.gameData.LeagueName(currentLeague),_dm.gameData.leagueSprites[prevLeague], _dm.gameData.LeagueName(prevLeague));
                        break;
                    case GenChange.Reset:
                        //UI design is not defined
                        endInfo.text = "Your league progress has been reset!";
                        break;
                }
                break;
        }

        void FillItems(Sprite fillCircleImg, string fillCircleName, Sprite fillPrevItemImg, string fillPrevItemName)
        {
            circleImg.sprite = fillCircleImg;
            circleName.text = fillCircleName;
            LayoutRebuilder.ForceRebuildLayoutImmediate(circleName.rectTransform);
            prevItemName.text = fillPrevItemName;
            LayoutRebuilder.ForceRebuildLayoutImmediate(prevItemName.rectTransform);
            prevItemImg.sprite = fillPrevItemImg;
            nextItemName.text = fillCircleName;
            LayoutRebuilder.ForceRebuildLayoutImmediate(nextItemName.rectTransform);
            nextItemImg.sprite = fillCircleImg;

        }
    }

    void Update()
    {
        ArrowAnimation();
    }

    void ArrowAnimation()
    {
        if (!_isAnimating) return;
        
        _timer += Time.deltaTime;
        if (_timer > 0.3f)
        {
            _timer = 0f;
            _counter++;
            for (int i = 0; i < 3; i++)
            {
                int nextPos = (_counter + i) % 3;
                _arrows[i].position = _arrowsStartPos[nextPos];
            }
        }
    }
}
