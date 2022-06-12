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
    public GameObject weaponModel;
    GameObject mainCamera;
    GameObject localPlayer;
    public Rigidbody playerRigid;

    //Vector3
    public Vector3 spawnPosition;

    //Shoot
    public Transform shootOrigin;
    public GameObject projectilePrefab;
    public float ShootForce = 2000f;
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
                mainCamera.transform.localPosition = new Vector3 (0f,0.66f,0f); //Sets position of the object to Vector3

                weaponModel.transform.SetParent(mainCamera.transform);
                weaponModel.transform.localPosition = new Vector3(1.1f, -0.27f, 2.7f);
                weaponModel.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

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
        if (Input.GetMouseButtonDown(0) & health > 0 & itemAmount > 0)
        {
            itemAmount--;

            //int playerID = GameObject.Find("LocalGamePlayer").GetComponent<PlayerObjectController>().ConnectionID;
            int playerID = localPlayer.GetComponent<PlayerObjectController>().ConnectionID;
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            RaycastHit hit;
            Vector3 targetPoint;
            Vector3 directionWithoutSpread;
            //Vector3 directionWithSpread;
            //float x;
            //float y;

            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(75);
            }

            directionWithoutSpread = (targetPoint - shootOrigin.position).normalized;

            InstantiateProjectile(shootOrigin, directionWithoutSpread).InitializeProjectile(directionWithoutSpread, ShootForce, playerID);
        }
    }

    public Projectile InstantiateProjectile(Transform shootOrigin, Vector3 direction)
    {
        return Instantiate(projectilePrefab, shootOrigin.position, Quaternion.LookRotation(direction, Vector3.up)).GetComponent<Projectile>(); //Instantiate projectile and return Projectile
    }

    /*
    if (leftShot == true)
        {
            leftShot = false;
            directionWithoutSpread = (targetPoint - shootOrigin2.position).normalized;
            x = Random.Range(-melonSpread, melonSpread);
            y = Random.Range(-melonSpread, melonSpread);
            directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Does this work in normalized way

            NetworkManager.instance.InstantiateMelonProjectile(shootOrigin2, directionWithSpread).InitializeMelonProjectile(directionWithSpread, melonShootForce, id);
        }
        else
        {
            leftShot = true;
            directionWithoutSpread = (targetPoint - shootOrigin.position).normalized;
            x = Random.Range(-melonSpread, melonSpread);
            y = Random.Range(-melonSpread, melonSpread);
            directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Does this work in normalized way

            NetworkManager.instance.InstantiateMelonProjectile(shootOrigin, directionWithSpread).InitializeMelonProjectile(directionWithSpread, melonShootForce, id);
        }
    */

    public void Pause()
    {
        if (Input.GetKeyDown("escape"))
        {
            PauseToggled = PlayerUIController.Instance.TogglePauseUI(); //PauseToggled determines if you can walk and look around as a character
        }
    } //Pauses screen
}