using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

//[CreateAssetMenu]
public class SoFactionData : ScriptableObject
{
    [FormerlySerializedAs("playerSide")] [FormerlySerializedAs("playerColor")] public PlayerFaction playerFaction;
    public Color colMain;
    public Gradient colGradientTrail;
    public Material matMain, matFinalHex;

}
