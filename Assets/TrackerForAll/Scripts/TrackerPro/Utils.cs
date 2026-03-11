using System;
using UnityEngine;

namespace TrackerPro
{
    public class Utils:MonoBehaviour
    {
        public GameObject target;
        private void Start()
        {
            Debug.Log($"name:{target}");
        }
        private void Update()
        {
            Debug.Log("I am from utils dll");
        }
        private int Add(int a,int b)
        {
            return a + b;
        }
    }
}

