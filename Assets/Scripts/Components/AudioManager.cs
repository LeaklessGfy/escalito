using System;
using UnityEngine;

namespace Components
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Main;
        
        [SerializeField] private AudioSource success = default;
        [SerializeField] private AudioSource failure = default;

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
    }
}