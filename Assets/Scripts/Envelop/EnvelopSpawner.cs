using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_2024
{
    public class EnvelopSpawner : MonoBehaviour
    {
        [SerializeField] private float width;
        [SerializeField] private float height;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(0, 0, 0), new Vector3(width, height, 0));
        }
    }
}
