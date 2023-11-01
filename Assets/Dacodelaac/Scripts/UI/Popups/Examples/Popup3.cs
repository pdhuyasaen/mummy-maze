using Dacodelaac.DebugUtils;

namespace Dacodelaac.UI.Popups.Examples
{
    public class Popup3 : BasePopup
    {
        public void ClickTest()
        {
            Dacoder.Log("Click");
            base.Close();
        }
    }
}