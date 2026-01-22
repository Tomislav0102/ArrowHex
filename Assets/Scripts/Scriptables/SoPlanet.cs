using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public class SoPlanet : ScriptableObject
{
    public Planet planet;
    public string[] descriptions = new string[4]; 
    public string PlanetName() => planet.ToString().Replace("_", " ");
    public Sprite planetSprite;
    public SubLevel[] subLevels;
    public SingleBubbleText[] startBubbleTexts;

    [System.Serializable]
    public struct SingleBubbleText
    {
        public CharacterType characterType;
        public string text;
    }

    [System.Serializable]
    public struct SubLevel
    {
        public SoEnvironment environment;
        public GameObject levelGo;
    }
}


