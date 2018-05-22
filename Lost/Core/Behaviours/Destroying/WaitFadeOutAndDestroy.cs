//-----------------------------------------------------------------------
// <copyright file="WaitFadeOutAndDestroy.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class WaitFadeOutAndDestroy : MonoBehaviour
    {
        private static Shader diffuseTrnasparentShader;

        #pragma warning disable 0649
        [SerializeField] private float waitTime = 10;
        [SerializeField] private float fadeSpeed = 1.0f;
        #pragma warning restore 0649

        private MeshRenderer meshRenderer;
        private float currentTime;

        private void OnEnable()
        {
            this.currentTime = this.waitTime;

            if (diffuseTrnasparentShader == null)
            {
                diffuseTrnasparentShader = Shader.Find("Transparent/Diffuse");
            }

            this.meshRenderer = this.GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            this.currentTime -= Time.deltaTime;

            if (this.currentTime < 0)
            {
                if (this.meshRenderer != null && this.meshRenderer.material != null)
                {
                    if (this.meshRenderer.material.shader != diffuseTrnasparentShader)
                    {
                        this.meshRenderer.material.shader = diffuseTrnasparentShader;
                    }

                    Color newColor = this.meshRenderer.material.color.OffsetA(-Time.deltaTime * this.fadeSpeed);
                    this.meshRenderer.material.color = newColor;

                    if (newColor.a <= 0.0f)
                    {
                        Pooler.Destroy(this.gameObject);
                    }
                }
                else
                {
                    Pooler.Destroy(this.gameObject);
                }
            }
        }
    }
}
