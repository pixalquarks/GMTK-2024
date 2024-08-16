using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CozyChaosSpring2024
{
    public class TextScroll : MonoBehaviour
    {
        [SerializeField] bool playOnAwake = true;
        [SerializeField] float textSpeed;
        TMP_Text textComponent;

        [SerializeField] float time;

        private void Start()
        {
            textComponent = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            time += Time.deltaTime * textSpeed;

            textComponent.ForceMeshUpdate();
            TMP_TextInfo textInfo = textComponent.textInfo;

            for(int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                {
                    continue;
                }

                Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                Vector3 middle = Vector3.zero;
                for (int j = 0; j < 4; j++)
                {
                    middle += verts[charInfo.vertexIndex + j];
                }
                middle /= 4;

                for (int j = 0; j < 4; j++)
                {
                    Vector3 origin = verts[charInfo.vertexIndex + j];

                    if (time > i)
                    {
                        verts[charInfo.vertexIndex + j] = Vector3.Lerp((origin - middle) * 1.2f + middle, origin, time - i);
                        
                    }
                    else
                    {
                        verts[charInfo.vertexIndex + j] = middle;
                    }
                }
            }

            for(int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                textComponent.UpdateGeometry(meshInfo.mesh, i);
            }
        }

        void Scroll()
        {
            
        }
    }
}
