
### Additional Instructions
[S3 Setup Instrctions]("./S3 Setup.md")
[Azure Setup Instrctions]("./Azure Setup.md")

### Setting Up Your AppConfig
...

### Setting Up Addressables in Editor

1. Edit the "AddressableAssetSettings" scriptable object.  In the Profiles section you'll want to add two new entries.
   * If using Azure
     * AzureBuildPath = [Lost.UploadAddressableToAzureSettings.BuildPath]
	 * AzureLoadPath = [Lost.UploadAddressableToAzureSettings.LoadPath]
   * If using S3
     * S3BuildPath = [Lost.UploadAddressableToS3Settings.BuildPath]
	 * S3LoadPath = [Lost.UploadAddressableToS3Settings.LoadPath]

2. Open up the addressables window, and right click to and select "Create New Group -> Packed Assets"

3. Rename this new group to a better file name
  * If using Azure I'd recommend something like "Azure Assets Group"
  * If using S3 I'd recommend something like "S3 Assets Group"

4. Now click on your newly made group to editor it.
  * Under Build Path, select the build path you made in step 1
  * Under Load Path, select the load path you made in step 1
  * Under "Asset Bundle Provider Type", set that to "AssetBunleProviderRemoteWebRequest".
