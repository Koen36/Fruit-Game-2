using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Diagnostics;


public class CharacterMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform GroundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    public GameObject PauseMenu;

    void Update()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump")) 
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        if(Input.GetButtonDown("Pause")) 
        {
            PauseMenu.SetActive(true);
            float fallSpeed = velocity.y;
            velocity.y = 0f;
            float speedPause = speed;
            speed = 0f;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int Elapsed = Convert.ToInt32(stopWatch.Elapsed);

            if(Input.GetButtonDown("Pause") && PauseMenu.activeSelf && Elapsed > 1000) {
                stopWatch.Stop();
                PauseMenu.SetActive(false);
                velocity.y = fallSpeed;
                speed = speedPause;
            }
        }

        if(Input.GetButtonDown("Fire1")) 
        {
            //Debug.Log("Shoot banana");
        }
    }
}
