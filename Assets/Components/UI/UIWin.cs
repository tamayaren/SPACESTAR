using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIWin : MonoBehaviour
{
    public static UIWin instance;
    [SerializeField] private Entity playerEntity;
    [SerializeField] private GameObject parent;

    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private GameObject retry;

    public Vector3 eyeCenter;
    public void Retry()
    {
        SceneManager.LoadScene(0);
    }

    public void Awake() => instance = this;
    public IEnumerator Win(PlayerFirstPersonCamera playerCamera)
    {
        MixerController.controller.mixer.SetFloat("MasterVolume", -80f);
        playerCamera.cameraLock = false;
            
        yield return new WaitForSeconds(1f);
        this.parent.SetActive(true);

        this.text.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(3f);
        Cursor.lockState = CursorLockMode.None;
        
        this.retry.SetActive(true);
    }
}
