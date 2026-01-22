using System;

[Serializable]
public class CampaignData
{
    public int level;
    public int sublevel;

    public CampaignData(int level, int sublevel)
    {
        this.level = level;
        this.sublevel = sublevel;
    }
}
