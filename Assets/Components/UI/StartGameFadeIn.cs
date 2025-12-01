using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartGameFadeIn : MonoBehaviour
{
    [SerializeField] private Image background;

    private IEnumerator Init()
    {
        MixerController.controller.mixer.SetFloat("MasterVolume", -80f);
        MixerController.controller.mixer.DOSetFloat("MasterVolume", 0f, 3f);
        this.background.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        this.background.DOFade(0f, 2f).onComplete += () => this.background.gameObject.SetActive(false);
    }
    
    private void Start() => StartCoroutine(Init());
}
