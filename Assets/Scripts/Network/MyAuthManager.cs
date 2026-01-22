using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Oculus.Platform;
using Oculus.Platform.Models;
using Application = UnityEngine.Application;


public class MyAuthManager : MonoBehaviour
{
    PersistentDataManager _dm;
    [SerializeField] string profileName = "default";
    [SerializeField] string env = "dev";
    bool _eventsInitialized;
    OculusAuth _auth;
    string _nonce, _urlImg;
    
    string _userId;
    string _userName;
    public Texture2D texturePlayer;


    void Start()
    {
        _dm = Launch.Instance.persistentDataManager;
        DontDestroyOnLoad(this);
        Utils.SpinnerActive?.Invoke(true, "Authenticating user...");
        
        if (!Application.isEditor)
        {
            _auth = new OculusAuth();
            _auth.Init(this);
        }
        else
        {
            Authenticate();
        }
    }


    public async void DataReturnFromOculusAuth(string userID, string nonce, string urlImg, string playerName)
    {
        _userId = userID;
        _dm.playerData.id = _userId;
        _nonce = nonce;
        _urlImg = urlImg;
        _dm.playerData.imageUrl = _urlImg;
        _userName = playerName;
        _dm.playerData.namePlayer = _userName;
        SetTexture();
        await Authenticate();
    }
    async void SetTexture()
    {
        try
        {
            texturePlayer = await Utils.GetRemoteTexture(_urlImg);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }


    async Task Authenticate()
    {
        try
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                InitializationOptions options = new InitializationOptions().SetEnvironmentName(env);

               // string myProfile = "default";
                options.SetProfile(profileName);

                await UnityServices.InitializeAsync(options);
            }

            if (!_eventsInitialized) SetupEvents();

            if (Application.isEditor)
            {
               // print("Editor - anon login");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            else
            {
                if (AuthenticationService.Instance.SessionTokenExists)
                {
                   // print("session token already exists - anon login");
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                else
                {
                   // print("no session token - oculus login");
                    await AuthenticationService.Instance.SignInWithOculusAsync(_nonce, _userId);
                }
            }
            
            
            Launch.Instance.persistentDataManager.FirstDownloadFromCLoud();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    void SetupEvents()
    {
        _eventsInitialized = true;
        // AuthenticationService.Instance.SignedIn += () =>
        // {
        //     print($"Signed in as {_userName} with {AuthenticationService.Instance.PlayerId} ID");
        // };
        // AuthenticationService.Instance.SignedOut += () => { print("Signed out"); };
        // AuthenticationService.Instance.Expired += () => { print("Expired"); };
    }
}

public class OculusAuth
{
    MyAuthManager _myAuthManager;
    string _userId, _userName, _userImgUrl;

    public void Init(MyAuthManager manager)
    {
        _myAuthManager = manager;
        Core.AsyncInitialize().OnComplete(OnInitializationCallback);
    }

    private void OnInitializationCallback(Message<PlatformInitialize> msg)
    {
        if (msg.IsError)
        {
            Debug.LogErrorFormat("Oculus: Error during initialization. Error Message: {0}",
                msg.GetError().Message);
        }
        else
        {
           // Debug.Log("OnInitializationCallback");
            Entitlements.IsUserEntitledToApplication().OnComplete(OnIsEntitledCallback);
        }
    }

    private void OnIsEntitledCallback(Message msg)
    {
        if (msg.IsError)
        {
            Debug.LogErrorFormat("Oculus: Error verifying the user is entitled to the application. Error Message: {0}",
                msg.GetError().Message);
        }
        else
        {
          //  Debug.Log("OnIsEntitledCallback");
            Users.GetLoggedInUser().OnComplete(OnLoggedInUserCallback);
        }
    }


    private void OnLoggedInUserCallback(Message<User> msg)
    {
        if (msg.IsError)
        {
            Debug.LogErrorFormat("Oculus: Error getting logged in user. Error Message: {0}",
                msg.GetError().Message);
        }
        else
        {
          //  Debug.Log("OnLoggedInUserCallback");
            _userId = msg.Data.ID.ToString(); // do not use msg.Data.OculusID;
            _userName = msg.Data.OculusID; 
            _userImgUrl = msg.Data.ImageURL;
            Users.GetUserProof().OnComplete(OnUserProofCallback);
        }
    }


    private void OnUserProofCallback(Message<UserProof> msg)
    {
        if (msg.IsError)
        {
            Debug.LogErrorFormat("Oculus: Error getting user proof. Error Message: {0}",
                msg.GetError().Message);
        }
        else
        {
            string oculusNonce = msg.Data.Value;

            _myAuthManager.DataReturnFromOculusAuth(_userId, oculusNonce, _userImgUrl, _userName);

        }
    }
}



// public async Task Authenticate()
// {
//     if (UnityServices.State == ServicesInitializationState.Uninitialized)
//     {
//         InitializationOptions options = new InitializationOptions().SetEnvironmentName(env);
//
//         string myProfile = "default";
//         if (ClonesManager.IsClone())
//         {
//             myProfile = System.Guid.NewGuid().ToString();
//             myProfile = myProfile.Replace("-", "");
//             myProfile = myProfile.Substring(0, 29);
//         }
//         options.SetProfile(myProfile);
//
//         await UnityServices.InitializeAsync(options);
//     }
//
//     AuthenticationService.Instance.SignedIn += () => { print($"Signed in as {AuthenticationService.Instance.PlayerId} with { Launch.Instance.myDatabaseManager.observableData[MyData.Name]} name"); };
//
//     if (!AuthenticationService.Instance.IsSignedIn)
//     {
//         await AuthenticationService.Instance.SignInAnonymouslyAsync();
//         Launch.Instance.myDatabaseManager.observableData[MyData.Id] = AuthenticationService.Instance.PlayerId;
//     }
//
// }
