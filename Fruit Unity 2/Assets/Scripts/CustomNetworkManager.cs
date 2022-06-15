using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>(); //Get information of every player
    public List<ProjectileObjectController> Projectiles { get; } = new List<ProjectileObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }

    public void AddProjectile(ProjectileObjectController projectile)
    {
        projectile.projectileID = Projectiles.Count + 1;
        projectile.playerShotID = -1; //PLAYER DIE KOGEL HEEFT GESCHOTEN, HEEFT FIX NODIG
        projectile.capable = true;
    }

    public void defuseProjectile(ProjectileObjectController projectile)
    {
        projectile.capable = false;
    }

    public void RemoveProjectile(ProjectileObjectController projectile)
    {
        Destroy(projectile);
    }

    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }
}
