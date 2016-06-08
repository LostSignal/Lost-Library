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

        private void Update()
        {
            this.waitTime -= Time.deltaTime;

            if (this.waitTime < 0.0f)
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
