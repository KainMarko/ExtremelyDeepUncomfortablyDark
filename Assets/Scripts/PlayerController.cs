using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Movement VAR's
    public float moveSpeed = 3f;
    public float jumpForce = 1f;
    public float moveFriction = 7f;
    public float rollSpeed = 7f;
    public float rollDelay = 0.1f;

    //Bools to check states
    public bool isBackstep = false;
    public bool canMove = true;
    public bool isBlocking = false;
    public bool isJumping = false;

    //Setting the player state;
    public enum PlayerState
    {
        NORMAL,
        ROLLING,
        BLOCKING,
        JUMPING,
        BACKSTEPPING,
        STATE_MOVING
    }
    public PlayerState myPlayerstate = PlayerState.NORMAL;

    //Calling components
    private Rigidbody mRigid;
    private Animator mAnim;
    private Collider mColl;

    //Calling Vectors
    public Vector3 Direction;
    public Vector3 tempVel;

    // Start is called before the first frame update
    void Start()
    {
        mRigid = GetComponent<Rigidbody>();
        mAnim = GetComponent<Animator>();
        mColl = GetComponent<Collider>();
    }

    void Update()
    {
        HandleInput();
        Animation();
        IsGrounded();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }

    private void SetState(PlayerState newState)
    {
        myPlayerstate = newState;
        switch (myPlayerstate)
        {
            case PlayerState.NORMAL:
                stopBlock();
                break;

            case PlayerState.ROLLING:

                break;

            case PlayerState.BLOCKING:
                Block();
                break;

            case PlayerState.JUMPING:

                break;

            case PlayerState.BACKSTEPPING:
                Backstep();
                break;
        }
    }

    void HandleInput()
    {
        if (Input.GetButtonDown("L1"))
        {
            SetState(PlayerState.BLOCKING);
        }
        if (Input.GetButtonUp("L1"))
        {
            SetState(PlayerState.NORMAL);
        }
        if (Input.GetButtonDown("Triangle"))
        {
            SetState(PlayerState.JUMPING);
        }
        if (Input.GetButtonDown("Circle") && tempVel.x == 0 && tempVel.z == 0)
        {
            SetState(PlayerState.BACKSTEPPING);
        }
    }

    void Animation()
    {
        mAnim.SetFloat("Speed", (Math.Abs(Input.GetAxis("Horizontal")) + Math.Abs(Input.GetAxis("Vertical"))));
    }

    bool IsGrounded()  //Method to check if there is a collider below
    {
        return Physics.Raycast(transform.position, Vector3.down, mColl.bounds.extents.y + 0.5f);
    }

    void FootL()
    {
        //TODO: Add footstep sounds
    }

    void FootR()
    {
        //TODO: Add footstep sounds
    }

    void Endbackstep()
    {
        canMove = true;
        myPlayerstate = PlayerState.NORMAL;
        isBackstep = false;
        mAnim.SetBool("Isbackstep", false);
    }

    void Jump()
    {
        if (IsGrounded())
        {
            Vector3 jump = new Vector3(0, jumpForce, 0);

            mRigid.AddForce(jump, ForceMode.Impulse);
        }
    }

    IEnumerator Roll()
    {
        myPlayerstate = PlayerState.ROLLING;
        canMove = false;
        mAnim.SetBool("Isrolling", true);

        mRigid.AddForce(transform.forward * rollSpeed);
        yield return new WaitForSeconds(rollDelay);

        canMove = true;
        myPlayerstate = PlayerState.NORMAL;
    }

    void Backstep()
    {
        isBackstep = true;
        canMove = false;
        mAnim.SetBool("Isbackstep", true);
        mRigid.AddForce(-transform.forward * 25f, ForceMode.Impulse);
    }

    void Block()
    {
        isBlocking = true;
        canMove = false;
        mAnim.SetBool("Isblocking", true);
    }

    void stopBlock()
    {
        isBlocking = false;
        canMove = true;
        mAnim.SetBool("Isblocking", false);
    }

    void Movement()
    {
        //Getting Camera forward + right for relative controls
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 playerMove = forward * moveVertical + right * moveHorizontal;

        //If the player can move, do so
        if (canMove)
        {
            tempVel.x = playerMove.x * moveSpeed;
            tempVel.z = playerMove.z * moveSpeed;
            tempVel.y = 0.0f;

            mRigid.velocity = Vector3.Scale(new Vector3(mRigid.velocity.x, mRigid.velocity.y, mRigid.velocity.z), new Vector3(moveFriction, 1.0f, moveFriction));
        }
        else
            tempVel.x = 0;
            tempVel.z = 0;

        mRigid.velocity += tempVel;

        if ((moveHorizontal + moveVertical != 0) && (myPlayerstate != PlayerState.ROLLING) && (myPlayerstate != PlayerState.BACKSTEPPING))
        {
            mRigid.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(playerMove.x, 0, playerMove.z)), 0.2F);
        }
    }
}

