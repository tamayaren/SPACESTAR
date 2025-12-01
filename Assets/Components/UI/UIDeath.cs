using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIDeath : MonoBehaviour
{
    [SerializeField] private Entity playerEntity;
    [SerializeField] private GameObject parent;

    [SerializeField] private Image eye;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private GameObject retry;

    public Vector3 eyeCenter;
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator Kill()
    {
        
        MixerController.controller.mixer.SetFloat("MasterVolume", -80f);
        this.parent.SetActive(true);

        yield return new WaitForSeconds(2f);
        
        this.eye.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        this.text.gameObject.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        
        yield return new WaitForSeconds(3f);
        this.retry.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (this.eye.gameObject.activeSelf)
        {
            this.eye.rectTransform.position = this.eyeCenter + new Vector3(
                Random.Range(-2f, 2f),
                Random.Range(-2f, 2f),
                0f);
        }
    }
    private void Start()
    {
        this.eyeCenter = this.eye.transform.position;
        
        this.playerEntity.HealthChanged.AddListener(health =>
        {
            if (this.playerEntity._health <= 0f)
                StartCoroutine(Kill());
        });
    }
}
