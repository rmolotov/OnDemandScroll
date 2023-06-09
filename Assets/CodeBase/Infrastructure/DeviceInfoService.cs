﻿using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastructure
{
    public class DeviceInfoService : MonoBehaviour
    {
        #region Singleton

        private void Awake() => Instance ??= this;
        public static DeviceInfoService Instance { get; private set; }

        #endregion
        
        [SerializeField] private CanvasScaler canvasScaler;

        
        public float GetScreenHeight()
        {
            return canvasScaler
                ? canvasScaler.referenceResolution.y
                : Screen.height < Screen.width
                    ? Screen.height
                    : Screen.width;
        }
    }
}