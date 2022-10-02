using UnityEngine;

namespace RPGEngine.Core
{
    public class CameraFacing : MonoBehaviour
    {
        private Camera _camera;
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            if (!_camera) gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.forward = _camera.transform.forward;
        }
    }
}