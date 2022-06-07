using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 0.1f;
    public GameObject PlayerModel;

    public void Start()
    {
        PlayerModel.SetActive(false);
    }

    public void Update()
    {
        if(SceneManager.GetActiveScene().name == "Forest")
        {
            if(!PlayerModel.activeSelf)
            {
                SetPosition();
                Debug.Log("Player spawned");

                PlayerModel.SetActive(true);
                Debug.Log("Forest PlayerModel enabled");
            }

            if (hasAuthority) //Only things that you have authority over and you want to control
            {
                Movement();
            }
        }
    }

    public void SetPosition() //Spawn player
    {
        transform.position = new Vector3(Random.Range(-10, 5), 1.5f, Random.Range(-10, 10)); //Random spawn player
    }

    public void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection);

        transform.position += moveDirection * Speed;

        Debug.Log(transform.position.ToString());
    }
}
