using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class LobbyDataEntry : MonoBehaviour
{
    //Data
    public CSteamID lobbyID;
    public string lobbyName;
    public TextMeshProUGUI lobbyNameText;


    public void SetLobbyData()
    {
        if (lobbyName == "")
        {
            lobbyNameText.text = "Empty";
        }
        else
        {
            lobbyNameText.text = lobbyName;
        }
    }

    public void JoinLobby()
    {
        SteamLobby.Instance.JoinLobby(lobbyID);
    }
}
