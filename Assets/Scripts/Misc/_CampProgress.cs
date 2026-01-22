[System.Serializable]
public class CampProgress
{
    public Planet planet;
    public int subLevel;

    public CampProgress(Planet planet, int subLevel)
    {
        this.planet = planet;
        this.subLevel = subLevel;
    }

    public bool IsTutorial() => planet == Planet.OpenSpace;
    public bool ShowIntro() => planet == Planet.Zim && subLevel == 0;
}