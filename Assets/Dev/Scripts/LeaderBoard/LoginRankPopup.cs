#if PlayFab

using _Root.Scripts.Pattern;
using Dacodelaac.Core;
using Dacodelaac.Variables;
using Pancake.GameService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.LeaderBoard
{
    public class LoginRankPopup : BaseMono
    {
        [SerializeField] private CountryCode countryCode;
        [SerializeField] private GameObject countryBoard;
        [SerializeField] private RectTransform content;
        [SerializeField] private CountryUI countryUI;

        [SerializeField] private Image countryFlag;
        [SerializeField] private TMP_Text countryName;

        [SerializeField] private StringVariable countryCodeVariable;
        [SerializeField] private StringVariable userName;
        
        [SerializeField] private PlayFabManager playFabManager;
        [SerializeField] private TMP_InputField inputField;

        [SerializeField] private GameObject error;
        [SerializeField] private TMP_Text errorSubmit;
        [SerializeField] private GameObject loading;

        public void Setup()
        {
            var c = countryCode.Get(countryCodeVariable.Value);
            countryFlag.sprite = c.icon;
            countryName.text = c.name;
            error.SetActive(false);
            errorSubmit.gameObject.SetActive(false);
            countryBoard.SetActive(false);
        }
        
        public void OnClickCountry()
        {
            if (content.childCount > 0)
            {
                countryBoard.SetActive(true);
                return;
            }
            
            content.Clear();
            foreach (var country in countryCode.countryCodeDatas)
            {
                var c = Instantiate(countryUI, content);
                c.Setup(country);
            }
            countryBoard.SetActive(true);
        }

        public void OnClickOk()
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                LogError();
                return;
            }
            
            playFabManager.OnSubmitName(inputField.text + "|" + countryCodeVariable.Value);
            loading.gameObject.SetActive(true);
        }

        private void LogError()
        {
            error.SetActive(true);
        }

        public void LogWrong(string errorMessage)
        {
            errorSubmit.text = errorMessage;
            errorSubmit.gameObject.SetActive(true);
        }

        public void DisplayCountryBoard()
        {
            countryBoard.SetActive(false);
        }
    }
}

#endif
