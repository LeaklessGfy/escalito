using UnityEngine;

namespace Singleton
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Main;
        [SerializeField] private AudioSource failure;
        [SerializeField] private AudioSource laugh;
        [SerializeField] private AudioSource success;
        [SerializeField] private AudioSource cash;

        private void Awake()
        {
            Main = this;
        }

        public void PlaySuccess()
        {
            success.Play();
        }

        public void PlayFailure()
        {
            failure.Play();
        }

        public void PlayLaugh()
        {
            laugh.Play();
        }

        public void PlayCash()
        {
            cash.Play();
        }
    }
}