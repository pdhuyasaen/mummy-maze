using UnityEngine;

namespace Dacodelaac.Inputs
{
    public class InputJoystick : MonoBehaviour
    {
        public Vector3 Direction { get; private set; }
        public bool Stop { get; set; }
        
        // Joystick joystick;
        Camera mainCamera;
        Vector3 cameraForward;
        
        public void Initialize()
        {
            // joystick = GetComponentInChildren<Joystick>();
            // mainCamera = SL.Get<CameraController>("main_camera").Camera;
        }

        public void GetInput()
        {
            if (Stop)
            {
                Direction = Vector3.zero;
                return;
            }
            // Direction = joystick.Direction;
            if (Direction != Vector3.zero)
            {
                cameraForward = mainCamera.transform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();
                Direction = Direction.x * mainCamera.transform.right + Direction.y * cameraForward;
            }
        }
    }
}