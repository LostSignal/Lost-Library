//-----------------------------------------------------------------------
// <copyright file="WaitAndDestroy.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class WaitAndDestroy : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private float waitTime = 10.0f;
        [SerializeField] private GameObject destroyEffect;
        #pragma warning restore 0649

        private float currentTime = 0.0f;

        private void OnEnable()
        {
            this.currentTime = this.waitTime;
        }

        private void Update()
        {
            this.currentTime -= Time.deltaTime;

            if (this.currentTime < 0.0f)
            {
                if (this.destroyEffect != null)
                {
                    GameObject destroyEffect = Pooler.Instantiate(this.destroyEffect);
                    destroyEffect.transform.position = this.transform.position;
                }

                Pooler.Destroy(this.gameObject);
            }
        }
    }
}
