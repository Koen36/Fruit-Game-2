using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class ProjectileObjectController : NetworkBehaviour
{
    public static ProjectileObjectController Instance;

    //Projectile Data
    [SyncVar] public int projectileID;
    [SyncVar] public int playerShotID;
    [SyncVar] public bool capable;

    //Manager
    public CustomNetworkManager manager;

    public CustomNetworkManager Manager
    {
        get
        {
            if (manager != null) //If networkmanager is already assigneds
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    public void Start()
    {
        manager.AddProjectile(this);
    }

    public void DestroyNetwork()
    {
        manager.RemoveProjectile(this);
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
