using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName; //Whenever string changes, function will be called
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;
    [SyncVar(hook = nameof(PlayerHostUpdate))] public bool IsHost;

    //Manager
    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if(manager != null) //If networkmanager is already assigneds
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }


    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    //GET INFO AND UPDATE INFO
    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString()); //Retrieves username Steam
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();

        LobbyController.Instance.HostPlayer(); //Checks if someone is Host
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }


    //PLAYER NAME
    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if (isServer) //If YOU are the HOST
        {
            this.PlayerName = NewValue;
        }
        if (isClient) //If YOU are the CLIENT
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }


    //READY STATUS
    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready); //Changes Ready to opposite
    }

    private void PlayerReadyUpdate(bool OldValue, bool NewValue)
    {
        if (isServer) //If YOU are the HOST
        {
            this.Ready = NewValue;
        }
        if (isClient) //If YOU are the CLIENT
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void ChangeReady()
    {
        if (hasAuthority)
        {
            CmdSetPlayerReady();
        }
    }


    //HOST STATUS
    [Command]
    private void CmdSetPlayerHost()
    {
        this.PlayerHostUpdate(this.IsHost, !IsHost);
    }

    public void PlayerHostUpdate(bool OldValue, bool NewValue)
    {
        if (isServer) //If YOU are the HOST
        {
            this.IsHost = NewValue;
        }
        if (isClient) //If YOU are the CLIENT
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void ChangeHost()
    {
        if (hasAuthority)
        {
            CmdSetPlayerHost();
        }
    }



    //START GAME
    [Command]
    public void CmdCanStartGame(string SceneName)
    {
        manager.StartGame(SceneName);
    }
    public void CanStartGame(string SceneName)
    {
        if (hasAuthority)
        {
            CmdCanStartGame(SceneName);
        }
    }
}
