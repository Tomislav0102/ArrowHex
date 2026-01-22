using Unity.Netcode;
using UnityEngine;

public class ArrowMain : MonoBehaviour
{
    protected GameManager gm;

    [SerializeField] Transform parTrails, parMeshes;
    GameObject[] _trails, _meshes;
    int _currentIndex;
    public Transform tip;
    public Transform myTransform;
    public ArrowState myArrowState;

    protected void Awake()
    {
        gm = GameManager.Instance;
        _trails = Utils.AllChildrenGameObjects(parTrails);
        _meshes = Utils.AllChildrenGameObjects(parMeshes);
        _currentIndex = Launch.Instance.persistentDataManager.playerData.arrowItem.id - Launch.Instance.persistentDataManager.gameData.IndicesForScriptables[CloudData.Arrow];
    }


    public virtual void Release(Vector3 vel)
    {
        myArrowState = ArrowState.Flying;
        //print("arrowMain released");
        gm.arrowManager.Trails_EveryoneRpc();
    }

    public void SetArrow(byte index)
    {
        _currentIndex = index;
        Utils.ActivateOneArrayElement(_meshes, _currentIndex);
    }
    public void SetTrail()
    {
        Utils.ActivateOneArrayElement(_trails, _currentIndex);
        if (_currentIndex == 0)
        {
            _trails[0].GetComponent<TrailRenderer>().colorGradient = gm.factionData[(int)gm.playerTurnNet.Value].colGradientTrail;
        }
        // if (_currentTrail.enabled) return;
        // _currentTrail.enabled = true;
        // _currentTrail.colorGradient = gm.playerDatas[(int)gm.playerTurnNet.Value].colGradientTrail;

    }


}
