using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterviewContentProcessing : MonoBehaviour
{
    InterviewGroupData iGroup_data;

    [SerializeField] TextMeshProUGUI txtContent;
    [SerializeField] Slider fillBar; //was  [SerializeField] Image fillBar;

    [SerializeField] List<AudioClip> mAudClipGroup;
    [SerializeField] AudioSource mAudSource;

    [SerializeField] List<string> groupQuestions;
   // [SerializeField] CareerStoryLandingView landingView;

    [SerializeField] GameObject fillBarObject;

    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject recordingStartedIcon;

    [SerializeField] GameObject recTimeObj;
    [SerializeField] TMP_Text recTimeLeftText;
    [SerializeField] Button NextBtn;

    [SerializeField] GameObject finishPanel;
    float totalTime = 0;

    [SerializeField] float textSpeed = 0.05f;

    [SerializeField] Recorder recorderRef;
    [SerializeField] int currentQuesNo;

    [SerializeField] GameObject countDownObj;
    [SerializeField] TMP_Text countDownText;

    string prevPlayerPrefs;
    int currentQuesGroupNo;

    [SerializeField] private LayerMask interviewCullingMask;
    private LayerMask savedCullMask;

    private Coroutine timeLeftCoroutine = null;
    private void OnDisable()
    {
        txtContent.text = string.Empty;
        mAudSource.Stop();
        StopAllCoroutines();
    }

    public void LoadContent(InterviewGroupData data)
    {
        savedCullMask = Camera.main.cullingMask;
        iGroup_data = data;
        gameObject.SetActive(true);
        int intNo;

        //Get main camera and cull water
        Camera.main.cullingMask = interviewCullingMask;

        //Check which was the interview should be played
        switch (data.coachName)
        {
            case CoachName.Ashleigh:
                Debug.Log("Coach name is Ashleigh");
                 intNo = PlayerPrefs.GetInt("AshIntNo", 0);

                if(intNo >= iGroup_data.questionGroups.Count())
                {
                    intNo = 0;
                }
                prevPlayerPrefs = "AshIntNo";
                currentQuesGroupNo = intNo;
                StartCoroutine(ProcessQuestionGroup(iGroup_data.questionGroups[intNo]));
                break;
            case CoachName.Isabella:
                Debug.Log("Coach name is Isabella");
                 intNo = PlayerPrefs.GetInt("IsaIntNo", 0);
                if (intNo >= iGroup_data.questionGroups.Count())
                {
                    intNo = 0;
                }
                prevPlayerPrefs = "IsaIntNo";
                currentQuesGroupNo = intNo;
                StartCoroutine(ProcessQuestionGroup(iGroup_data.questionGroups[intNo]));
                break;
            case CoachName.Jaxon:
                Debug.Log("Coach name is Jaxon");
                intNo = PlayerPrefs.GetInt("JaxIntNo", 0);
                if (intNo >= iGroup_data.questionGroups.Count())
                {
                    intNo = 0;
                }
                prevPlayerPrefs = "JaxIntNo";
                currentQuesGroupNo = intNo;
                StartCoroutine(ProcessQuestionGroup(iGroup_data.questionGroups[intNo]));
                break;
            case CoachName.Juniper:
                Debug.Log("Coach name is Juniper");
                intNo = PlayerPrefs.GetInt("JuniperIntNo", 0);
                if (intNo >= iGroup_data.questionGroups.Count())
                {
                    intNo = 0;
                }
                prevPlayerPrefs = "JuniperIntNo";
                currentQuesGroupNo = intNo;
                StartCoroutine(ProcessQuestionGroup(iGroup_data.questionGroups[intNo]));
                break;
            case CoachName.Madison:
                Debug.Log("Coach name is Madison");
                intNo = PlayerPrefs.GetInt("MadIntNo", 0);
                if (intNo >= iGroup_data.questionGroups.Count())
                {
                    intNo = 0;
                }
                prevPlayerPrefs = "MadIntNo";
                currentQuesGroupNo = intNo;
                StartCoroutine(ProcessQuestionGroup(iGroup_data.questionGroups[intNo]));
                break;
            case CoachName.Orion:
                Debug.Log("Coach name is Orion");
                intNo = PlayerPrefs.GetInt("OriIntNo", 0);
                if (intNo >= iGroup_data.questionGroups.Count())
                {
                    intNo = 0;
                }
                prevPlayerPrefs = "OriIntNo";
                currentQuesGroupNo = intNo;
                StartCoroutine(ProcessQuestionGroup(iGroup_data.questionGroups[intNo]));
                break;
            case CoachName.Luna:
                Debug.Log("Coach name is Luna");
                intNo = PlayerPrefs.GetInt("LunaIntNo", 0);
                if (intNo >= iGroup_data.questionGroups.Count())
                {
                    intNo = 0;
                }
                prevPlayerPrefs = "LunaIntNo";
                currentQuesGroupNo = intNo;
                StartCoroutine(ProcessQuestionGroup(iGroup_data.questionGroups[intNo]));
                break;
            case CoachName.Quinn:
                Debug.Log("Coach name is Quinn");
                intNo = PlayerPrefs.GetInt("QuinnIntNo", 0);
                if (intNo >= iGroup_data.questionGroups.Count())
                {
                    intNo = 0;
                }
                prevPlayerPrefs = "QuinnIntNo";
                currentQuesGroupNo = intNo;
                StartCoroutine(ProcessQuestionGroup(iGroup_data.questionGroups[intNo]));
                break;
        }

    }

    IEnumerator ProcessQuestionGroup(QuestionGroup questionGroup)
    {
        mAudClipGroup.Clear();
        groupQuestions.Clear();
        currentQuesNo = 1;
        fillBarObject.SetActive(true);
        foreach (AudioClip clip in questionGroup.audioClips)
        {
            mAudClipGroup.Add(clip); //populate audio clips of questions
        }

        foreach (string ques in questionGroup.questions)
        {
            groupQuestions.Add(ques); //populate questions text
        }
        UpdateFillbar();
        for (int i = 0; i < groupQuestions.Count; i++)
        {
            // Play the audio and display the question
            yield return StartCoroutine(ProcessQuestion(i));

            yield return StartCoroutine(ShowCountDown(3));
            timeLeftCoroutine = StartCoroutine(WaitAndInvokeButton(questionGroup.maxDuration)); //To show max duration the player can record their response

            yield return StartCoroutine(WaitForPlayerAnswer(questionGroup.maxDuration));// Allow player to answer the question

            // Activate Next btn
            nextButton.gameObject.SetActive(true);
            yield return new WaitUntil(() => nextClicked); // Wait for "Next" btn click

            if (timeLeftCoroutine != null)
            {
                StopCoroutine(timeLeftCoroutine);
                timeLeftCoroutine = null;
                recordingStartedIcon.SetActive(false);
            }
            nextClicked = false; // Reset the click flag

            // Hide Next btn
            nextButton.gameObject.SetActive(false);
            PlayerPrefs.SetInt("currentRecStage", currentQuesNo);
            currentQuesNo++;
            UpdateFillbar();
        }

        if (LocalAd.instance != null)
            LocalAd.instance.ShowAd("GettingGrown");

        currentQuesGroupNo++;
        PlayerPrefs.SetInt(prevPlayerPrefs, currentQuesGroupNo);

        finishPanel.SetActive(true);
        recorderRef.MergeAllRecordings(mAudClipGroup); //Merge all just recorded clips into 1

        this.gameObject.SetActive(false);

    }
  

    IEnumerator ShowCountDown(int countDownSeconds)
    {
        float timeLeft = countDownSeconds;
        countDownObj.SetActive(true);
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            countDownText.text = Mathf.CeilToInt(timeLeft).ToString();
            yield return null;
        }
        countDownObj.SetActive(false);
    }
    IEnumerator ProcessQuestion(int quesNo)
    {
        // Play audio and show question text
        mAudSource.clip = mAudClipGroup[quesNo];
        mAudSource.Play();

        txtContent.text = groupQuestions[quesNo];
        totalTime = mAudClipGroup[quesNo].length;

        yield return new WaitWhile(() => mAudSource.isPlaying); // Wait until the audio finish
    }

    IEnumerator WaitForPlayerAnswer(int duration)
    {
        float timeLeft = 60f; // 1-minute timer

        recordingStartedIcon.SetActive(true);
        recorderRef.StartRecording(duration);

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            if (playerAnswered)
            {
                playerAnswered = false; // Reset player answer flag
                yield break;
            }

            yield return null; // Wait for the next frame
        }

        Debug.Log("Time is up for answering!");
    }

    IEnumerator WaitAndInvokeButton(int seconds)
    {
        int timeLeft = seconds;
        recTimeObj.SetActive(true);
        while (timeLeft > 0)
        {
            recTimeLeftText.text = "Time Left: " + timeLeft + "s";
            yield return new WaitForSeconds(1);
            timeLeft--;
        }

        recTimeLeftText.text = "Time's Up!";
        NextBtn.onClick.Invoke();
    }

    private void ExitQuestionAnswerUI()
    {
        txtContent.text = string.Empty;
        fillBarObject.SetActive(false);
        gameObject.SetActive(false);

        if (LocalAd.instance != null)
            LocalAd.instance.ShowAd("GettingGrown");
    }
    private bool nextClicked = false;

    private bool playerAnswered = false;

    public void OnNextButtonClick()
    {
        nextClicked = true;
        recorderRef.StopRecording();

    }

    // Called when player answers the question
    public void OnPlayerAnswered()
    {
        playerAnswered = true;
    }
 
    void UpdateFillbar()
    {
        if (groupQuestions.Count > 0)
        {
            fillBar.value = (float)currentQuesNo / groupQuestions.Count;
            Debug.Log($"Fill bar: CURRENT QUES NO: {currentQuesNo} & GROUPQUESTION COUNT: {groupQuestions.Count} so FILL BAR VALUE: {fillBar.value}");
        }
    }

    IEnumerator ProcessText(string text)
    {
        txtContent.text = "";
        Debug.Log("text processor");
        foreach (char ch in text.ToCharArray())
        {
            txtContent.text += ch;
            yield return new WaitForSeconds(textSpeed);
        }
    }

}
