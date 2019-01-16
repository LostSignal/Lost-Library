//-----------------------------------------------------------------------
// <copyright file="TestFlightUploadSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

//// * XCode Upload to TestFlight
////   * https://gist.github.com/dlo/1568eb5d3317d8b98bc393e58b0f84b2
////   * https://gist.github.com/keith/5b5f61f4cc690aec403afd92aab020c3
////
////
//// * https://developer.cloud.unity3d.com/orgs/lost-signal-llc/projects/tienlen/buildtargets/tienlen-ios-dev/builds/9/log/text/
////
//// /BUILD_PATH/lost-signal-llc.tienlen.tienlen-ios-dev/.build/last/tienlen-ios-dev/build.ipa
////
//// Can you read the xcode project and get the ipa build location?
////
//// ```
//// $ xcodebuild -showBuildSettings -scheme Unity-iPhone -project./Unity-iPhone.xcodeproj -configuration Release
//// Command timed out after 3 seconds on try 1 of 4, trying again with a 6 second timeout...
//// Detected provisioning profile mapping: {
////     "com.BigBlindInteractive.TienLen" = &#62;"8d9a8d26-75c7-47de-ab41-67216c37d7ad"}
////
//// output_directory = / BUILD_PATH / lost - signal - llc.tienlen.tienlen - ios - dev /.build / last / tienlen - ios - dev
//// xcode_path = / APPLICATION_PATH / Xcode9_4_1.app
////
//// / BUILD_PATH / lost - signal - llc.tienlen.tienlen - ios - dev /.build / last / tienlen - ios - dev / build.ipa
//// ```
////
//// PBXProject.GetBuildPropertyForConfig("Release", "output_directory") + "/build.ipa"
////
//// * iOS: Added a AddShellScriptBuildPhase public method, plus InsertShellScriptBuildPhase and InsertCopyFileBuildPhase methods which accept an index parameter to the Xcode API.
////   * https://docs.unity3d.com/2018.3/Documentation/ScriptReference/iOS.Xcode.PBXProject.AddShellScriptBuildPhase.html
////
////
////
//// * https://developer.apple.com/videos/play/wwdc2018/301/
////   * Transporter will be updated to use new App Store Connect json web tokens
////
//// * Looks like altool is still the best way to go
////   * https://medium.com/xcblog/5-tools-for-uploading-ios-apps-to-itunes-connect-d8afa436c415
////   * $ altool --upload-app -f "path/to/file.ipa" -u %USERNAME% -p %PASSWORD%
////
//// * NEVERMIND - Transport may work on Windows
////   * https://itunespartner.apple.com/en/movies/faq/Transporter_Getting%20Set%20Up
////
//// * Upload ipa to testflight? (https://gist.github.com/beatspace9/2166620)
////
//// * Can I upload through App Store Connect now? (https://appstoreconnect.apple.com/WebObjects/iTunesConnect.woa/ra/ng/app)
////   * https://developer.apple.com/videos/play/wwdc2018/301/
////   * https://developer.apple.com/videos/play/wwdc2018/303/
////

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(430)]
    public class TestFlightUploadSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private string username;
        [SerializeField] private string password;
        #pragma warning restore 0649

        public override string DisplayName => "TestFlight Upload";
        public override bool IsInline => false;
    }
}
