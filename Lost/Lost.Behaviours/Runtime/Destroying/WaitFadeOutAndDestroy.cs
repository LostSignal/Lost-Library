//-----------------------------------------------------------------------
// <copyright file="WaitFadeOutAndDestroy.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;

    public class WaitFadeOutAndDestroy : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private float waitTime = 10;
        [SerializeField] private float fadeTime = 1.0f;

        [SerializeField] private Material transparentMaterial;
        [SerializeField] private string colorPropertyToAnimate = "_BaseColor";
        [SerializeField] private string[] colorsPropertiesToCopy = new string[1] { "_BaseColor" };
        [SerializeField] private string[] texturesPropertiesToCopy = new string[1] { "_BaseMap" };
        #pragma warning restore 0649

        private Coroutine coroutine;

        private void OnEnable()
        {
            this.coroutine = CoroutineRunner.Instance.StartCoroutine(this.FadeOutCoroutine());
        }

        private void OnDisable()
        {
            if (Platform.IsApplicationQuitting == false)
            {
                CoroutineRunner.Instance.StopCoroutine(this.coroutine);
            }
        }

        private IEnumerator FadeOutCoroutine()
        {
            yield return WaitForUtil.Seconds(this.waitTime);

            var meshRenderer = this.GetComponent<MeshRenderer>();
            var material = meshRenderer?.material;

            if (this.transparentMaterial == null)
            {
                Debug.Log("this.transparentMaterial is NULL!");
            }

            if (material != null && this.transparentMaterial != null)
            {
                Material newTransparent = Object.Instantiate(this.transparentMaterial);

                // Copy over color properties
                if (this.colorsPropertiesToCopy != null)
                {
                    for (int i = 0; i < this.colorsPropertiesToCopy.Length; i++)
                    {
                        newTransparent.SetColor(this.colorsPropertiesToCopy[i], material.GetColor(this.colorsPropertiesToCopy[i]));
                    }
                }

                // Copy over texture properties
                if (this.texturesPropertiesToCopy != null)
                {
                    for (int i = 0; i < this.texturesPropertiesToCopy.Length; i++)
                    {
                        newTransparent.SetTexture(this.texturesPropertiesToCopy[i], material.GetTexture(this.texturesPropertiesToCopy[i]));
                    }
                }

                meshRenderer.material = newTransparent;

                yield return newTransparent.FadeAlpha(this.colorPropertyToAnimate, 1.0f, 0.0f, this.fadeTime);

                Object.Destroy(newTransparent);
            }

            Pooler.Destroy(this.gameObject);
        }
    }
}
