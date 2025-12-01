using TMPro;
using UnityEngine;

public class UITerminal : MonoBehaviour
{
    public static UITerminal instance;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private TextMeshProUGUI msgData;
    [SerializeField] private GameObject terminal;
    
    private PlayerFirstPersonCamera playerCamera;
    private void Awake() => instance = this;
    public void Display(string message)
    {
        MixerController.controller.mixer.SetFloat("MasterVolume", -80f);
        this.playerCamera.cameraLock = false;
        Cursor.lockState = CursorLockMode.None;
        this.terminal.SetActive(true);
        this.message.text = message;

        this.msgData.text = $"MSG.TXT --- VIM -> {message.Length} BYTES";
        Time.timeScale = .001f;
    }

    private void Start()
    {
        this.playerCamera = GameObject.FindFirstObjectByType<PlayerFirstPersonCamera>();
    }

    public void Back()
    {
        MixerController.controller.mixer.SetFloat("MasterVolume", 0f);
        this.playerCamera.cameraLock = true;
        Time.timeScale = 1f;
        this.terminal.SetActive(false);
    }
}
