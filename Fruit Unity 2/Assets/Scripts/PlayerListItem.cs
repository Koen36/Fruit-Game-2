using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    private bool AvatarReceived;

    public TextMeshProUGUI PlayerNameText;
    public RawImage PlayerIcon;
    public GameObject UnreadyIcon;
    public GameObject ReadyIcon;
    public bool Ready;
    public GameObject HostIcon;
    public bool IsHost;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    public void ChangeReadyStatus()
    {
        if (Ready)
        {
            UnreadyIcon.SetActive(false);
            ReadyIcon.SetActive(true);
        }
        else
        {
            ReadyIcon.SetActive(false);
            UnreadyIcon.SetActive(true);
        }
    }
    
    public void ChangeHostStatus()
    {
        HostIcon.SetActive(IsHost);
    }

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    public void SetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
        ChangeReadyStatus();
        ChangeHostStatus();
        if (!AvatarReceived) //If avatar is not received
        {
            GetPlayerIcon();
        }
    }

    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if(ImageID == -1) //Check if image retrieval is succesful
        {
            return;
        }
        PlayerIcon.texture = GetSteamImageAsTexture(ImageID);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarReceived = true;
        return texture;
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if(callback.m_steamID.m_SteamID == PlayerSteamID) //If this is YOU set player icon
        {
            PlayerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else //OTHER PLAYERS
        {
            return;
        }
    }
}
