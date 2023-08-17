using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speed;
    private Rigidbody _rigidbody;
    private float _targetX;
    public float smoothTime = 0.2f;
    private bool _hasObstacleLeft;
    private bool _hasObstacleRight;

    private void Start()
    {
        _speed = 3;
        _rigidbody = GetComponent<Rigidbody>();
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
        var newX = Mathf.Lerp(position.x, _targetX, smoothTime);
        position = new Vector3(newX, position.y, position.z);
        transform.position = position;
        transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(Vector3.up * 6, ForceMode.Impulse);
        }
        else if (Input.GetKeyDown(KeyCode.A) && transform.position.x >= -1 && !_hasObstacleLeft)
        {
            _targetX -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.D) && transform.position.x <= 1 && !_hasObstacleRight)
        {
            _targetX += 1;
        }
        Debug.DrawRay(transform.position, Vector3.left, Color.red);
        Debug.DrawRay(transform.position, Vector3.right, Color.green);
    }
}