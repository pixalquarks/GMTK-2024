using System;
using UnityEngine;

namespace CozyChaosSpring2024
{
    public enum ClipType
    {
        Menu,
        Level,
        Sfx
    }
    [Serializable]
    public class M_AudioClip
    {
        public string name;
        public AudioClip clip;
        public ClipType type;
    }
}