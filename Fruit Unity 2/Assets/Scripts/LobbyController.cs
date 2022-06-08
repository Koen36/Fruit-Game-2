using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using Steamworks;
using System.Linq;
using TMPro;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    public GameObject NetworkManagerPrefab;

    //UI Elements
    public TextMeshProUGUI LobbyNameText;

    //Player Data
    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    //Other Data
    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalplayerController;

    //Ready
    public Button StartGameButton;
    public TextMeshProUGUI ReadyButtonText;

    //Manager
    private CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    //READY STATUS
    public void ReadyPlayer()
    {
        LocalplayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        if (LocalplayerController.Ready)
        {
            ReadyButtonText.text = "Unready";
        }
        else
        {
            ReadyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady()
    {
        bool AllReady = false;

        foreach(PlayerObjectController player in Manager.GamePlayers)
        {
            if (player.Ready)
            {
                AllReady = true;
            }
            else
            {
                AllReady = false;
                break;
            }
        }

        if (AllReady)
        {
            if(LocalplayerController.PlayerIdNumber == 1)
            {
                StartGameButton.interactable = true;
            }
            else
            {
                StartGameButton.interactable = false;
            }
        }
        else
        {
            StartGameButton.interactable = false;
        }
    }

    //READY STATUS
    public void HostPlayer()
    {
        if (LocalplayerController.PlayerIdNumber == 1)
        {
            LocalplayerController.ChangeHost();
        }
    }

    //LOBBY
    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }


    //PLAYER LIST
    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated) //HOST
        {
            CreateHostPlayerItem();
        }
        if(PlayerListItems.Count < Manager.GamePlayers.Count) //New Client when it connects
        {
            CreateClientPlayerItem();
        }
        if(PlayerListItems.Count > Manager.GamePlayers.Count) //Remove disconnected client
        {
            RemovePlayerItem();
        }
        if (PlayerListItems.Count == Manager.GamePlayers.Count) //Update list
        {
            UpdatePlayerItem();
        }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalplayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem() //If YOU are the HOST
    {
        foreach(PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.Ready = player.Ready;
            NewPlayerItemScript.IsHost = player.IsHost;
            NewPlayerItemScript.SetPlayerValues();


            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem() //If YOU are the CLIENT
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID)) //Check if YOU are in the LIST
            {
                GameObject newPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.Ready = player.Ready;
                NewPlayerItemScript.IsHost = player.IsHost;
                NewPlayerItemScript.SetPlayerValues();


                newPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                newPlayerItem.transform.localScale = Vector3.one;

                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach(PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if(PlayerListItemScript.ConnectionID == player.ConnectionID) //Check if us pressing it
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.Ready = player.Ready;
                    PlayerListItemScript.IsHost = player.IsHost;
                    PlayerListItemScript.SetPlayerValues();
                    if (player == LocalplayerController) //If PLAYER is LOCAL
                    {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    } //UPDATE EVERYONE

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>(); //New list

        foreach(PlayerListItem playerListItem in PlayerListItems)
        {
            if(!Manager.GamePlayers.Any(b => b.ConnectionID == playerListItem.ConnectionID)) //Check if YOU are in the list
            {
                playerListItemToRemove.Add(playerListItem);
            }
        }
        if(playerListItemToRemove.Count > 0) //Removes all the players in the list
        {
            foreach (PlayerListItem playerlistItemToRemove in playerListItemToRemove)
            {
                GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
                PlayerListItems.Remove(playerlistItemToRemove);
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }

    } //REMOVE disconnected players


    //START GAME
    public void StartGame(string SceneName)
    {
        LocalplayerController.CanStartGame(SceneName);
    }


    //QUIT LOBBY
    public void QuitLobby()
    {
        //Destroy(SteamLobby.Instance);
        //Destroy(LocalPlayerObject);

        //SteamLobby.Instance.ReloadSteam();

        //Destroy(GameObject.Find("NetworkManager"));
        //Instantiate(NetworkManagerPrefab);

        SteamLobby.Instance.LeaveLobby();
        Destroy(LocalPlayerObject);
        

        SceneManager.LoadScene("MainMenu");
    }
}