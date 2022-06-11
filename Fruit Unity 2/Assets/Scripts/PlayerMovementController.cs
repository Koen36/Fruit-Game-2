using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    //Physics variables
    public float speed = 12f;
    public float jumpForce = 200f;
    public float mouseSensitivity = 150f;
    public float clampAngle = 85f;
    float actualSpeed;
    float xRotation = 0f;

    //GameObjects
    public GameObject playerModel;
    GameObject mainCamera;
    GameObject localPlayer;
    public Rigidbody playerRigid;

    //Vector3
    public Vector3 spawnPosition;

    //Shoot
    public Transform shootOrigin;
    public GameObject projectile;
    public int health = 100;
    public int itemAmount = 50;


    //Pause
    bool PauseToggled = false;


    public void Start()
    {
        playerModel.SetActive(false);
    }

    public void Update()
    {
        if(SceneManager.GetActiveScene().name == "Forest")
        {
            if(!playerModel.activeSelf)
            {
                SetPosition();
                Debug.Log("Player spawned");

                mainCamera = GameObject.Find("Main Camera");
                localPlayer = GameObject.Find("LocalGamePlayer");

                mainCamera.transform.SetParent(localPlayer.transform); //Sets object inside the content
                mainCamera.transform.localPosition = new Vector3 (0f,0.6f,0f); //Sets position of the object to Vector3

                playerModel.SetActive(true);
                Debug.Log("Forest PlayerModel enabled");

                PlayerUIController.Instance.EnableCursorLock();
                PlayerUIController.Instance.LoadLevelUI(false);
            }

            if (hasAuthority) //Only things that you have authority over and you want to control
            {
                if (!PauseToggled)
                {
                    Movement();
                    Rotate();
                    Shoot();
                }
                Pause();
            }
        }
    }

    public void SetPosition() //Spawn player
    {
        spawnPosition = new Vector3(Random.Range(-10, 10), 1.5f, Random.Range(-10, 10)); //Random spawn player "Random.Range(-10, 10)"
        transform.position = spawnPosition;
    }
    
    public void Movement()
    {
        Run();

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        transform.position += (transform.right * x + transform.forward * z) * Time.deltaTime * actualSpeed;

        Jump();

        if (transform.position.y < -10 || transform.position.y > 100)
        {
            transform.position = spawnPosition;
        }
    } //Move player

    public void Run()
    {
        if (Input.GetKey("left shift"))
        {
            actualSpeed = speed * 1.5f;
        }
        else
        {
            actualSpeed = speed;
        }
    } //Lets player run

    public void Jump()
    {
        if (Input.GetKeyDown("space") & playerRigid.velocity.y == 0f)
        {
            playerRigid.AddForce(transform.up * jumpForce);
        }
    } //Lets player jump

    public void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);

    } //Rotate player

    public void Shoot() //Shoot projectiles
    {
        if (Input.GetMouseButton(0) & health > 0 & itemAmount > 0)
        {
            itemAmount--;

            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            RaycastHit hit;
            Vector3 targetPoint;
            Vector3 directionWithoutSpread;
            //Vector3 directionWithSpread;
            //float x;
            float y;

            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(75);
            }
            directionWithoutSpread = (targetPoint - shootOrigin.position).normalized;

            //NetworkManager.instance.InstantiateBananaProjectile(shootOrigin, directionWithoutSpread).InitializeBananaProjectile(directionWithoutSpread, bananaShootForce, id);

            //SHOOT ORIGIN ASSIGN
        }
    }

    public void Pause()
    {
        if (Input.GetKeyDown("escape"))
        {
            PauseToggled = PlayerUIController.Instance.TogglePauseUI(); //PauseToggled determines if you can walk and look around as a character
        }
    } //Pauses screen
}