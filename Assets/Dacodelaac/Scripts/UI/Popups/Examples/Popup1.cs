using Dacodelaac.DebugUtils;

namespace Dacodelaac.UI.Popups.Examples
{
    public class Popup1 : BasePopup
    {
        public void ClickTest()
        {
            Dacoder.Log("Click");
            base.Close();
        }
    }
}