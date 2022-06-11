using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager Instance;

    //Lobies List Variables
    public GameObject onlineMenu;
    public GameObject lobbiesMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;

    public List<GameObject> listOfLobbies = new List<GameObject>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void GetListOfLobbies()
    {
        onlineMenu.SetActive(false);
        lobbiesMenu.SetActive(true);

        SteamLobby.Instance.GetLobbiesList();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        for (int i=0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab); //Creates object

                createdItem.GetComponent<LobbyDataEntry>().lobbyID = (CSteamID)lobbyIDs[i].m_SteamID; //Adds lobbyID to object
                createdItem.GetComponent<LobbyDataEntry>().lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name"); //Adds lobbyName to object
                createdItem.GetComponent<LobbyDataEntry>().SetLobbyData(); //Sets lobbyname on screen

                createdItem.transform.SetParent(lobbyListContent.transform); //Sets object inside the content
                createdItem.transform.localScale = Vector3.one; //Scale the object (back) to 1

                listOfLobbies.Add(createdItem); //Adds to list of lobbies
            }
        }
    }

    public void DestroyLobbies()
    {
        foreach(GameObject lobbyItem in listOfLobbies)
        {
            Destroy(lobbyItem);
        }
        listOfLobbies.Clear();
    }

    public void Back()
    {
        onlineMenu.SetActive(true);
        lobbiesMenu.SetActive(false);
    }
}