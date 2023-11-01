using System.Collections;
using Dacodelaac.DataStorage;
using Dacodelaac.Events;
using Dacodelaac.UI.Popups;
using Dacodelaac.Variables;
using Google.Play.Review;
using UnityEngine;

namespace Dev.Scripts.UI
{
    public class RatingPopup : BasePopup
    {
        #if UNITY_ANDROID
        private ReviewManager reviewManager;
        private PlayReviewInfo playReviewInfo;
#endif
        [SerializeField] private StringEvent logEvent;
        [SerializeField] private BooleanVariable isRateApp;

        public void OnClickYes()
        {
            isRateApp.Value = true;
            GameData.Save();
#if DACODER_RELEASE && !UNITY_EDITOR
            logEvent.Raise("RATING_PRESS_YES");
#endif

#if UNITY_ANDROID
            StartCoroutine(RequestReview());
#endif
        }

        public void OnClickNo()
        {
#if DACODER_RELEASE && !UNITY_EDITOR
            logEvent.Raise("RATING_PRESS_NO");
#endif
            Close();
        }

        public void OnClickClose()
        {
#if DACODER_RELEASE && !UNITY_EDITOR
            logEvent.Raise("RATING_PRESS_X");
#endif
            Close();
        }
        
#if UNITY_ANDROID
        private IEnumerator RequestReview()
        {
            reviewManager = new ReviewManager();
            var requestFlowOperation = reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }

            playReviewInfo = requestFlowOperation.GetResult();

            var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
            yield return launchFlowOperation;
            playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }

            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.
            Close();
        }
#endif
    }
}
