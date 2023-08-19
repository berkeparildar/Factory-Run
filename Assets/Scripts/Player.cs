using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Animator _modelAnimator;
    private BoxCollider _boxCollider;
    private float _speed;
    private float _targetX;
    private readonly float _smoothTime = 0.03f;
    private bool _hasObstacleLeft;
    private bool _hasObstacleRight;
    private bool _isOnFloor;
    private static readonly int TurnRight = Animator.StringToHash("turnRight");
    private static readonly int TurnLeft = Animator.StringToHash("turnLeft");
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int Jumping = Animator.StringToHash("jumping");
    private static readonly int Duck1 = Animator.StringToHash("duck");

    private void Start()
    {
        _speed = 3;
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        _modelAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.right, 1))
        {
            _hasObstacleRight = true;
        }
        else
        {
            _hasObstacleRight = false;
        }
        
        if (Physics.Raycast(transform.position, Vector3.left, 1))
        {
            _hasObstacleLeft = true;
        }
        else
        {
            _hasObstacleLeft = false;
        }
        
        /*if (Physics.Raycast(transform.position, Vector3.down, out var ray, 0.05f))
        {
            if (ray.transform.CompareTag("Platform"))
            {
                Debug.Log("true");
                _modelAnimator.SetTrigger(IsGrounded);
            }
        }*/
        _modelAnimator.SetBool(Jumping,!Physics.Raycast(transform.position, Vector3.down, 0.2f));
    }

    private void Update()
    {
        var position = transform.position;
        var newX = Mathf.Lerp(position.x, _targetX, _smoothTime);
        position = new Vector3(newX, position.y, position.z);
        transform.position = position;
        transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(Vector3.up * 6, ForceMode.Impulse);
            _modelAnimator.SetTrigger(Jump);
        }
        else if (Input.GetKeyDown(KeyCode.A) && transform.position.x > -0.99f)
        {
            _targetX -= 1;
            _modelAnimator.SetTrigger(TurnLeft);
        }
        else if (Input.GetKeyDown(KeyCode.D) && transform.position.x < 0.99f)
        {
            _targetX += 1;
            _modelAnimator.SetTrigger(TurnRight);
        }
        Debug.DrawRay(transform.position, Vector3.down * 0.05f, Color.red);
        Debug.DrawRay(transform.position, Vector3.right, Color.green);
        //Duck();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            _speed = 0;
        }
    }

    private void Duck()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            _modelAnimator.SetTrigger(Duck1);
            //_boxCollider.size = new Vector3(0.5f, 0.9f, 0.5f);
        }
    }
}