using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowPredicitionUi : MonoBehaviour
{
    [SerializeField] Button btnArrowPredicition;
    GameObject[] _arrowPredicitionVisuals = new GameObject[2];

    private void Awake()
    {
        for (int i = 0; i < 2; i++)
        {
            _arrowPredicitionVisuals[i] = btnArrowPredicition.transform.GetChild(i).gameObject;
        }
    }

    private void Start()
    {
        BtnMethodTrajectory(true);

        btnArrowPredicition.onClick.AddListener(() => BtnMethodTrajectory(false));

    }

    void BtnMethodTrajectory(bool initialization = false)
    {
        bool visible = PlayerPrefs.GetInt(Launch.Instance.persistentDataManager.gameData.trajectoryVisible_Int) == 1;
        for (int i = 0; i < 2; i++)
        {
            _arrowPredicitionVisuals[i].SetActive(false);
        }
        if (!initialization)
        {
            visible = !visible;
            AudioManager.Instance.PlaySfx(AudioManager.Instance.uiButton);
        }
        PlayerPrefs.SetInt(Launch.Instance.persistentDataManager.gameData.trajectoryVisible_Int, visible ? 1 : 0);
        _arrowPredicitionVisuals[PlayerPrefs.GetInt(Launch.Instance.persistentDataManager.gameData.trajectoryVisible_Int)].SetActive(true);
    }

}
