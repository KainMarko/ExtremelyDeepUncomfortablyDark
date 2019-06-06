using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Movement VAR's
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Rigidbody rigid;
    public Vector3 Direction;

    Animator mAnim;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Animation();
        Movement();
    }

    void Animation()
    {
        mAnim = GameObject.FindWithTag("Player").GetComponent<Animator>();

        mAnim.SetFloat("Speed", Math.Max(Math.Abs(Input.GetAxis("Horizontal")), Math.Abs(Input.GetAxis("Vertical"))));
    }

    void Movement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 jump = new Vector3(0, jumpForce, 0);
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        if (Input.GetButtonDown("Jump"))
        {
            rigid.AddForce(jump, ForceMode.Impulse);
        }

        if (moveHorizontal + moveVertical != 0)
        {
            rigid.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.2F);
        }

        Vector3 nextpos = transform.position;

        nextpos += movement * moveSpeed * Time.deltaTime;

        rigid.MovePosition(nextpos);
    }
}

