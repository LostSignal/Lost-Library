//-----------------------------------------------------------------------
// <copyright file="Rotate.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class Rotate : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Vector3 rotationSpeed;
        #pragma warning restore 0649

        private Vector3 eulerRotation;

        private void Awake()
        {
            this.eulerRotation = this.transform.localEulerAngles;
        }

        private void Update()
        {
            this.eulerRotation = new Vector3(this.eulerRotation.x + (this.rotationSpeed.x * Time.deltaTime),
                                             this.eulerRotation.y + (this.rotationSpeed.y * Time.deltaTime),
                                             this.eulerRotation.z + (this.rotationSpeed.z * Time.deltaTime));

            this.transform.localEulerAngles = this.eulerRotation;
        }
    }
}
