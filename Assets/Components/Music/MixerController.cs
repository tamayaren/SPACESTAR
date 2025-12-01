using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    public static MixerController controller;

    public AudioMixer mixer;
    private void Awake() => controller = this;
}
