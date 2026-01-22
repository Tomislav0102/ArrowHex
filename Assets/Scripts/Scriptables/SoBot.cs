using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SoBot : ScriptableObject
{
  //  public SoEnvironment myEnvironment; //hopefully redundant (bad coding practice)
    public string botName;
    [TextArea]
    public string description;
    [FormerlySerializedAs("botSprite")] public Texture2D botTexture;
    public BotStrength difficulty;
}
