#if PlayFab
using PlayFab;
#endif

namespace Pancake.GameService
{
    using System;
    using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;


    // ReSharper disable InconsistentNaming
    [Serializable]
    [CreateAssetMenu(menuName = "Country")]
    public class CountryCode : ScriptableObject
    {
        public string nameTableLeaderboard = "RANK_LEVEL";
        public CountryCodeData[] countryCodeDatas;

        public CountryCodeData Get(string code)
        {
            Enum.TryParse(code, out ECountryCode countryCode);
            return countryCodeDatas[(int) countryCode];
        }

#if UNITY_EDITOR
        [ContextMenu("Execute")]
        public void Execute()
        {
            string[] CountryName = new[]
            {
                "Andorra", "United Arab Emirates", "Afghanistan", "Antigua and Barbuda", "Anguilla", "Albania", "Armenia", "Angola", "Antarctica", "Argentina",
                "American Samoa", "Austria", "Australia", "Aruba", "Aland Islands", "Azerbaijan", "Bosnia and Herzegovina", "Barbados", "Bangladesh", "Belgium",
                "Burkina Faso", "Bulgaria", "Bahrain", "Burundi", "Benin", "Saint Barthélemy", "Bermuda", "Brunei", "Bolivia", "Caribbean Netherlands", "Brazil",
                "The Bahamas", "Bhutan", "Botswana", "Belarus", "Belize", "Canada", "Democratic Republic of the Congo", "Central African Republic",
                "Republic of the Congo", "Switzerland", "Côte d'Ivoire", "Cook Islands", "Chile", "Cameroon", "China", "Colombia", "Costa Rica", "Cuba", "Cape Verde",
                "Curacao", "Christmas Island", "Cyprus", "Czechia", "Germany", "Djibouti", "Denmark", "Dominica", "Dominican Republic", "Algeria", "Ecuador",
                "Estonia", "Egypt", "Western Sahara", "Eritrea", "Spain", "Ethiopia", "Finland", "Fiji", "Falkland Islands", "Federated States of Micronesia",
                "Faroe Islands", "France", "Georgia", "United Kingdom", "Grenada", "Georgia", "French Guiana", "Guernsey", "Ghana", "Gibraltar", "Greenland",
                "The Gambia", "Guinea", "Guadeloupe", "Equatorial Guinea", "Greece", "Guatemala", "Guam", "Guinea-Bissau", "Guyana", "Hong Kong", "Honduras",
                "Croatia", "Haiti", "Hungary", "Indonesia", "Ireland", "Israel", "Isle of Man", "India", "British Indian Ocean Territory", "Iraq", "Iran", "Iceland",
                "Italy", "Jersey", "Jamaica", "Jordan", "Japan", "Kenya", "Kyrgyzstan", "Cambodia", "Kiribati", "Comoros", "Saint Kitts and Nevis", "North Korea",
                "South Korea", "Kuwait", "Cayman Islands", "Kazakhstan", "Laos", "Lebanon", "Saint Lucia", "Liechtenstein", "Sri Lanka", "Liberia", "Lesotho",
                "Lithuania", "Luxembourg", "Latvia", "Libya", "Morocco", "Monaco", "Moldova", "Montenegro", "Saint Martin", "Madagascar", "Marshall Islands",
                "North Macedonia", "Mali", "Myanmar", "Mongolia", "Macao", "Northern Mariana Islands", "Martinique", "Mauritania", "Montserrat", "Malta", "Mauritius",
                "Maldives", "Malawi", "Mexico", "Malaysia", "Mozambique", "Namibia", "New Caledonia", "Niger", "Norfolk Island", "Nigeria", "Nicaragua",
                "Netherlands", "Norway", "Nepal", "Nauru", "Niue", "New Zealand", "Oman", "Panama", "Peru", "French Polynesia", "Papua New Guinea", "Philippines",
                "Pakistan", "Poland", "Saint Pierre and Miquelon", "Puerto Rico", "Palestine", "Portugal", "Palau", "Paraguay", "Qatar", "Reunion", "Romania",
                "Serbia", "Russia", "Rwanda", "South Africa", "Solomon Islands", "Seychelles", "Sudan", "Sweden", "Singapore", "Saint Helena", "Slovenia",
                "Svalbard and Jan Mayen", "Slovakia", "Sierra Leone", "San Marino", "Senegal", "Somalia", "Suriname", "South Sudan", "Sao Tome and Principe",
                "El Salvador", "Sint Maarten", "Syria", "Eswatini", "Turks and Caicos Islands", "Chad", "Togo", "Thailand", "Tajikistan", "Tokelau", "Timor-Leste",
                "Turkmenistan", "Tunisia", "Tonga", "Turkey", "Trinidad and Tobago", "Tuvalu", "Taiwan", "Tanzania", "Ukraine", "Uganda", "United States", "Uruguay",
                "Uzbekistan", "Saint Vincent and the Grenadines", "Venezuela", "British Virgin Islands", "U.S. Virgin Islands", "Vietnam", "Vanuatu",
                "Wallis and Futuna", "Samoa", "Kosovo", "Yemen", "Mayotte", "South Africa", "Zambia", "Zimbabwe"
            };

            countryCodeDatas = new CountryCodeData[241];
            string[] guids2 = AssetDatabase.FindAssets("t:texture2D", new[] {"Assets\\Dev\\Sprites\\CountryIcon"});
            for (int i = 0; i < 241; i++)
            {
                countryCodeDatas[i] = new CountryCodeData() {code = (ECountryCode) i, name = CountryName[i]};

                for (int j = 0; j < guids2.Length; j++)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids2[j]);
                    if (countryCodeDatas[i].code.ToString() == Path.GetFileNameWithoutExtension(path))
                    {
                        countryCodeDatas[i].icon = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    }
                }
            }
        }
#endif
        #if UNITY_EDITOR && ENABLE_PLAYFABSERVER_API
        private int _internalIndex = 0;
        [ContextMenu("Update Aggregation")]
        public void CreateOrUpdateAggregationLeaderboard()
        {
            if (UnityEditor.EditorUtility.DisplayDialog("Update Aggregation All Leaderboard",
                    "Are you sure you wish to update aggregation for all leaderboard to Maximum?\nThis action cannot be reversed.",
                    "Update",
                    "Cancel"))
            {
                _internalIndex = 0;
                Call();
            }

            void Call()
            {
                var c = countryCodeDatas[_internalIndex];
                UnityEditor.EditorUtility.DisplayProgressBar("Update Aggregation Highest Value",
                    $"Updating {c.code.ToString()}...",
                    _internalIndex / (float) countryCodeDatas.Length);
                PlayFabAdminAPI.CreatePlayerStatisticDefinition(new PlayFab.AdminModels.CreatePlayerStatisticDefinitionRequest()
                    {
                        StatisticName = $"{nameTableLeaderboard}_{c.code.ToString()}",
                        AggregationMethod = PlayFab.AdminModels.StatisticAggregationMethod.Max,
                        VersionChangeInterval = PlayFab.AdminModels.StatisticResetIntervalOption.Never
                    },
                    _ =>
                    {
                        if (_internalIndex < countryCodeDatas.Length - 1)
                        {
                            _internalIndex++;
                            Call();
                        }
                        else
                        {
                            Debug.Log("Update Aggregation Completed!");
                            UnityEditor.EditorUtility.ClearProgressBar();
                        }
                    },
                    error =>
                    {
                        if (error.Error == PlayFabErrorCode.StatisticNameConflict)
                        {
                            PlayFabAdminAPI.UpdatePlayerStatisticDefinition(new PlayFab.AdminModels.UpdatePlayerStatisticDefinitionRequest()
                                {
                                    StatisticName = $"{nameTableLeaderboard}_{c.code.ToString()}",
                                    AggregationMethod = PlayFab.AdminModels.StatisticAggregationMethod.Max,
                                    VersionChangeInterval = PlayFab.AdminModels.StatisticResetIntervalOption.Never
                                },
                                _ =>
                                {
                                    if (_internalIndex < countryCodeDatas.Length - 1)
                                    {
                                        _internalIndex++;
                                        Call();
                                    }
                                    else
                                    {
                                        Debug.Log("Update Aggregation Completed!");
                                        UnityEditor.EditorUtility.ClearProgressBar();
                                    }
                                },
                                fabError => { Debug.LogError(fabError.Error); });
                        }
                        else
                        {
                            Debug.LogError(error.Error);
                        }
                    });
            }
        }
#endif
    }

    [Serializable]
    public class CountryCodeData
    {
        public ECountryCode code;
        public Sprite icon;
        public string name;
    }

    public enum ECountryCode
    {
        AD = 0,
        AE,
        AF,
        AG,
        AI,
        AL,
        AM,
        AO,
        AQ,
        AR,
        AS,
        AT,
        AU,
        AW,
        AX,
        AZ,
        BA,
        BB,
        BD,
        BE,
        BF,
        BG,
        BH,
        BI,
        BJ,
        BL,
        BM,
        BN,
        BO,
        BQ,
        BR,
        BS,
        BT,
        BW,
        BY,
        BZ,
        CA,
        CD,
        CF,
        CG,
        CH,
        CI,
        CK,
        CL,
        CM,
        CN,
        CO,
        CR,
        CU,
        CV,
        CW,
        CX,
        CY,
        CZ,
        DE,
        DJ,
        DK,
        DM,
        DO,
        DZ,
        EC,
        EE,
        EG,
        EH,
        ER,
        ES,
        ET,
        FI,
        FJ,
        FK,
        FM,
        FO,
        FR,
        GA,
        GB,
        GD,
        GE,
        GF,
        GG,
        GH,
        GI,
        GL,
        GM,
        GN,
        GP,
        GQ,
        GR,
        GT,
        GU,
        GW,
        GY,
        HK,
        HN,
        HR,
        HT,
        HU,
        ID,
        IE,
        IL,
        IM,
        IN,
        IO,
        IQ,
        IR,
        IS,
        IT,
        JE,
        JM,
        JO,
        JP,
        KE,
        KG,
        KH,
        KI,
        KM,
        KN,
        KP,
        KR,
        KW,
        KY,
        KZ,
        LA,
        LB,
        LC,
        LI,
        LK,
        LR,
        LS,
        LT,
        LU,
        LV,
        LY,
        MA,
        MC,
        MD,
        ME,
        MF,
        MG,
        MH,
        MK,
        ML,
        MM,
        MN,
        MO,
        MP,
        MQ,
        MR,
        MS,
        MT,
        MU,
        MV,
        MW,
        MX,
        MY,
        MZ,
        NA,
        NC,
        NE,
        NF,
        NG,
        NI,
        NL,
        NO,
        NP,
        NR,
        NU,
        NZ,
        OM,
        PA,
        PE,
        PF,
        PG,
        PH,
        PK,
        PL,
        PM,
        PR,
        PS,
        PT,
        PW,
        PY,
        QA,
        RE,
        RO,
        RS,
        RU,
        RW,
        SA,
        SB,
        SC,
        SD,
        SE,
        SG,
        SH,
        SI,
        SJ,
        SK,
        SL,
        SM,
        SN,
        SO,
        SR,
        SS,
        ST,
        SV,
        SX,
        SY,
        SZ,
        TC,
        TD,
        TG,
        TH,
        TJ,
        TK,
        TL,
        TM,
        TN,
        TO,
        TR,
        TT,
        TV,
        TW,
        TZ,
        UA,
        UG,
        US,
        UY,
        UZ,
        VC,
        VE,
        VG,
        VI,
        VN,
        VU,
        WF,
        WS,
        XK,
        YE,
        YT,
        ZA,
        ZM,
        ZW,
    }
}