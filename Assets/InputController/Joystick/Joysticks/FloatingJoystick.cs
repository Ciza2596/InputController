using UnityEngine;
using UnityEngine.EventSystems;

namespace joystick
{
    public class FloatingJoystick : JoystickBase, IPointerDownHandler
    {
        //private variable
        [Space] [SerializeField] private CanvasGroup _canvasGroup;

        //unity callback
        private void Awake() => Init();

        public void OnPointerDown(PointerEventData eventData)
        {
            JoystickBody.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        }


        //protected method
        protected override void ShowJoystick()
        {
            if (_canvasGroup.alpha > 0)
                return;

            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        protected override void HideJoystick()
        {
            if (_canvasGroup.alpha == 0)
                return;
            
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}