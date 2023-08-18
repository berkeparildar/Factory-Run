using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Animator _modelAnimator;
    private float _speed;
    private float _targetX;
    private readonly float _smoothTime = 0.03f;
    private bool _hasObstacleLeft;
    private bool _hasObstacleRight;
    private static readonly int TurnRight = Animator.StringToHash("turnRight");
    private static readonly int TurnLeft = Animator.StringToHash("turnLeft");

    private void Start()
    {
        _speed = 3;
        _rigidbody = GetComponent<Rigidbody>();
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
        }
        else if (Input.GetKeyDown(KeyCode.A) && transform.position.x > -0.99f && !_hasObstacleLeft)
        {
            _targetX -= 1;
            _modelAnimator.SetTrigger(TurnLeft);
        }
        else if (Input.GetKeyDown(KeyCode.D) && transform.position.x < 0.99f && !_hasObstacleRight)
        {
            _targetX += 1;
            _modelAnimator.SetTrigger(TurnRight);
        }
        Debug.DrawRay(transform.position, Vector3.left, Color.red);
        Debug.DrawRay(transform.position, Vector3.right, Color.green);
    }
}