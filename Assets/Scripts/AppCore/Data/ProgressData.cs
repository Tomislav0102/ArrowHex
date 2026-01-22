using System;
using Newtonsoft.Json;

[Serializable]
public class ProgressData
{
    [JsonProperty("completed")]   public LevelData[] CompletedLevels { get; set; }

    public ProgressData (LevelData[] CompletedLevels)
    {
        this.CompletedLevels = CompletedLevels;
    }

    public ProgressData(int completedLevelsNum)
    {
        CompletedLevels = new LevelData[completedLevelsNum];
    }
}

[Serializable]
public class LevelData
{
    [JsonProperty("level")]     public int Level {  get; set; }
    [JsonProperty("sublevel")]  public int Sublevel { get; set; }
    [JsonProperty("datetime")]  public string Datetime { get; set; }

    public LevelData(int level, int sublevel)
    {
        this.Level = level;
        this.Sublevel = sublevel;
    }
}
