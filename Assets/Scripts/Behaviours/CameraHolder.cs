using UnityEngine;

namespace Behaviours
{
    public class CameraHolder : MonoBehaviour
    {
        public new static Camera camera;

        void Awake()
        {
            camera = GetComponent<Camera>();
        }
    }
}