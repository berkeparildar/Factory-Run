using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float speed;
    [SerializeField] private bool canTurnAgain;
    [SerializeField] private bool hasObstacleLeft;
    [SerializeField] private bool hasObstacleRight;
    [SerializeField] private bool isOnFloor;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource moveSource;
    [SerializeField] private AudioClip death;
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
        speed = 4.5f;
        canTurnAgain = true;
        rigidBody = GetComponent<Rigidbody>();
        for (int i = 0; i < 8; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        StartCoroutine(GameManager.ScoreRoutine());
        Debug.Log(PlayerPrefs.GetInt("Model"));
        transform.GetChild(PlayerPrefs.GetInt("Model", 0)).gameObject.SetActive(true);
        modelAnimator = transform.GetChild(PlayerPrefs.GetInt("Model", 0)).GetComponent<Animator>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void FixedUpdate()
    {
        var position = transform.position;
        isOnFloor = !Physics.Raycast(position, Vector3.down, 0.2f);
        if (isOnFloor)
        {
            audioSource.Stop();
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        modelAnimator.SetBool(Jumping, isOnFloor);
        var origin = new Vector3(position.x, position.y + 1, position.z);

        if (Physics.Raycast(origin, Vector3.right, 1))
        {
            hasObstacleRight = true;
        }
        else
        {
            hasObstacleRight = false;
        }

        if (Physics.Raycast(origin, Vector3.left, 1))
        {
            hasObstacleLeft = true;
        }
        else
        {
            hasObstacleLeft = false;
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
            speed = 0;
            audioSource.enabled = false;
            moveSource.clip = death;
            moveSource.Play();
            IsAlive = false;
            modelAnimator.SetTrigger(Die);
            gameManager.ShowHitUI();
        }
    }
    
    private void Movement()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
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
                            if (swipeDelta.x > 0 && !hasObstacleLeft && canTurnAgain && !isOnFloor)
                            {
                                TurnRight();
                            }
                            else if (swipeDelta.x < 0 && !hasObstacleRight && canTurnAgain && !isOnFloor)
                            {
                                TurnLeft();
                            }
                        }
                        else
                        {
                            if (swipeDelta.y > 0 && !isOnFloor)
                            {
                                moveSource.Play();
                                rigidBody.AddForce(Vector3.up * 5.5f, ForceMode.Impulse);
                            }
                            else if (swipeDelta.y < 0 && !isOnFloor && canTurnAgain)
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
        moveSource.Play();
        audioSource.Stop();
        canTurnAgain = false;
        transform.DOMoveX(-1, 0.6f).SetRelative().OnComplete(() =>
        {
            canTurnAgain = true;
            audioSource.Play();
        });
        modelAnimator.SetTrigger(Left);
    }

    private void TurnRight()
    {
        moveSource.Play();
        canTurnAgain = false;
        audioSource.Stop();
        transform.DOMoveX(1, 0.6f).SetRelative().OnComplete(() =>
        {
            canTurnAgain = true;
            audioSource.Play();
        });
        modelAnimator.SetTrigger(Right);
    }

    private IEnumerator Duck()
    {
        moveSource.Play();
        modelAnimator.SetTrigger(Duck1);
        audioSource.Stop();
        var tunnels = GameObject.FindGameObjectsWithTag("Tunnel");
        foreach (var tunnel in tunnels)
        {
            tunnel.GetComponent<BoxCollider>().enabled = false;
        }

        yield return new WaitForSeconds(1.16f);
        audioSource.Play();
        foreach (var tunnel in tunnels)
        {
            tunnel.GetComponent<BoxCollider>().enabled = true;
        }
    }
}