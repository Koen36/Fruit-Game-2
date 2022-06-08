using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 10f;
    public GameObject PlayerModel;
    public Vector3 SpawnPosition;

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
        SpawnPosition = new Vector3(Random.Range(-10, 10), 1.5f, Random.Range(-10, 10)); //Random spawn player "Random.Range(-10, 10)"
        transform.position = SpawnPosition;
    }

    public void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection);
        //Debug.Log("MovePos " + moveDirection.ToString());

        transform.position += moveDirection * Speed * Time.deltaTime;
        //Debug.Log("NewPos " + transform.position.ToString());

        if (transform.position.y < 0 || transform.position.y > 100)
        {
            transform.position = SpawnPosition;
        }
    }
}
