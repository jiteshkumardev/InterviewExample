using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Question Group", menuName = "ScriptableObj/Interview/New Interview Questions")]
public class QuestionGroup : ScriptableObject
{
    [SerializeField]
    public AudioClip[] audioClips;

    [SerializeField]
    public string[] questions;

    [SerializeField]
    public int maxDuration; // In seconds
}
