using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TrackerPro.Unity
{
    public abstract class TrackingEventHandler : MonoBehaviour
    {
        public UnityAction<Solution> OnPoseUpdateEvent;
        public void InitializeEvent()
        {
            OnPoseUpdateEvent+=(solution)=>
            {
                if(solution) OnPoseUpdate(solution);
            };
        }
        public abstract void OnPoseUpdate(Solution solution);
    }
}