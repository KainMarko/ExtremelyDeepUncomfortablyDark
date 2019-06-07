using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Movement VAR's
    public float moveSpeed = 25f;
    public float jumpForce = 1f;
    public float moveFriction = 7f;
    public float rollSpeed = 7f;
    public float rollDelay = 0.1f;

    //Bools to check states
    public bool isRoll = false;
    public bool isBackstep = false;
    public bool canMove = true;
    public bool isBlocking = false;

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
        Animation();
        IsGrounded();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }

    void Animation()
    {
        mAnim.SetFloat("Speed", Math.Max(Math.Abs(Input.GetAxis("Horizontal")), Math.Abs(Input.GetAxis("Vertical"))));
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
        isRoll = true;
        canMove = false;
        mAnim.SetBool("Isrolling", true);

        mRigid.AddForce(transform.forward * rollSpeed);
        yield return new WaitForSeconds(rollDelay);

        canMove = true;
        isRoll = false;
    }

    void Backstep()
    {
        isBackstep = true;
        mAnim.SetBool("Isbackstep", true);
        mRigid.AddForce(-transform.forward * 50f, ForceMode.VelocityChange);
    }

    void Block()
    {
        canMove = false;
        mRigid.velocity = new Vector3(0, tempVel.y, 0);
        isBlocking = true;
        mAnim.SetBool("Isblocking", true);
    }

    void Movement()
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 playerMove = forward * moveVertical + right * moveHorizontal;

        if (canMove)
        {
            tempVel.x = playerMove.x * moveSpeed;
            tempVel.z = playerMove.z * moveSpeed;
            tempVel.y = 0.0f;
        }

        if ((moveHorizontal + moveVertical != 0) && !isRoll)
        {
            mRigid.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(playerMove.x, 0, playerMove.z)), 0.2F);
        }

        if (Input.GetButtonDown("L1") && canMove && IsGrounded())
        {
            Block();
        }

        if (Input.GetButtonUp("L1"))
        {
            canMove = true;
            isBlocking = false;
            mAnim.SetBool("Isblocking", false);
        }

        if (Input.GetButtonDown("Triangle"))
        {
            Jump();
        }

        if (Input.GetButtonDown("Circle") && !isRoll && canMove && !isBackstep)
        {
            if ((Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")) > 0))
            {
                StartCoroutine(Roll());
            }
            else
            {
                Backstep();
            }
        }
        mRigid.velocity += tempVel;

        mRigid.velocity = Vector3.Scale(new Vector3(mRigid.velocity.x, mRigid.velocity.y, mRigid.velocity.z), new Vector3(moveFriction, 1.0f, moveFriction));
    }
}

