using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayMenu : MonoBehaviour
{
    [SerializeField] private Image bg;
    
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage image;
    
    [SerializeField] private AudioSource audio;
    
    public IEnumerator StartSeq()
    {
        yield return new WaitForSeconds(2f);
        this.bg.DOFade(0f, 1f).onComplete = () => this.bg.gameObject.SetActive(false);
    }

    public IEnumerator OnPlay()
    {
        this.audio.DOFade(0f, 1f);
        this.bg.gameObject.SetActive(true);
        this.bg.DOFade(1f, 1f);
        yield return new WaitForSeconds(3f);
        
        this.videoPlayer.gameObject.SetActive(true);
        this.image.gameObject.SetActive(true);
        
        this.videoPlayer.Play();
        this.videoPlayer.loopPointReached += videoPlayer =>
        {
            SceneManager.LoadScene(1);
        };
    }
    
    public void Start()
    {
        StartCoroutine(StartSeq());
    }
    
    public void PlayMode()
    {
        Debug.Log("Boom");
        StartCoroutine(OnPlay());
    }
}
