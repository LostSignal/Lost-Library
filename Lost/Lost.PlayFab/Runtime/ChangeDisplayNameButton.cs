//-----------------------------------------------------------------------
// <copyright file="ChangeDisplayNameButton.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class ChangeDisplayNameButton : MonoBehaviour
    {
#pragma warning disable 0649
        [HideInInspector, SerializeField] private Button button;
        [SerializeField] private UnityEvent onNameChangedSuccess;
        [SerializeField] private UnityEvent onNameChangedFailed;
#pragma warning restore 0649

        private void OnValidate()
        {
            this.AssertGetComponent(ref this.button);
        }

        private void Awake()
        {
            this.OnValidate();
            this.button.onClick.AddListener(this.ButtonClicked);
        }

        private void ButtonClicked()
        {
            this.button.interactable = false;

            CoroutineRunner.Instance.StartCoroutine(Coroutine());

            IEnumerator Coroutine()
            {
                var changeName = PF.User.ChangeDisplayNameWithPopup();

                yield return changeName;

                if (changeName.HasError == false)
                {
                    this.onNameChangedSuccess?.Invoke();
                }
                else
                {
                    this.onNameChangedFailed?.Invoke();
                }

                this.button.interactable = true;
            }
        }
    }
}

#endif
