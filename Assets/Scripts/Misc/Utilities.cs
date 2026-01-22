using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.Networking;

public class Utils
{
    public static System.Action<bool, string> SpinnerActive;
    public static System.Action<GenMove> FadeOut; 
    public static System.Action MainUiUnselect;

    #region HELPER METHODS
    public static void DisplayAllPlayerPrefs()
    {
        Debug.Log($"Difficulty: {(BotStrength)PlayerPrefs.GetInt(Launch.Instance.persistentDataManager.gameData.difficulty_Int)}\n" +
                  $"Size: {(GenSize)PlayerPrefs.GetInt(Launch.Instance.persistentDataManager.gameData.size_Int)}\n" +
                  $"Wind amount: {PlayerPrefs.GetFloat(Launch.Instance.persistentDataManager.gameData.windAmount_Fl)} \n" +
                  $"Trajectory visible: {PlayerPrefs.GetInt(Launch.Instance.persistentDataManager.gameData.trajectoryVisible_Int)} \n");
    }

    public static Sprite SpriteFromTexture(Texture2D texture) => Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

    public static bool IsLetter(char letter)
    {
         return (letter >= 'A' && letter <= 'Z') || (letter >= 'a' && letter <= 'z');
    }

    public static string ThousandsSeparator(int num) => num.ToString("N0").Replace(',', '.');
    public static IEnumerator CheckInternetConnection(System.Action<bool> isConnected)
    {
        UnityWebRequest request = new UnityWebRequest("https://google.com");
        yield return request.SendWebRequest();
        if (request.error == null)
        {
            // Debug.Log("success, connected to internet");
            isConnected?.Invoke(true);
        }
        else
        {
            // Debug.Log("no internet connection");
            isConnected?.Invoke(false);
        }
    }
    
    public static async Task<Texture2D> GetRemoteTexture (string url)
    {
        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        // begin request:
        var asyncOp = www.SendWebRequest();

        // await until it's done: 
        while( asyncOp.isDone==false )
            await Task.Delay( 1000/30 );//30 hertz

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"{www.error}, URL:{www.url}");
            return null;
        }
        else
        {
            // return valid results:
            return DownloadHandlerTexture.GetContent(www);
        }
    }


    public static void Activation<T>(T tip, GenActivation activation, string message = "")
    {
        if (tip == null) return;

        switch (tip)
        {
            case GameObject go:
                if (go != null && go.activeSelf != (activation == GenActivation.On)) go.SetActive(activation == GenActivation.On);
                break;
            case Transform tr:
                if (tr.gameObject != null && tr.gameObject.activeSelf != (activation == GenActivation.On)) tr.gameObject.SetActive(activation == GenActivation.On);
                break;
            case Image img:
                if (img != null) img.enabled = activation == GenActivation.On;
                break;
            case Canvas can:
                if (can != null) can.enabled = activation == GenActivation.On;
                break;
            case TextMeshProUGUI textMeshPro:
                if (textMeshPro != null) textMeshPro.enabled = activation == GenActivation.On;
                break;
        }
    }

    public static void DestroyGo(GameObject go)
    {
        if (go != null) GameObject.Destroy(go);
    }

    public static GameObject[] AllChildrenGameObjects(Transform parGos)
    {
        GameObject[] gos = new GameObject[parGos.childCount];
        for (int i = 0; i < gos.Length; ++i)
        {
            gos[i] = parGos.GetChild(i).gameObject;
        }
        return gos;
    }

    public static T[] AllChildren<T>(Transform parTransform) where T : notnull
    {
        T[] children = new T[parTransform.childCount];
        for (int i = 0; i < children.Length; ++i)
        {
            children[i] = parTransform.GetChild(i).GetComponent<T>();
        }
        return children;
    }
    public static T[] AllChildrenInterfaces<T>(Transform parTransform) where T : notnull
    {
        T[] children = new T[parTransform.childCount];
        for (int i = 0; i < children.Length; ++i)
        {
            children[i] = parTransform.GetChild(i).GetComponent<T>();
        }
        return children;
    }

    public static void ActivateOneArrayElement<T>(T[] arr, int ordinal = int.MaxValue, string message = "")
    {
        for (int i = 0; i < arr.Length; i++)
        {
            Activation(arr[i], GenActivation.Off);
        }
        if (ordinal < arr.Length)
        {
            Activation(arr[ordinal], GenActivation.On, message);
        }
    }

    static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out WaitForSeconds wait)) return wait;
        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    public static List<int> RandomList(int size)
    {
        List<int> nums = Enumerable.Range(0, size).ToList();
        var rnd = new System.Random();
        var randNums = nums.OrderBy(n => rnd.Next());
        List<int> list = new List<int>();
        foreach (var item in randNums)
        {
            list.Add(item);
        }
        return list;
    }

    public static List<T> RandomListByType<T>(List<T> listToRandomize)
    {
        var rnd = new System.Random();
        var randNums = listToRandomize.OrderBy(n => rnd.Next());
        List<T> list = new List<T>();
        foreach (var item in randNums)
        {
            list.Add(item);
        }
        return list;
    }

    // public static string MyId()
    // {
    //     if (!PlayerPrefs.HasKey(leaderId_Str)) PlayerPrefs.SetString(leaderId_Str, System.Guid.NewGuid().ToString());
    //     return PlayerPrefs.GetString(leaderId_Str);
    // }

    public static string[] PurgedString(string stInput)
    {
        stInput = stInput.Replace("\r\n\r\n", ".");
        stInput = stInput.Replace("\r\n", ".");
        return stInput.Split('.');
    }

    public static string AdjustedGuid(int length = -1)
    {
        string st = System.Guid.NewGuid().ToString();
        st = st.Replace("-", "");
        if (length > 0) st = st.Substring(0, length);

        return st;
    }
    #endregion

}



#region ENUMS
public enum CampaignFinishState { None, SublevelDone, PlanetDone, GameDone }
public enum CharacterType { Player, Bot }
public enum MainGameType { MainMenu, Intro, Campaign, Practice, Multi, Tutorial }
public enum UiVisibleEndGame {Campaign, Xp, League}
public enum UiVisibleInGame { Main, Practice_Tut, Camp };
public enum AudioControls { Play, Pause, Previous, Next }
public enum League : byte
{
    Bronze1, Bronze2, Bronze3, Silver1, Silver2, Silver3, Gold1, Gold2, Gold3, Platinum1, Platinum2, Platinum3, Diamond1, Diamond2, Diamond3, Challenger
}
public enum BotStrength { Noob, Beginner, Average, Skilled, Expert, Insane }
public enum Planet { OpenSpace, Zim, Kokonotis, Jarix, Stasius, Calambara, Chronosant, Kaaman }
public enum GenActivation { On, Off }
public enum GenMove { Enter, Exit, QuickToggle }
public enum GenSize { Small, Medium, Big }
public enum GenSide { Left, Right, Center }
public enum GenResult { Win, Lose, Draw }
public enum GenChange { Increase, Decrease, Reset }
public enum TileState { Free, InActive, Taken }
public enum CloudData { Id, Name, ImageUrl, Xp, 
    League, MpTotal, MpWins, 
    Bow, Head, Gloves, Arrow, Environment, 
    Currency_Gold, Currency_Diamond,
    MatchHistory //ScorePlayer/LeaguePlayer/OpponentName/ScoreOpponent/LeagueOpponent/Datetime
}

public enum PlayerFaction
{
    First_left,
    Second_right,
    None,
    Undefined //needed for OnValueChange Callback
}
public enum BowState { RackMoving, RackDone, InHand, Free }
public enum ArrowState { Notched, Flying, Done }
#endregion

#region INTERFACES
public interface ITargetForArrow
{
    void HitMe();
}
public interface IItemCarrier
{
    public SoItem Item { get; set; }
    public GameObject MyGameObject { get; set; }
}

// public interface ILateInitialization<T>
// {
//     void InitializeMe(T parentType);
//     bool IsInitialized { get; set; }
// }
#endregion

#region NETWORK SERIALIZATION
public struct NetworkString : INetworkSerializable
{
    FixedString32Bytes _myString;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _myString);
    }
    public override string ToString()
    {
        return _myString.ToString();
    }
    public static implicit operator string(NetworkString str) => str.ToString();
    public static implicit operator NetworkString(string str) => new NetworkString()
    {
        _myString = new FixedString32Bytes(str)
    };
}

public struct NetTransform : INetworkSerializable
{
    public Vector3 pos;
    public Quaternion rot;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref pos);
        serializer.SerializeValue(ref rot);

    }
}
public struct NetHexState : INetworkSerializable
{
    public Vector2Int pos;
    public sbyte val;
    public TileState tState;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref pos);
        serializer.SerializeValue(ref val);
        serializer.SerializeValue(ref tState);
    }
}
public struct NetPlayerDisplay : INetworkSerializable, System.IEquatable<NetPlayerDisplay>
{
    public FixedString128Bytes name;
    public FixedString512Bytes imageUrl;
    public uint level;
    public byte league;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref imageUrl);
        serializer.SerializeValue(ref level);
        serializer.SerializeValue(ref league);
    }

    public bool Equals(NetPlayerDisplay other)
    {
        return name.Equals(other.name) && imageUrl.Equals(other.imageUrl) && level == other.level && league == other.league;
    }

    public override bool Equals(object obj)
    {
        return obj is NetPlayerDisplay other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(name, imageUrl, level, league);
    }
}

public struct NetPlayerEquipment : INetworkSerializable, System.IEquatable<NetPlayerEquipment>
{
    public byte bowIndex;
    public byte headIndex;
    public byte glovesIndex;
    public byte arrowIndex;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref bowIndex);
        serializer.SerializeValue(ref headIndex);
        serializer.SerializeValue(ref glovesIndex);
        serializer.SerializeValue(ref arrowIndex);
    }

    public bool Equals(NetPlayerEquipment other)
    {
        return bowIndex.Equals(other.bowIndex) && headIndex.Equals(other.headIndex) && glovesIndex.Equals(other.glovesIndex) && arrowIndex.Equals(other.arrowIndex);
    }
}


public struct LeagueWrapper : System.IEquatable<LeagueWrapper>, INetworkSerializable
{
    public League value;

    LeagueWrapper(League val)
    {
        this.value = val;
    }

    public bool Equals(LeagueWrapper other)
    {
        return value == other.value;
    }
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref value);
    }

    public static implicit operator League(LeagueWrapper wrapper) => wrapper.value;
    public static implicit operator LeagueWrapper(League value) => new LeagueWrapper(value);
}

#endregion

