using UnityEngine;
using UnityEngine.EventSystems;

namespace joystick
{
    public class FloatingJoystick : Joystick
    {
        [Space] [SerializeField] private CanvasGroup _canvasGroup;

        protected override void Start()
        {
            base.Start();
            Hide();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            JoystickBodyBackground.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            Show();
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData){
            Hide();
            base.OnPointerUp(eventData);
        }


        private void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            
        }
    }
}