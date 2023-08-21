using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Animator _modelAnimator;
    private BoxCollider _boxCollider;
    private float _speed;
    private bool _canTurnAgain;
    private bool _hasObstacleLeft;
    private bool _hasObstacleRight;
    private bool _isOnFloor;
    private bool _isDucking;
    private static readonly int TurnRight = Animator.StringToHash("turnRight");
    private static readonly int TurnLeft = Animator.StringToHash("turnLeft");
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int Jumping = Animator.StringToHash("jumping");
    private static readonly int Duck1 = Animator.StringToHash("duck");

    private void Start()
    {
        _speed = 3;
        _canTurnAgain = true;
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        _modelAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        var position = transform.position;
        if (!_isDucking)
        {
            _isOnFloor = !Physics.Raycast(position, Vector3.down, 0.2f);
            _modelAnimator.SetBool(Jumping, _isOnFloor);
        }
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
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        Debug.Log(_isOnFloor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            _speed = 0;
        }
    }

    private void Movement()
    {
        transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(Vector3.up * 6, ForceMode.Impulse);
        }
        else if (Input.GetKeyDown(KeyCode.A) && !_hasObstacleLeft && _canTurnAgain && !_isOnFloor)
        {
            _canTurnAgain = false;
            transform.DOMoveX(-1, 0.6f).SetRelative().OnComplete(() => { _canTurnAgain = true; });
            ;
            _modelAnimator.SetTrigger(TurnLeft);
        }
        else if (Input.GetKeyDown(KeyCode.D) && !_hasObstacleRight && _canTurnAgain && !_isOnFloor)
        {
            _canTurnAgain = false;
            transform.DOMoveX(1, 0.6f).SetRelative().OnComplete(() => { _canTurnAgain = true; });
            _modelAnimator.SetTrigger(TurnRight);
        }
        else if (Input.GetKeyDown(KeyCode.S) && !_isOnFloor && _canTurnAgain)
        {
            StartCoroutine(Duck());
        }
    }

    private IEnumerator Duck()
    {
        //_modelAnimator.SetTrigger(Duck1);
        _isDucking = true;
        var currentSize = _boxCollider.size;
        var currentCenter = _boxCollider.center;
        _boxCollider.size = new Vector3(currentSize.x, 0.5f, currentSize.z);
        _boxCollider.center = new Vector3(currentCenter.x, 0.25f, currentCenter.z);
        yield return new WaitForSeconds(1.16f);
        _boxCollider.size = currentSize;
        _boxCollider.center = currentCenter;
        _isDucking = false;
        _isOnFloor = !Physics.Raycast(transform.position, Vector3.down, 0.2f);
    }
}