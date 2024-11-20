using System;
using UnityEngine;

namespace NMX
{
    [Serializable]
    public class CameraData
    {
        
        [field: SerializeField][Range(0, 10)] public float defaultDistance = 6f, minimumDistance = 1, maximumDistance = 6f;

        [field: SerializeField][Range(0, 10)] public float smoothing = 4f, zoomSensitivity = 3.5f;
    }
}
