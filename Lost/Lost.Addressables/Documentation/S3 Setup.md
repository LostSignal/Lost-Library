
# Setting Up S3 For AssetBundles

### 1) Create a S3 Bucket
--------------------------
* Create a S3 Bucket for your asset bundles.  I name it something like "mygame-assetbundles". This
   is now the {bucket-name}.  Replace every instance of {bucket-name} below with your bucket name.
* Create a index.html and error.html file in root of your new bucket
* Go To Bucket Properties and select "Static website hosting"
  * Set "Index document" to "index.html"
  * Set "Error document" to "error.html"
  * Record "Endpoint". It will be needed in Unity AppSetting
* Add the following bucket permissions

```json
{
    "Version": "2017-11-16",
    "Statement": [
        {
            "Sid": "AllowPublicRead",
            "Effect": "Allow",
            "Principal": {
                "AWS": "*"
            },
            "Action": "s3:GetObject",
            "Resource": "arn:aws:s3:::{bucket-name}/*"
        }
    ]
}
```

### 2) Create a AWS IAM user for this bucket
---------------------------------------------
* Create User:   {bucket-name}          (remote connection only and record access key and secret access key)
* Create Policy: {bucket-name}-policy   (add the polity to user { bucket - name})

```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": "s3:PutObject",
            "Resource": [
                "arn:aws:s3:::{bucket-name}",
                "arn:aws:s3:::{bucket-name}/*"
            ]
        }
    ]
}
```
