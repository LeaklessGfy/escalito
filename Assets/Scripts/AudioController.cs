using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Main;
    public AudioSource cash;
    public AudioSource failure;
    public AudioSource laugh;
    public AudioSource success;

    private void Awake()
    {
        Main = this;
    }
}