using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Items/Environment")]
public class SoEnvironment : SoItem
{
    public string Name() => this.name.Substring(this.name.IndexOf('-') + 1);
    [TextArea]
    public string description;
    public Planet myPlanet; //hopefully redundant (bad coding practice)
    
    public Material skyboxMaterial;
    public SoBot bot;
    public bool hasHeightFog;
    [ShowIf(nameof(hasHeightFog))]
    public Color fogColor;

    
    void Awake()
    {
        dataType = CloudData.Environment;
    }

}
