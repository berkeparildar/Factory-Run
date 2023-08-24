using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Animator _modelAnimator;
    private GameManager _gameManager;
    private float _speed;
    private bool _canTurnAgain;
    private bool _hasObstacleLeft;
    private bool _hasObstacleRight;
    private bool _isOnFloor;
    public static bool IsAlive;
    private static readonly int Right = Animator.StringToHash("turnRight");
    private static readonly int Left = Animator.StringToHash("turnLeft");
    private static readonly int Jumping = Animator.StringToHash("jumping");
    private static readonly int Duck1 = Animator.StringToHash("duck");
    private static readonly int Die = Animator.StringToHash("die");

    private Vector2 _touchStartPos;
    private float _minSwipeDistance;
    private const float MinSwipeDistancePercent = 0.05f; // Adjust this value as needed

    private void Start()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        _minSwipeDistance = Mathf.Max(screenWidth, screenHeight) * MinSwipeDistancePercent;
        IsAlive = true;
        _speed = 4.5f;
        _canTurnAgain = true;
        _rigidbody = GetComponent<Rigidbody>();
        for (int i = 0; i < 8; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        Debug.Log(PlayerPrefs.GetInt("Model"));
        transform.GetChild(PlayerPrefs.GetInt("Model", 0)).gameObject.SetActive(true);
        _modelAnimator = transform.GetChild(PlayerPrefs.GetInt("Model", 0)).GetComponent<Animator>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            IsAlive = false;
            _modelAnimator.SetTrigger(Die);
            _gameManager.ShowHitUI();
        }
    }
    
    private void Movement()
    {
        transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchStartPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    Vector2 swipeDelta = touch.position - _touchStartPos;
                    if (swipeDelta.magnitude > _minSwipeDistance)
                    {
                        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                        {
                            if (swipeDelta.x > 0 && !_hasObstacleLeft && _canTurnAgain && !_isOnFloor)
                            {
                                TurnRight();
                            }
                            else if (swipeDelta.x < 0 && !_hasObstacleRight && _canTurnAgain && !_isOnFloor)
                            {
                                TurnLeft();
                            }
                        }
                        else
                        {
                            if (swipeDelta.y > 0 && !_isOnFloor)
                            {
                                _rigidbody.AddForce(Vector3.up * 5.5f, ForceMode.Impulse);
                            }
                            else if (swipeDelta.y < 0 && !_isOnFloor && _canTurnAgain)
                            {
                                StartCoroutine(Duck());
                            }
                        }
                    }

                    break;
            }
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