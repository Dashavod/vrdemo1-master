using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Experiment_1
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }
        public static Guid sessionId = Guid.NewGuid();

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                return;
            }
            Destroy(this.gameObject);
        }
    }
}