using TMPro;
using UnityEngine;

namespace joystick.Example
{
    public class ShowJoystickDirectionComponent : MonoBehaviour
    {
        //private variable
        [SerializeField] private JoystickBase _joystick;
        [SerializeField] private TMP_Text direction_Text;
        
        //unity callback
        private void Update()
        {
            if(_joystick is null)
                return;

            var content = $"Joystick Direction: {_joystick.Direction.ToString()}";
            direction_Text.text = content;
            Debug.Log(content);
            
        }

    }
}