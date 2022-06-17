using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerEx : MonoBehaviour
{
    State _state;
    Move_State _move;
    Idle_State _idle;

    private bool isMove;
    private float Speed;

    Animator animator;

    void Start()
    {
        _state = State.None;
        _move = Move_State.None;
        _idle = Idle_State.None;
        isMove = false;
        Speed = 7f;
        transform.position = new Vector3(0.75f,0.25f,0);
        DirPos = transform.position;
        animator = gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        Move_State_Update();
        Ani_Update();
    }

    private void FixedUpdate()
    {
        
    }

    private void Move_State_Update()
    {
        Dist = Vector3.Distance(transform.position, DirPos);
        if (Dist <= 0.01f)
        {
            isMove = false;
            _state = State.Idle;
            if (Input.GetKey(KeyCode.W))
            {
                _idle = Idle_State.Up;
                _move = Move_State.Up;
                _state = State.Move;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _idle = Idle_State.Down;
                _state = State.Move;
                _move = Move_State.Down;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                _idle = Idle_State.Left;
                _state = State.Move;
                _move = Move_State.Left;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _idle = Idle_State.Right;
                _state = State.Move;
                _move = Move_State.Right;
            }
            else
            {
                _move = Move_State.None;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, DirPos, Speed * Time.deltaTime);

        

        if (isMove == false)
        {
            MoveToDir();
        }
    }

    private void Ani_Update()
    {
        if (_state == State.Move)
        {
            switch (_move)
            {
                case Move_State.Up:
                    animator.Play("run_up");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case Move_State.Down:
                    animator.Play("run_down");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case Move_State.Right:
                    animator.Play("run_side");
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                    break;
                case Move_State.Left:
                    animator.Play("run_side");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
            }
        }
        else if(_state == State.Idle)
        {
            switch (_idle)
            {
                case Idle_State.Up:
                    animator.Play("idle_up");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case Idle_State.Down:
                    animator.Play("idle_down");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case Idle_State.Right:
                    animator.Play("idle_side");
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                    break;
                case Idle_State.Left:
                    animator.Play("idle_side");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;

            }
        }
    }

    private Vector3 DirPos { get; set; }
    private float Dist;

    private void MoveToDir()
    {

        Vector3 DirPos_offset;
        switch (_move)
        {
            case Move_State.Up:
                isMove = true;
                DirPos_offset = new Vector3(0,1.5f,0);
                DirPos = transform.position + DirPos_offset;
                break;
            case Move_State.Down:
                isMove = true;
                DirPos_offset = new Vector3(0,-1.5f, 0);
                DirPos = transform.position + DirPos_offset;
                break;
            case Move_State.Right:
                isMove = true;
                DirPos_offset = new Vector3(1.5f,0,0);
                DirPos = transform.position + DirPos_offset;
                break;
            case Move_State.Left:
                isMove = true;
                DirPos_offset = new Vector3(-1.5f,0,0);
                DirPos = transform.position + DirPos_offset;
                break;
        };
    }
}
