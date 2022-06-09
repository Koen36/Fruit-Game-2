using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 12f;
    float ActualSpeed;
    public float JumpForce = 200f;
    public float mouseSensitivity = 150f;
    float xRotation = 0f;

    public GameObject PlayerModel;
    public GameObject Camera;
    public Rigidbody PlayerRigid;

    public Vector3 SpawnPosition;
    public Vector3 Velocity;


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

                GameObject.Find("Main Camera").SetActive(false);

                PlayerModel.SetActive(true);
                Debug.Log("Forest PlayerModel enabled");

                Cursor.lockState = CursorLockMode.Locked; //Lock cursor inside window
            }

            if (hasAuthority) //Only things that you have authority over and you want to control
            {
                Movement();
                Rotate();
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
        Run();

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        transform.position += (transform.right * x + transform.forward * z) * Time.deltaTime * ActualSpeed;

        Jump();

        if (transform.position.y < -10 || transform.position.y > 100)
        {
            transform.position = SpawnPosition;
        }
    } //Move player

    public void Run()
    {
        if (Input.GetKey("left shift"))
        {
            ActualSpeed = Speed * 1.5f;
        }
        else
        {
            ActualSpeed = Speed;
        }
    } //Lets player run

    public void Jump()
    {
        if (Input.GetKeyDown("space") & PlayerRigid.velocity.y == 0)
        {
            PlayerRigid.AddForce(transform.up * JumpForce);
        }
    } //Lets player jump

    public void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        Camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);

    } //Rotate player
}