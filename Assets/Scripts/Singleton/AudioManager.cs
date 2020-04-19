using UnityEngine;

namespace Singleton
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Main;
        [SerializeField] private AudioSource failure;
        [SerializeField] private AudioSource success;
        [SerializeField] private AudioSource laugh;

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
    }
}