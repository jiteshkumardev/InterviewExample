using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityApps;

[CreateAssetMenu(fileName = "Interview Group Data", menuName = "ScriptableObj/Interview/New Interview Group")]
public class InterviewGroupData : ScriptableObject
{
    [SerializeField]
    public CoachName coachName;

    [SerializeField]
    public AudioClip introClip;

    [SerializeField]
    public string introText;

    [SerializeField]
    public QuestionGroup[] questionGroups;
}

public enum CoachName
{
    Ashleigh = 0,
    Isabella,
    Jaxon,
    Madison,
    Orion,
    Juniper,
    Luna,
    Quinn
}