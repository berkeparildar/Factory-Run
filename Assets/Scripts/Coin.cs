using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioSource coinSound;
    [SerializeField] private GameObject player;
    void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), 1).SetRelative().SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        coinSound = GameObject.Find("CoinSound").GetComponent<AudioSource>();
        StartCoroutine(DestroyCoin());
    }

    private void Update()
    {
        DestroyCoin();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Coins++;
            coinSound.Play();
            DOTween.Kill(this.transform);
            DOTween.instance.DOKill();
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyCoin()
    {
        yield return new WaitForSeconds(30);
            DOTween.Kill(this.transform);
            DOTween.instance.DOKill();
            Destroy(gameObject);
    }
}
