using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Clip
{
    public string id;
    public AudioClip clip;
}
[CreateAssetMenu(menuName = "Sound/Sound Bank", fileName = "New Sound Bank")]
public class SoundBank : ScriptableObject
{
    public List<Clip> clips = new List<Clip>();
}
