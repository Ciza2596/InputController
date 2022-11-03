using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace joystick
{
    public abstract class JoystickBase : MonoBehaviour, IDragHandler, IPointerUpHandler
    {
        //private variable
        private float Horizontal => _isSnapX ? SnapFloat(_direction.x, AxisTypes.Horizontal) : _direction.x;
        private float Vertical => _isSnapY ? SnapFloat(_direction.y, AxisTypes.Vertical) : _direction.y;


        [SerializeField] private float _handleRange = 1;
        [SerializeField] private float _deadZone = 0;
        [Space] [SerializeField] private AxisTypes _axisType = AxisTypes.Both;
        [SerializeField] private bool _isSnapX = false;
        [SerializeField] private bool _isSnapY = false;

        private Camera _camera;
        private Vector2 _direction;


        //protected variable
        [Space] [SerializeField] protected Setting _setting;

        

        //public variable
        public Vector2 Direction => new Vector2(Horizontal, Vertical);

        public AxisTypes AxisType => _axisType;


        //unity callback
        public void OnDrag(PointerEventData eventData)
        {
            ShowJoystick();

            _camera = null;
            if (_setting.Canvas.renderMode == RenderMode.ScreenSpaceCamera)
                _camera = _setting.Canvas.worldCamera;

            var joystickBody = _setting.JoystickBody;
            var position = RectTransformUtility.WorldToScreenPoint(_camera, joystickBody.position);
            var radius = joystickBody.sizeDelta                    / 2;
            _direction = (eventData.position - position) / (radius * _setting.Canvas.scaleFactor);

            FormatInput();
            HandleInput(_direction.magnitude, _direction.normalized);
            _setting.JoystickBodyHandle.anchoredPosition = _direction * radius * _handleRange;
            Debug.Log(_direction);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            HideJoystick();
            _direction = Vector2.zero;
            _setting.JoystickBodyHandle.anchoredPosition = Vector2.zero;
        }


        //public method

        public void Init(AxisTypes axisType = AxisTypes.Both, bool isSnapX = false, bool isSnapY = false)
        {
            SetAxisType(axisType);
            SetIsSnapX(isSnapX);
            SetIsSnapY(isSnapY);

            Init();
        }

        public void Init()
        {
            HideJoystick();

            var center = new Vector2(0.5f, 0.5f);
            _setting.JoystickBody.pivot = center;

            var joystickBodyHandle = _setting.JoystickBodyHandle;
            joystickBodyHandle.anchorMin = center;
            joystickBodyHandle.anchorMax = center;
            joystickBodyHandle.pivot = center;
            joystickBodyHandle.anchoredPosition = Vector2.zero;
        }

        public void SetAxisType(AxisTypes axisType) => _axisType = axisType;
        public void SetIsSnapX(bool isSnapX) => _isSnapX = isSnapX;
        public void SetIsSnapY(bool isSnapY) => _isSnapY = isSnapY;


        //protected method
        private void HandleInput(float magnitude, Vector2 normalised)
        {
            if (magnitude <= _deadZone)
            {
                _direction = Vector2.zero;
                return;
            }

            if (magnitude > 1)
                _direction = normalised;
        }

        protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
        {
            var localPoint = Vector2.zero;
            var touchPadRectTransform = _setting.TouchPadRectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(touchPadRectTransform, screenPosition, _camera,
                                                                        out localPoint))
            {
                var pivotOffset = touchPadRectTransform.pivot * touchPadRectTransform.sizeDelta;
                return localPoint - (_setting.JoystickBody.anchorMax   * touchPadRectTransform.sizeDelta) + pivotOffset;
            }

            return Vector2.zero;
        }

        protected abstract void ShowJoystick();
        protected abstract void HideJoystick();


        //private method
        private void FormatInput()
        {
            if (_axisType == AxisTypes.Horizontal)
                _direction = new Vector2(_direction.x, 0f);
            else if (_axisType == AxisTypes.Vertical)
                _direction = new Vector2(0f, _direction.y);
        }

        private float SnapFloat(float value, AxisTypes snapAxis)
        {
            if (value == 0)
                return value;

            if (_axisType != AxisTypes.Both)
                return value > 0 ? 1 : -1;


            var angle = Vector2.Angle(_direction, Vector2.up);
            if (snapAxis == AxisTypes.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;

                return value > 0 ? 1 : -1;
            }

            if (snapAxis == AxisTypes.Vertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;

                return value > 0 ? 1 : -1;
            }

            return value;
        }

        //model
        [Serializable]
        protected class Setting
        {
            public Canvas Canvas;
            public RectTransform TouchPadRectTransform;

            [Space] public RectTransform JoystickBody;
            public RectTransform JoystickBodyHandle;
        }

        public enum AxisTypes
        {
            Both,
            Horizontal,
            Vertical
        }
    }
}