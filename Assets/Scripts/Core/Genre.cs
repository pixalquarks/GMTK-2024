using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Genre", menuName = "Game/Genre")]
public class Genre : ScriptableObject
{
    public Sprite icon;
    public Color color = Color.gray;
}
