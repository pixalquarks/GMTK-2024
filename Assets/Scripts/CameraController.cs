using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_2024
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float zoomSpeedMultiplier = 5f;
        [SerializeField] private bool disableZoom;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 100f;
        [SerializeField] private float panningSpeedMultiplier = 1f;
        private Camera _camera;
        private float velocity = 0f;
        private float smoothTime = 0.15f;
        private float _zoom;

        private void Awake()
        {
            _camera = Camera.main;
            _zoom = _camera.orthographicSize;
        }


        private void Update()
        {
            HandleZoom();
            HandlePanning();
        }

        private void HandleZoom()
        {
            if (disableZoom)
                return;
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, _zoom, ref velocity, smoothTime);
            //if (!Input.GetMouseButton(2)) return;
            var zoomDelta = -Input.GetAxis("Mouse ScrollWheel");

            _zoom += zoomDelta * zoomSpeedMultiplier;
            _zoom = Mathf.Clamp(_zoom, minZoom, maxZoom);
        }

        private Vector3 dragOrigin;

        private void HandlePanning()
        {
            if (Input.GetMouseButtonDown(1))
            {
                dragOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(1))
            {
                Vector3 difference = dragOrigin - _camera.ScreenToWorldPoint(Input.mousePosition);
                _camera.transform.position += difference * panningSpeedMultiplier;
            }
        }
    }
}
