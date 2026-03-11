using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TrackerPro
{
    public class Config : MonoBehaviour
    {
        public static Config Instance;
        [HideInInspector]
        public float cameraDistance;
        [HideInInspector]
        public Vector3 scaleOffset;
        [HideInInspector]
        public Vector3 positionOffset;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

