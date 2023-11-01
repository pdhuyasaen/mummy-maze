using UnityEngine;

namespace Dacodelaac.UI.Popups.Examples
{
    public class Test : MonoBehaviour
    {
        [SerializeField] BasePopupController controller;

        public void Show1()
        {
            controller.Show<Popup1>(true, BasePopupController.ShowAction.DoNothing);
        }

        public void Show2()
        {
            controller.Show<Popup2>(true, BasePopupController.ShowAction.PauseCurrent);
        }

        public void Show3()
        {
            controller.Show<Popup3>(true);
        }
    }
}