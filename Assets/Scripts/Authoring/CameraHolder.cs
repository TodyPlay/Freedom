using UnityEngine;

namespace Authoring
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