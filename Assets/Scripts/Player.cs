using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Animator _modelAnimator;
    private float _speed;
    private bool _canTurnAgain;
    private bool _hasObstacleLeft;
    private bool _hasObstacleRight;
    private bool _isOnFloor;
    private static readonly int Right = Animator.StringToHash("turnRight");
    private static readonly int Left = Animator.StringToHash("turnLeft");
    private static readonly int Jumping = Animator.StringToHash("jumping");
    private static readonly int Duck1 = Animator.StringToHash("duck");
    private static readonly int Die = Animator.StringToHash("die");

    private void Start()
    {
        _speed = 4.5f;
        _canTurnAgain = true;
        _rigidbody = GetComponent<Rigidbody>();
        _modelAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        var position = transform.position;
        _isOnFloor = !Physics.Raycast(position, Vector3.down, 0.2f);
        _modelAnimator.SetBool(Jumping, _isOnFloor);

        var origin = new Vector3(position.x, position.y + 1, position.z);
        
        if (Physics.Raycast(origin, Vector3.right, 1))
        {
            _hasObstacleRight = true;
        }
        else
        {
            _hasObstacleRight = false;
        }

        if (Physics.Raycast(origin, Vector3.left, 1))
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
        Movement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Tunnel"))
        {
            _speed = 0;
            _modelAnimator.SetTrigger(Die);
        }
    }

    private void Movement()
    {
        transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
        if (Input.GetKeyDown(KeyCode.Space) && !_isOnFloor)
        {
            _rigidbody.AddForce(Vector3.up * 5.5f, ForceMode.Impulse);
        }
        else if (Input.GetKeyDown(KeyCode.A) && !_hasObstacleLeft && _canTurnAgain && !_isOnFloor)
        {
           TurnLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D) && !_hasObstacleRight && _canTurnAgain && !_isOnFloor)
        {
            TurnRight();
        }
        else if (Input.GetKeyDown(KeyCode.S) && !_isOnFloor && _canTurnAgain)
        {
            StartCoroutine(Duck());
        }
    }

    private void TurnLeft()
    {
        _canTurnAgain = false;
        transform.DOMoveX(-1, 0.6f).SetRelative().OnComplete(() => { _canTurnAgain = true; });
        _modelAnimator.SetTrigger(Left);
    }

    private void TurnRight()
    {
        _canTurnAgain = false;
        transform.DOMoveX(1, 0.6f).SetRelative().OnComplete(() => { _canTurnAgain = true; });
        _modelAnimator.SetTrigger(Right);
    }

    private IEnumerator Duck()
    {
        _modelAnimator.SetTrigger(Duck1);
        var tunnels = GameObject.FindGameObjectsWithTag("Tunnel");
        foreach (var tunnel in tunnels)
        {
            tunnel.GetComponent<BoxCollider>().enabled = false;
        }
        yield return new WaitForSeconds(1.16f);
        foreach (var tunnel in tunnels)
        {
            tunnel.GetComponent<BoxCollider>().enabled = true;
        }

    }
}