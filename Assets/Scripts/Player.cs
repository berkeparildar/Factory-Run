using UnityEngine;

public class Player : MonoBehaviour
{
    private float _speed;
    private Rigidbody _rigidbody;
    private float _targetX;
    public float smoothTime = 0.2f;

    private void Start()
    {
        _speed = 3;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var position = transform.position;
        float newX = Mathf.Lerp(position.x, _targetX, smoothTime);
        position = new Vector3(newX, position.y, position.z);
        transform.position = position;
        transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(Vector3.up * 6, ForceMode.Impulse);
        }
        else if (Input.GetKeyDown(KeyCode.A) && transform.position.x >= -1)
        {
            _targetX -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.D) && transform.position.x <= 1)
        {
            _targetX += 1;
        }
    }
}