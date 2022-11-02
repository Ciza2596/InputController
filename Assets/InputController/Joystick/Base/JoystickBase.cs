﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace joystick
{
    public abstract class JoystickBase : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {    

        //private variable
        private float Horizontal => (_isSnapX) ? SnapFloat(_input.x, AxisTypes.Horizontal) : _input.x;
        private float Vertical => (_isSnapY) ? SnapFloat(_input.y, AxisTypes.Vertical) : _input.y;
        
        
        [SerializeField] private float _handleRange = 1;
        [SerializeField] private float _deadZone = 0;
        [SerializeField] private AxisTypes _axisType = AxisTypes.Both;
        [SerializeField] private bool _isSnapX = false;
        [SerializeField] private bool _isSnapY = false;
        [SerializeField] private Setting _setting;
        

        private Camera _camera;
        private Vector2 _input = Vector2.zero;
        
        
        //protected variable
        protected RectTransform JoystickBody => _setting.JoystickBody;
        
        
        //public variable
        public Vector2 Direction => new Vector2(Horizontal, Vertical);
        public AxisTypes AxisType => _axisType;



        //unity callback

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            //ShowJoystick();
        }

        public void OnDrag(PointerEventData eventData)
        {
            ShowJoystick();
            
            _camera = null;
            if (_setting.Canvas.renderMode == RenderMode.ScreenSpaceCamera)
                _camera = _setting.Canvas.worldCamera;

            var position = RectTransformUtility.WorldToScreenPoint(_camera, JoystickBody.position);
            var radius = JoystickBody.sizeDelta      / 2;
            _input = (eventData.position - position) / (radius * _setting.Canvas.scaleFactor);
            FormatInput();
            HandleInput(_input.magnitude, _input.normalized, radius, _camera);
            _setting.JoystickBodyHandle.anchoredPosition = _input * radius * _handleRange;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            HideJoystick();
            _input = Vector2.zero;
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
            
            Vector2 center = new Vector2(0.5f, 0.5f);
            JoystickBody.pivot = center;

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
        protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
        {
            if (magnitude > _deadZone)
            {
                if (magnitude > 1)
                    _input = normalised;
            }
            else
                _input = Vector2.zero;
        }

        protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
        {
            var localPoint = Vector2.zero;
            var touchPadRectTransform = _setting.TouchPadRectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(touchPadRectTransform, screenPosition, _camera,
                                                                        out localPoint))
            {
                var pivotOffset = touchPadRectTransform.pivot         * touchPadRectTransform.sizeDelta;
                return localPoint - (JoystickBody.anchorMax * touchPadRectTransform.sizeDelta) + pivotOffset;
            }

            return Vector2.zero;
        }

        protected abstract void ShowJoystick();
        protected abstract void HideJoystick();
        


        //private method

        private void FormatInput()
        {
            if (_axisType == AxisTypes.Horizontal)
                _input = new Vector2(_input.x, 0f);
            else if (_axisType == AxisTypes.Vertical)
                _input = new Vector2(0f, _input.y);
        }

        private float SnapFloat(float value, AxisTypes snapAxis)
        {
            if (value == 0)
                return value;

            if (_axisType == AxisTypes.Both)
            {
                float angle = Vector2.Angle(_input, Vector2.up);
                if (snapAxis == AxisTypes.Horizontal)
                {
                    if (angle < 22.5f || angle > 157.5f)
                        return 0;
                    else
                        return (value > 0) ? 1 : -1;
                }
                else if (snapAxis == AxisTypes.Vertical)
                {
                    if (angle > 67.5f && angle < 112.5f)
                        return 0;
                    else
                        return (value > 0) ? 1 : -1;
                }

                return value;
            }
            else
            {
                if (value > 0)
                    return 1;
                if (value < 0)
                    return -1;
            }

            return 0;
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