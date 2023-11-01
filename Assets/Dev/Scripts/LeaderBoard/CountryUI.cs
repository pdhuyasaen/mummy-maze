using Dacodelaac.Core;
using Dacodelaac.DataStorage;
using Dacodelaac.Variables;
using Pancake.GameService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = Dacodelaac.Events.Event;

namespace Dev.Scripts.LeaderBoard
{
    public class CountryUI : BaseMono
    {
        [SerializeField] private TMP_Text countryName;
        [SerializeField] private Image flag;
        [SerializeField] private StringVariable countryCode;
        [SerializeField] private Event closeChooseCountry;

        private CountryCodeData countryCodeData;

        public void Setup(CountryCodeData data)
        {
            countryCodeData = data;
            flag.sprite = data.icon;
            countryName.text = data.name;
        }

        public void OnClick()
        {
            countryCode.Value = countryCodeData.code.ToString();
            GameData.Save();
            closeChooseCountry.Raise();
        }
    }
}
