using Ricimi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterviewManager : MonoBehaviour
{
    static InterviewManager instance;
    public static InterviewManager Instance => instance;

    PopupOpener popupOpner;
    [SerializeField] GameObject prfbIntroView;
    [SerializeField] GameObject prfbPublicSpeakView;
    [SerializeField] InterviewGroupData groupData;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            gameObject.name = GetType().Name;
        }
    }
    void Start()
    {
        popupOpner = GetComponent<PopupOpener>();
    }
    public void LoadIntro(InterviewGroupData groupData)
    {
        GameObject obj = popupOpner.OpenPopup(prfbIntroView);
        obj.GetComponent<InterviewLandingPage>().LoadIntro(groupData);
    }

    public void LoadIntroPublicSpeaking(PublicSpeakingQuestions questions)
    {
        GameObject obj = popupOpner.OpenPopup(prfbPublicSpeakView);
        obj.GetComponent<PublicSpeakingLandingPage>().LoadIntro(questions);
    }

}
