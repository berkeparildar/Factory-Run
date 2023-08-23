using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private GameObject _player;
    void Start()
    {
        _player = GameObject.Find("Player");
        transform.DORotate(new Vector3(0, 360, 0), 1).SetRelative().SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    private void Update()
    {
        DestroyCoin();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player)
        {
            GameManager.Coins++;
            DOTween.Kill(this.transform);
            DOTween.instance.DOKill();
            Destroy(gameObject);
        }
    }

    private void DestroyCoin()
    {
        if (transform.position.z + 5 <= _player.transform.position.z)
        {
            DOTween.Kill(this.transform);
            DOTween.instance.DOKill();
            Destroy(gameObject);
        }
    }
}
