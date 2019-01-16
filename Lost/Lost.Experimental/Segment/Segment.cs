//// //
//// // Session Tracking?
//// // Make some stort of storing class (just do a simple file store, but later upgrate to sqlite, or something else)
//// // Ability to append properites to every event
//// //
////
//// //
//// // Check out AmazonHookedPlatformInfo.cs from the AWS Unity Plugin for how to get Make, Model, Etc
//// //
////
//// // have a flag for development build or not?
//// // In GetEventDictionary: if UserId is null, then set the annoymousId to SystemInfo.deviceId?
//// // Keep track of current session?  If the app pauses for too long, reset session id?
//// // Track if we've sent events using the annoymousId, and if we set the UserId, then send an Alias event automatically
//// // If set Age and/or Sex, then resend Identify event
////
//// // Have an option for forwarding all these event to Unity Analytics?
//// // Make special event for tracking purchases?
////
//// // Check out segment Playbook and see if we can use terms they use to get more out of the box support
//// // Also, try to make sure all game agnostic sql I write will be easily applicible to other projects (keep values consistant)
////
//// // session_id, session_start_time
////
//// // device_model     device_make device_platform_name    device_platform_version     device_locale_code  device_locale_language  device_locale_country
//// // iPhone           apple        iPhone OS                9.3.1                        en_US                en                        US
////
//// // shoudl also track Debug.isDevelopemntBuild or is it Debug.isDevBuild?
////
//// // Need a Config class
//// //   * Context
//// //   * Integrations
//// //   * WriteKey
//// //   * MaxBatchCount
//// //   * Age/Sex/UserId/Traits???
////
//// public class SegmentManager
//// {
////     private Dictionary<string, object> context = new Dictionary<string, object>();
////     private Dictionary<string, object> integrations = new Dictionary<string, object>();
////     private List<Dictionary<string, object>> analyticsEvents = new List<Dictionary<string, object>>();
////     private string writeKey;
////
////     public SegmentManager()
////     {
////         var app = new Dictionary<string, object>();
////         app.Add("name", Application.productName);
////         app.Add("version", Application.version);
////
////         var device = new Dictionary<string, object>();
////         device.Add("id", SystemInfo.deviceUniqueIdentifier);
////         device.Add("manufacturer", "");                       //   "manufacturer": "Apple",
////         device.Add("model", SystemInfo.deviceModel);          //
////         device.Add("name", "");                               //   "name": "maguro",
////         device.Add("type", Application.platform);             //   "type": "ios" }  IPhonePlayer || Android
////
////         var library = new Dictionary<string, object>();
////         library.Add("name", "Unity Analytics.NET");
////         library.Add("version", "1.0.0");
////
////         var os = new Dictionary<string, object>();
////         os.Add("name", SystemInfo.operatingSystem);
////
////         var screen = new Dictionary<string, object>();
////         screen.Add("width", UnityEngine.Screen.width);
////         screen.Add("height", UnityEngine.Screen.height);
////         screen.Add("dpi", UnityEngine.Screen.dpi);
////
////         // initializing the context object
////         context.Add("app", app);
////         context.Add("device", device);
////         context.Add("library", library);
////         context.Add("locale", Application.systemLanguage);  // should be "en-US", but it's not :(
////         context.Add("os", os);
////         context.Add("screen", screen);
////         context.Add("timezone", "Europe/Amsterdam");  //TimeZone localZone = TimeZone.CurrentTimeZone
////
////         // setting it so all integrations are turned on
////         integrations["All"] = true;
////     }
////
////     public string UserID { get; set; }
////
////     public void SendTrackEvent(string name, Dictionary<string, object> properties)
////     {
////         var segmentEvent = this.GetEventDictionary("track");
////         segmentEvent.Add("event", name);
////         segmentEvent.Add("properties", properties);
////
////         this.QueueEvent(segmentEvent);
////     }
////
////     public void SendScreenEvent(string name)
////     {
////         var segmentEvent = this.GetEventDictionary("screen");
////         segmentEvent.Add("name", name);
////
////         this.QueueEvent(segmentEvent);
////     }
////
////     public void SendAliasEvent(string previousId)
////     {
////         var segmentEvent = this.GetEventDictionary("alias");
////         segmentEvent.Add("previousId", previousId);
////
////         this.QueueEvent(segmentEvent);
////     }
////
////     public void SendIdentifyEvent(Traits traits)
////     {
////         var segmentEvent = this.GetEventDictionary("identify");
////         segmentEvent.Add("traits", traits);
////
////         this.QueueEvent(segmentEvent);
////     }
////
////     private Dictionary<string, object> GetEventDictionary(string action)
////     {
////         var segmentEvent = new Dictionary<string, object>();
////         segmentEvent.Add("action", action);
////         segmentEvent.Add("messageId", Guid.NewGuid().ToString());
////         segmentEvent.Add("timestamp", this.GetDateTimeNow());
////
////         if (string.IsNullOrEmpty(this.UserID) == false)
////         {
////             segmentEvent.Add("userId", this.UserID);
////         }
////         else
////         {
////             segmentEvent.Add("anonymousId", SystemInfo.deviceId);
////         }
////
////         return segmentEvent;
////     }
////
////     private void QueueEvent(Dictionary<string, object> analyticsEvent)
////     {
////         this.analyticsEvents.Add(analyticsEvent);
////     }
////
////     private void Update()
////     {
////         if (this.analyticsEvents.Count >= 20)
////         {
////             this.UploadBatch();
////         }
////     }
////
////     private void UploadBatch()
////     {
////         var batch = new List<Dictionary<string, object>>(20);
////         // pop off 20 off of this.analyticsEvents and put them in batch
////
////         var importEvent = this.GetEventDictionary("import");
////         importEvent.Add("batch", batch);
////         importEvent.Add("context", this.context);
////         importEvent.Add("integrations", this.integrations);
////         importEvent.Add("sentAt", this.GetDateTimeNow());
////
////         string url = this.client.Config.Host + "/v1/import";
////         string json = JsonConvert.SerializeObject(batch);
////         byte[] jsonBytes = System.Text.UTF8.GetBytes(json);
////
////         var headers = new Dictionary<string, string>();
////         headers.Add("Authorization", this.BasicAuthHeader(batch.WriteKey, string.Empty));
////         headers.Add("ContentType", "application/json");
////         // request.Timeout = (int)this.Timeout.TotalMilliseconds;
////         // request.ServicePoint.Expect100Continue = false;
////
////         WWW www = new WWW(url, jsonBytes, headers);
////
////         while (www.isDone == false)
////         {
////             // just chill, or start a coroutine instead
////         }
////     }
////
////     private string BasicAuthHeader(string username, string password)
////     {
////         var authHeaderBytes = Encoding.Default.GetBytes(username + ":" + password);
////         return "Basic " + Convert.ToBase64String(authHeaderBytes);
////     }
////
////     private GetDateTimeNow()
////     {
////         // https://en.wikipedia.org/wiki/ISO_8601
////         return DateTime.Now.ToString("o");
////     }
//// }
////
//// public enum Sex
//// {
////     Unknown = 1,
////     Male = 2,
////     Female = 4,
////     Any = 7
//// }
////
//// public class Traits : Dictionary<string, object>
//// {
////     public void AddAge(int age)
////     {
////         if (this.ContainsKey(age) == false)
////         {
////             this.Add("age", age);
////         }
////         else
////         {
////             this["age"] = age;
////         }
////     }
////
////     public void AddSex(Sex sex)
////     {
////         if (this.ContainsKey(sex) == false)
////         {
////             this.Add("sex", sex.ToString());
////         }
////         else
////         {
////             this["sex"] = sex.ToString();
////         }
////     }
//// }
////
