using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthRender : MonoBehaviour
{
    [SerializeField] private Entity playerEntity;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image gradient;

    [SerializeField] private Image hurtGradient;
    [SerializeField] private Color[] colors;
    private void UpdateHealth(float health)
    {
        this.text.text = Mathf.Round(this.playerEntity._health).ToString();
        
        this.text.color = Color.Lerp(this.colors[0], Color.red, 1f - (this.playerEntity._health /
                                                                      this.playerEntity._maxHealth));
        this.gradient.color = Color.Lerp(this.colors[1], Color.red, 1f - (this.playerEntity._health /
                                                                          this.playerEntity._maxHealth));
    }
    
    private void Start()
    {
        this.playerEntity.HealthChanged.AddListener(UpdateHealth);
        UpdateHealth(this.playerEntity._health);

        this.colors = new Color[] { this.text.color, this.gradient.color };

        float lastHealth = this.playerEntity._health;
        this.playerEntity.HealthChanged.AddListener(health =>
        {
            if (lastHealth > this.playerEntity._health)
            {
                Debug.Log("boomColor");
                Color color = this.hurtGradient.color;
                color.a = 1f;
                
                this.hurtGradient.color = color;
                this.hurtGradient.DOFade(0f, .35f).SetEase(Ease.OutQuad);
            }
            
            lastHealth = health;
        });
    }
}
