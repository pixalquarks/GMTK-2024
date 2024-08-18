using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Type", menuName = "Game/Employee Type")]
public class EmployeeType : ScriptableObject
{
    public Sprite icon;
    public Color color = Color.gray;

    public Genre strongGenre, weakGenre;

    public float GetGenreBonus(Genre genre)
    {
        if (genre == strongGenre) return 1;
        if (genre == weakGenre) return -1;
        return 0;
    }
}
