using UnityEngine;

namespace GMTK_2024
{
    /// <summary>
    /// Pointer Class updates the pointer's rotation and position.
    /// pointsTo is the gameobject that will be pointed at.
    /// border acts like padding around the screen.
    /// pointerRectTransform points to UI Elem (Image) that will be used as the pointer.
    /// indiactorRectTransform points to UI Elem (Image) that will indicate the object being pointed.
    /// Make sure that both of them are aligned on X-axis with 0 rotation under the same parent object, which have this script.
    ///
    /// </summary>
    public class Pointer : MonoBehaviour
    {
        [SerializeField] private Transform pointsTo;
        [SerializeField] private RectTransform pointerRectTransform;
        [SerializeField] private RectTransform indiactorRectTransform;
        [SerializeField] private float border = 100f;

        private RectTransform _rectTransform;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _rectTransform = GetComponent<RectTransform>();
        }


        private void Update()
        {
            var toPosition = pointsTo.position;
            var fromPosition = _mainCamera.transform.position;
            fromPosition.z = 0f;
            var dir = (toPosition - fromPosition).normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            _rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);

            var targetPositionScreenPoint = _mainCamera.WorldToScreenPoint(toPosition);

            var isOffScreen = GetIsOffScreen(targetPositionScreenPoint);

            if (isOffScreen)
            {
                pointerRectTransform.gameObject.SetActive(true);
                indiactorRectTransform.gameObject.SetActive(true);
                var cappedPosition = new Vector3(Mathf.Clamp(targetPositionScreenPoint.x, border, Screen.width - border), Mathf.Clamp(targetPositionScreenPoint.y, border, Screen.height - border), border);

                var pointToWorldPosition = _mainCamera.ScreenToWorldPoint(cappedPosition);
                _rectTransform.position = pointToWorldPosition - CalculateOffsetBetweenPointerAndIndicator(angle);
                _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x, _rectTransform.localPosition.y, 0f);
            }
            else
            {
                pointerRectTransform.gameObject.SetActive(false);
                indiactorRectTransform.gameObject.SetActive(false);
            }
        }

        private bool GetIsOffScreen(Vector3 targetPositionScreenPoint)
        {
            return targetPositionScreenPoint.x <= border || targetPositionScreenPoint.x >= Screen.width - border || targetPositionScreenPoint.y <= border || targetPositionScreenPoint.y >= Screen.height - border;
        }

        private Vector3 CalculateOffsetBetweenPointerAndIndicator(float angle)
        {
            var radialDistanceBetweenPointerAndIndicator = Vector2.Distance(pointerRectTransform.position, indiactorRectTransform.position);
            var offsetX = radialDistanceBetweenPointerAndIndicator * Mathf.Cos(angle * Mathf.Deg2Rad);
            var offsetY = radialDistanceBetweenPointerAndIndicator * Mathf.Sin(angle * Mathf.Deg2Rad);
            return new Vector3(offsetX, offsetY, 0);
        }
    }
}

