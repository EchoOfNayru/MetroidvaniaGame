using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    float gravSpeed = 0f;
    float gravAcc = .02f;
    float gravCap = -0.6f;
    float jumpStrength = 0.3f;
    float lrMoveSpeed = 0.2f;
    float lrMovement = 0f;
    float lrMovementMax = 0.3f;
    float lrMovementMin = -0.3f;
    float wallJumpBoost = 0;
    float wallJumpPower = 0.8f;
    float wallJumpBoostDecay = 0.80f;
    float wallSlideSpeed = -0.03f;

    RaycastHit hit;

    public bool isGrounded;
    public bool isWallSlideLeft;
    public bool isWallSlideRight;
    public bool hasJumped;

    private void Start()
    {
        isGrounded = false;
        hasJumped = false;
    }

    private void Update()
    {
        //gravity
        gravSpeed -= gravAcc;
        if (gravSpeed <= gravCap)
        {
            gravSpeed = gravCap;
        }
        if (isGrounded)
        {
            gravSpeed = 0;
        }

        //ground check
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.51f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        //wall slide
        if (Physics.Raycast(transform.position, Vector3.left, out hit, 0.55f))
        {
            isWallSlideLeft = true;
            isWallSlideRight = false;
        }
        else if (Physics.Raycast(transform.position, Vector3.right, out hit, 0.55f))
        {
            isWallSlideRight = true;
            isWallSlideLeft = false;
        }
        else
        {
            if (Mathf.Abs(wallJumpBoost) < (wallJumpPower / 2))
            {
                isWallSlideLeft = false;
                isWallSlideRight = false;
            }
            isWallSlideLeft = false;
            isWallSlideRight = false;
        }

        //inputs
            //jump
            if (Input.GetAxis("Jump") == 1 && isGrounded && !hasJumped)
            {
                gravSpeed = jumpStrength;
                isGrounded = false;
                hasJumped = true;
            }
            //walljump
            if (Input.GetAxis("Jump") == 1 && isWallSlideLeft && !hasJumped || Input.GetAxis("Jump") == 1 && isWallSlideRight && !hasJumped)
            {
                gravSpeed = jumpStrength * .8f;
                if (isWallSlideLeft)
                {
                    wallJumpBoost = wallJumpPower;
                }
                else
                {
                    wallJumpBoost = -wallJumpPower;
                }
            hasJumped = true;
            }
            //jumpstop
            if (Input.GetAxis("Jump") == 0)
            {
                if (isGrounded || isWallSlideLeft || isWallSlideRight)
                {
                    hasJumped = false;
                }
            }
            //left and right
            if (Input.GetAxis("Horizontal") > 0)
                {
                    lrMovement = lrMoveSpeed * Input.GetAxis("Horizontal");
                }
            if (Input.GetAxis("Horizontal") < 0)
            {
                lrMovement = lrMoveSpeed * Input.GetAxis("Horizontal");
            }
            else if (Input.GetAxis("Horizontal") == 0)
            {
                lrMovement /= 9;
            }

        //movement
        float totalMovement = lrMovement + wallJumpBoost;
        if (totalMovement > lrMovementMax)
        {
            totalMovement = lrMovementMax;
        }
        if (totalMovement < lrMovementMin - wallJumpBoost)
        {
            totalMovement = lrMovementMin;
        }
        
        if (isWallSlideLeft || isWallSlideRight)
        {
            if (gravSpeed < wallSlideSpeed)
            {
                gravSpeed = wallSlideSpeed;
            }
            if (isWallSlideLeft)
            {
                if (lrMovement < 0)
                {
                    totalMovement -= lrMovement;
                }
            }
            if (isWallSlideRight)
            {
                if (lrMovement > 0)
                {
                    totalMovement -= lrMovement;
                }
            }
        }
        transform.position += new Vector3(totalMovement, gravSpeed, 0);
        wallJumpBoost *= wallJumpBoostDecay;
    }
}
