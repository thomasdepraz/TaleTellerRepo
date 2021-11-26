using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryStep
{
    [SerializeField] public List<CardFeedback> cardFeedbackContainers = new List<CardFeedback>();
    [SerializeField] public Transform self;
}
