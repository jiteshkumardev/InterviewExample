using Ricimi;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterviewLandingPage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtIntro;

    InterviewGroupData iGroup_data;
    [SerializeField] GameObject objLanding;
    [SerializeField] InterviewContentProcessing intContentProcessor;
    [SerializeField] AudioSource introAudSource;

    [SerializeField] GameObject objFees, objLessBalance;
    [SerializeField] Button btnConfirm_GGB;
    [SerializeField] Button btnConfirm_Credit;

    [SerializeField]
    GameObject panelEnd, btnRecord;

    [SerializeField] float textSpeed = 0.05f;

    private CanvasGroup popupCanvasGroup;
    [SerializeField] float fadeDuration = 1f;

    [SerializeField] private LayerMask interviewCullMask;
    private void OnDisable()
    {
        txtIntro.text = string.Empty;
        introAudSource.Stop();
        StopAllCoroutines();
    }
    public void LoadIntro(InterviewGroupData data)
    {
        AudioManager.Instance.Mute();
        CheckCoachAndSetPlayerPrefs(data);

        iGroup_data = data;
        txtIntro.text = data.introText;
        introAudSource.clip = data.introClip;
        introAudSource.mute = false;
        introAudSource.Play();
    }
    void CheckCoachAndSetPlayerPrefs(InterviewGroupData data)
    {
        switch (data.coachName)
        {
            case CoachName.Ashleigh:
                Debug.Log("Coach name is Ashleigh");
                PlayerPrefs.SetInt("InterviewCurrCoach", 0);
                break;
            case CoachName.Isabella:
                Debug.Log("Coach name is Isabella");
                PlayerPrefs.SetInt("InterviewCurrCoach", 1);
                break;
            case CoachName.Jaxon:
                Debug.Log("Coach name is Jaxon");
                PlayerPrefs.SetInt("InterviewCurrCoach", 2);
                break;
            case CoachName.Juniper:
                Debug.Log("Coach name is Juniper");
                PlayerPrefs.SetInt("InterviewCurrCoach", 3);
                break;
            case CoachName.Luna:
                Debug.Log("Coach name is Luna");
                PlayerPrefs.SetInt("InterviewCurrCoach", 4);
                break;
            case CoachName.Madison:
                Debug.Log("Coach name is Madison");
                PlayerPrefs.SetInt("InterviewCurrCoach", 5);
                break;
            case CoachName.Orion:
                Debug.Log("Coach name is Orion");
                PlayerPrefs.SetInt("InterviewCurrCoach", 6);
                break;
            case CoachName.Quinn:
                Debug.Log("Coach name is Quinn");
                PlayerPrefs.SetInt("InterviewCurrCoach", 7);
                break;
        }
    }
    private void Start()
    {
        objLanding.SetActive(true);
        popupCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnStart()
    {
        if (UserManager.Instance.user.player_state.current_ggbs < 50)
        {
            objLessBalance.SetActive(true);
            btnConfirm_GGB.interactable = false;
            btnConfirm_Credit.interactable = true;
        }
        else
        {
            objLessBalance.SetActive(false);
            btnConfirm_GGB.interactable = true;
            btnConfirm_Credit.interactable = true;
        }
        objFees.SetActive(true);
    }
    public void OnConfirm() //Using GGB
    {
        objFees.SetActive(false);
        UserManager.Instance.UpdateGGBs(-50);
        StartSitting();
        Camera.main.cullingMask = interviewCullMask;
        StartCoroutine(LoadCutscene());
    }
    public void OnConfirmCredit() //Using Credit
    {
        objFees.SetActive(false);
        UserManager.Instance.DeductGGBsFromServer("50", "credit_card", "Interview Fees");
        StartSitting();
        Camera.main.cullingMask = interviewCullMask;
        StartCoroutine(LoadCutscene());
    }
    public void StartSitting()
    {
        ThirdPersonController.instance.GetComponent<ThirdPersonController>().enabled = false;
        ThirdPersonController.instance.GetComponent<CharacterController>().enabled = false;
        ThirdPersonController.instance.GetComponent<Animator>().SetBool("isSitting", true);

    }
    public void StopSitting()
    {
        ThirdPersonController.instance.GetComponent<Animator>().SetBool("isSitting", false);
        ThirdPersonController.instance.GetComponent<CharacterController>().enabled = true;
        ThirdPersonController.instance.GetComponent<ThirdPersonController>().enabled = true;
    }
    IEnumerator LoadCutscene()
    {
        popupCanvasGroup.alpha = 0;
        popupCanvasGroup.interactable = false;

        if (UIManager.instance != null)
        {
            UIManager.instance.DisableGeneralUI();
        }

        StartSitting();
        CutsceneManagerSoliloquy.Instance.InterviewCutscene();
        yield return new WaitForSeconds(5f);
        StartCoroutine(FadeIn());
        StartContent();
    }
    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            popupCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        popupCanvasGroup.alpha = 1f;
        popupCanvasGroup.interactable = true;
    }
    void StartContent()
    {
        AudioManager.Instance.Mute();
        objLanding.SetActive(false);
        intContentProcessor.LoadContent(iGroup_data);
    }

    public void OnContentOver()
    {
        btnRecord.SetActive(false);
        panelEnd.SetActive(true);

        Invoke("EnableRecordButton", 0.5f);
    }

    void EnableRecordButton()
    {
        btnRecord.SetActive(true);
    }

    bool IsRecord = false;
    IEnumerator ProcessIntroText(string text)
    {
        txtIntro.text = "";
        Debug.Log("text processor");
        foreach (char ch in text.ToCharArray())
        {
            txtIntro.text += ch;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    public void StopAllContentAndExit()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.EnableGeneralUI();
        }
        UIManager.instance.ComeBackToMainGame();

        if (LocalAd.instance != null)
            LocalAd.instance.ShowAd("GettingGrown");

        if (CutsceneManagerSoliloquy.Instance != null)
            CutsceneManagerSoliloquy.Instance.GetBackToMainGame();

        AudioManager.Instance.UnMute();

        StopSitting();
    }
}
