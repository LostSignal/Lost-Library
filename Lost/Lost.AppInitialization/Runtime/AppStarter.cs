//-----------------------------------------------------------------------
// <copyright file="AppStarter.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppInitialization
{
    using System.Collections;
    using UnityEngine;

    public enum LoginType
    {
        AnonymousDeviceId,
        EmailPasswordDialog,
    }

    // Does this system work will all our projects?
    //   MatchFu, TienLen, Idio, Blueprint, Pebble, Vibe, PullyTabs
    //   Hourglass (Does not need Localization, but that's okay)

    // Resources
    //   AppStarter
    //   MessageBox
    //   LocalizationTable

    // Objects that need to be in addressables
    //   WhatsNew
    //   IAPCatalog
    //     PurchaseItemDialog
    //   DialogManager
    //     StringInputBox
    //     LogInDialog/SignUpDialog (if uses that LoginType)
    //

    // Custom Button Scripts / Prefabs
    //   Link/Unlink Facebook
    //   Change DisplayName

    // Make a Facebook Button(Handles LogIn/Out/Link/Unlink and tells user they'll lost data if they continue)
    // In order for this to work
    //   * What's New Needs at least 1 entry and it needs to be uploaded to Playfab
    //

    //
    // This Class should also detect if the app needs to be reinitialized (if the app was minimized/restored after too long of a time period)
    //

    //
    // IS IT POSSIBLE TO HAVE OFFLINE PLAY WITH THIS SYSTEM?
    //   * Will Addressables be able to load cached items with no internet connect?
    //

    public class AppStarter : SingletonGameObject<AppStarter>
    {
        #pragma warning disable 0649
        [SerializeField] private string defaultLanguage;
        [SerializeField] private Steps[] languages = new Steps[1];

        [SerializeField] private Dialog loadingDialog;

        [Header("Hard References")]
        [SerializeField] private Dialog messageBoxDialog;
        [SerializeField] private GameObject localizationTable;

        [Header("Login")]
        [SerializeField] private LoginType loginType;

        [Header("Soft References")]
        [SerializeField] private WhatsNewManager whatsNew;
        [SerializeField] private IAPCatalog iapCatalog;
        [SerializeField] private DialogManager dialogManager;
        [SerializeField] private StringInputBox stringInputBox; // For changing display name
        [SerializeField] private LazyLoader lazyLoader;

        [Header("Extra Steps")]
        [SerializeField] private bool initializeIapAtStatup;
        [SerializeField] private bool initializePlayFabPushNotifications;
        #pragma warning restore 0649

        Coroutine initializeCoroutine = null;

        private Coroutine Startup()
        {
            if (this.initializeCoroutine == null)
            {
                this.initializeCoroutine = CoroutineRunner.Instance.StartCoroutine(Coroutine());
            }

            return this.initializeCoroutine;

            IEnumerator Coroutine()
            {
                // Show Loading Dialog
                //
                // SetText(Steps.LoadingContent);
                //   * Load in MessageBox
                //   * Load in LocalizationTable
                //
                // SetText(Steps.CheckingForUpdates);
                //   * Calls Whats New (passes in Version, Store) - THIS WOULD WORK BEST IF IT WERE AN ANNONYMOUS FUCTION CALL
                //     * Force App Update If Needed
                //   * Initializes Addressables and sets the location (based on WhatsNew feedback)
                //
                // SetText(Steps.DownloadingContent);
                //   * Preload all Soft References (IAPCatalog, DialogManager, StringInputBox, LazyLoader)
                //     * Figure out of those assets need to be downloaded and bring up a loading bar if they do
                //
                // SetText(Steps.LoggingIn);
                //   * Based on LogInType launch Load/Launch dialogs
                //
                // SetText(Steps.Initializing);
                //   * Callback for when Login is complete and we're ready to initialize our backend
                //
                // If (initializePlayFabPushNotifications)
                //   * PF.PushNotifications.RegisterForPushNotifications();
                //
                // If (initializeIapAtStatup)
                //   * SetText(Steps.InitializingIap);
                //   * Initialize the IAP Catalog with Unity IAP
                //

                this.initializeCoroutine = null;
                yield break;
            }
        }

        [System.Serializable]
        public class Steps
        {
            #pragma warning disable 0649
            [SerializeField] private string language = "en";
            [SerializeField] private string loadingContent = "Loading Content...";
            [SerializeField] private string checkingForUpdates = "Checking for updates...";
            [SerializeField] private string downloadingContent = "Downloading content...";
            [SerializeField] private string loggingIn = "Logging in...";
            [SerializeField] private string initializing = "Initializing...";
            [SerializeField] private string initializingIap = "Initializing IAP...";
            #pragma warning restore 0649

            public string Language => this.language;
            public string LoadingContent => this.loadingContent;
            public string CheckingForUpdates => this.checkingForUpdates;
            public string DownloadingContent => this.downloadingContent;
            public string LoggingIn => this.loggingIn;
            public string Initializing => this.initializing;
            public string InitializingIap => this.initializingIap;
        }
    }
}
