using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Type", menuName = "Game/Employee Type")]
public class EmployeeType : ScriptableObject
{
    public Sprite icon;
    public Color color = Color.gray;

    public Genre strongGenre, weakGenre;
}
