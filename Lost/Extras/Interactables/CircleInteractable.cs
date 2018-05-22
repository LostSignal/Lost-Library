//-----------------------------------------------------------------------
// <copyright file="CircleInteractable.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    public class CircleInteractable : Interactable
    {
        private static readonly Vector3 InvalidVector = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        #pragma warning disable 0649
        [SerializeField] private Transform spinningTransform;

        [Header("Rotation Velocity")]
        [SerializeField] private bool useRotationalVelocity;
        [SerializeField] private float maxRotationalVelocity = 720.0f;
        [SerializeField] private float rotationalDeceleration = 360.0f;
        #pragma warning restore 0649

        private Vector3 previousHitVector = InvalidVector;
        private float rotationalVelocity;
        private float previousVelocity;
        private float zRotation;

        // cached values
        [NonSerialized]
        private float minimumPixelMovementSquared;

        public float ZRotation
        {
            get
            {
                return this.zRotation;
            }

            set
            {
                this.zRotation = value;

                if (this.spinningTransform)
                {
                    this.spinningTransform.localRotation = Quaternion.Euler(0, 0, this.zRotation);
                }
            }
        }

        protected override void OnInput(Input input, Collider collider, Camera camera)
        {
            if (this.previousHitVector == InvalidVector && input.InputState == InputState.Pressed)
            {
                RaycastHit hit;

                if (collider.Raycast(camera.ScreenPointToRay(input.CurrentPosition), out hit, float.MaxValue))
                {
                    this.previousHitVector = (hit.point - collider.transform.position).SetZ(0).normalized;
                    return;
                }
            }
            else if (this.previousHitVector == InvalidVector || Time.deltaTime == 0.0f)
            {
                return;
            }

            Transform colliderTransform = collider.transform;
            Vector3 colliderPosition = colliderTransform.position;
            Plane interactablePlane = new Plane(colliderPosition, colliderPosition + colliderTransform.up, colliderPosition + colliderTransform.right);
            Ray inputRay = camera.ScreenPointToRay(input.CurrentPosition);

            float enter;
            if (interactablePlane.Raycast(inputRay, out enter) == false)
            {
                // if we didnt' hit the plane
                this.rotationalVelocity = this.useRotationalVelocity ? this.previousVelocity : 0.0f;
                this.previousHitVector = InvalidVector;
                this.previousVelocity = 0.0f;

                return;
            }

            // calculating the current hit vector
            Vector3 hitPoint = inputRay.GetPoint(enter);
            Vector3 currentHitVector = (hitPoint - collider.transform.position).SetZ(0).normalized;

            float cosTheta = Vector3.Dot(currentHitVector, this.previousHitVector);
            Vector3 cross = Vector3.Cross(this.previousHitVector, currentHitVector);
            float theta = 0.0f;

            // NOTE [bgish]: Sometimes, if cosTheta is too close to 1, but slightly off (by 1 bit), it turns costTheta into a NaN and breaks every
            //               00111111 10000000 00000000 00000000 - C# thinks it's 1.00000000 and this is fine (0x3F800000)
            //               00111111 10000000 00000000 00000001 - C# thinks it's 1.00000000 and this is bad  (0x3F800001)
            //               Debug.Log("0x3F800000 = " + Mathf.Acos(BitConverter.ToSingle(BitConverter.GetBytes(0x3F800000), 0)));  // 0x3F800000 = 1
            //               Debug.Log("0x3F800001 = " + Mathf.Acos(BitConverter.ToSingle(BitConverter.GetBytes(0x3F800001), 0)));  // 0x3F800001 = NaN
            if (cosTheta < 0.999999f)
            {
                bool didFingerMove = (input.PreviousPosition - input.CurrentPosition).sqrMagnitude > minimumPixelMovementSquared;

                if (didFingerMove)
                {
                    theta = Mathf.Acos(cosTheta) * 180.0f / Mathf.PI * (Vector3.Dot(cross, collider.transform.forward) > 0 ? 1 : -1);
                }
            }

            this.ZRotation += theta;

            if (input.InputState == InputState.Released)
            {
                this.rotationalVelocity = this.useRotationalVelocity ? Mathf.Clamp(this.previousVelocity, -this.maxRotationalVelocity, this.maxRotationalVelocity) : 0.0f;
                this.previousVelocity = 0.0f;
            }
            else
            {
                this.rotationalVelocity = 0.0f;
                this.previousVelocity = theta / Time.deltaTime;
            }

            this.previousHitVector = input.InputState == InputState.Released ? InvalidVector : currentHitVector;
        }

        protected override void Awake()
        {
            base.Awake();

            this.minimumPixelMovementSquared = Input.GetMinimumPixelMovementSquared();
        }

        protected virtual void Update()
        {
            if (this.useRotationalVelocity && this.rotationalVelocity != 0.0f)
            {
                if (this.rotationalVelocity > 0.0f)
                {
                    this.rotationalVelocity -= this.rotationalDeceleration * Time.deltaTime;

                    if (this.rotationalVelocity < 0.0f)
                    {
                        this.rotationalVelocity = 0.0f;
                    }
                }
                else
                {
                    this.rotationalVelocity += this.rotationalDeceleration * Time.deltaTime;

                    if (this.rotationalVelocity > 0.0f)
                    {
                        this.rotationalVelocity = 0.0f;
                    }
                }

                this.ZRotation += this.rotationalVelocity * Time.deltaTime;
            }
        }
    }
}
