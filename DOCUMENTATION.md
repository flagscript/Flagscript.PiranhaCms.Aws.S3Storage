# Flagscript.PiranhaCms.Aws.S3Storage Documentation (>= v1.0.0)

Thanks for using Flagscript.PiranhaCms.Aws.S3Storage! If you have any questons or problems, feel free to post a question on the [issues board](../../issues). 

## Contents

1. Configuration
   - Flagscript Library Configuration
   - AWS Credential Configuration
2. Service Registration
3. Using CloudFormation and Environment Variables

## Configuration

This section explains the various options for configuring the Flagscript.PiranhaCms.Aws.S3Storage library and the AWS credentials used by the library.

### Flagscript.PiranhaCms.Aws.S3Storage Configuration

The Flagscript.PiranhaCms.Aws.S3Storage library has 3 configuration settings. These are as follows:

- Bucket Name: Name of S3 bucket with static website hosting enabled.
- Key Prefix: Optional Key Prefix for items written to the S3 bucket. For example, a key prefix of flagscript/dev will write items to [bucket]/flagscript/dev/[item name]. This is an optional setting. If it is not set, it will default to 'uploads'. You can set it to the empty string or null if you wish to write to the root of th ebucket. 
- Public Root Url: The scheme/domain portion of the URL assigned to your S3 bucket, or a CloudFront distribution on top of that bucket. e.g. http://[my-bucket]-int.s3-website-us-west-2.amazonaws.com/

There are two ways to configure/obtain these settings programatically:

- In Code
- JSON Configuration

How programmatically obtained configurations can be used are discussed in the **Service Configuration** section.

#### In Code

The configuration can be directly set in code. This is done via the `PiranhaS3StorageOptions' class. The following snippet shows how to directly configure options:

``` chsarp
using Flagscript.PiranhaCms.Aws.S3Storage;

var s3StorageOptions = new PiranhaS3StorageOptions
{
	BucketName = "my-public-bucket",
	KeyPrefix = "path-to-my-items",
	PublicUrlRoot = "http://domain-to-my-bucket or https://my-cloudfront-bucket-distribution"
};

```

#### In appsettings.json

The settings can also be configured in appsettings.json. The default configuration key is "Flagscript.PiranhaCms.Aws.S3Storage". Below is an example appsettings.json with the default key:

``` json
"Flagscript.PiranhaCms.Aws.S3Storage": {
	"BucketName": "my-public-bucket",
	"KeyPrefix": "path-to-my-items",
	"PublicUrlRoot": "http://domain-to-my-bucket or https://my-cloudfront-bucket-distribution"
}
```

This library contains extensions you can use to retrieve the configuration object after you have built the configuration container in your ConfigureServices method:

``` csharp
using Flagscript.PiranhaCms.Aws.S3Storage;

var configBuilder = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json", optional: false);
var iConfig = configBuilder.Build();

// Get configuration using the default "Flagscript.PiranhaCms.Aws.S3Storage" key:
var s3StorageOptions = iConfig.GetPiranhaS3StorageOptions();

// Put options in another section in config named "My.Other.Key"
s3StorageOptions = iConfig.getPirnhaS3StorageOptions("My.Other.Key");

// Obtain the settings from the root level
s3StorageOptions = iConfig.getPirnhaS3StorageOptions("");
```

### AWS Credential Configuration

Flagscript.PiranhaCms.Aws.S3Storage utilizes standard mechanisms for AWS Credentials. S3 is accessed via the [AmazonS3Client](https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/S3/TS3Client.html) class. With no credentials specified in the application, this will resolve credentials using [dotnet SDK credential resolution](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-creds.html).

In addition to standard credential resolution, this library supports the Amazon.Extensions.NETCore.Setup library extension. You may configure the AWS credentials to be used with this library as described in [Configuring AWS SDK with .NET Core](https://aws.amazon.com/blogs/developer/configuring-aws-sdk-with-net-core/).

## Service Registration

The S3 File Storage for Piranha CMS operates much the same as [Local File Storage](http://piranhacms.org/docs/components/media-storage/local-file-storage) or [Azure Blob Storage](http://piranhacms.org/docs/components/media-storage/azure-blob-storage). You register S3 File Storage in the `ConfigureServices()` method of your application.
