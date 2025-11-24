using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBlinking : MonoBehaviour
{
    [SerializeField] private Image blink;
    
    private float randomTime = 0f;
    private float timePassed = 0f;

    private void Start()
    {
        this.randomTime = Random.Range(1f, 8f);    
    }

    private void Blink()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(this.blink.DOFade(1f, .1f).SetEase(Ease.OutBounce));
        sequence.Append(this.blink.DOFade(0f, .5f).SetEase(Ease.OutCirc));
    }
    
    private void Update()
    {
        if (this.timePassed >= this.randomTime)
        {
            Blink();
            this.randomTime = Random.Range(1f, 8f);
            this.timePassed = 0f;
        }
        
        this.timePassed += Time.deltaTime;
    }
}
