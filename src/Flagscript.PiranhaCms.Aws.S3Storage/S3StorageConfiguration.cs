using System;

using Amazon.Extensions.NETCore.Setup;
using Flurl;

namespace Flagscript.PiranhaCms.Aws.S3Storage
{

	/// <summary>
	/// Configuration items for <see cref="S3Storage"/>.
	/// </summary>
	public class S3StorageConfiguration
	{

		/// <summary>
		/// The URI host environment variable.
		/// </summary>
		/// <remarks>
		/// Environment Variable which will be scanned to get the S3 public 
		/// website URI host, or CloudFront distribution URI host. This will be 
		/// scanned if an explicit URI host is not provided. 
		/// 
		/// This is passed as an environment variable for cases when Piranha CMS
		/// is running in an AWS stack, so the CloudFormation templates can pass
		/// the S3 URI, CloudFront URI, or a Route 53 domain URI in to this 
		/// variable depending upon which service is being used. 
		/// </remarks>
		public const string UriHostEnvironmentVariable = "PIRANHA_AWS_URI_HOST";

		/// <summary>
		/// The AWS options used to create service clients. 
		/// </summary>
		/// <remarks>
		/// This will override the default AWS options added using 
		/// AddDefaultAWSOptions in NuGet package AWSSDK.Extensions.NETCore.Setup.
		/// </remarks>
		/// <value>The AWS options used to create service clients.</value>
		public AWSOptions AwsOptions { get; internal set; }

		/// <summary>
		/// Name of the S3 Bucket.
		/// </summary>
		/// <value>The name of the S3 bucket.</value>
		public string BucketName { get; internal set; }

		/// <summary>
		/// Key prefix to use when adding files to the S3 bucket.
		/// </summary>
		/// <value>The key prefix to use when adding files to the S3 bucket.</value>
		public string KeyPrefix { get; internal set; }

		/// <summary>
		/// The public URL prefix used to access media files.
		/// </summary>
		/// <value>The public URL prefix used to access media files.</value>
		public string PublicUrlPrefix { get; internal set; }

		/// <summary>
		/// The scheme/host portion of the uri used to serve the media files.
		/// </summary>
		/// <value>The scheme/host portion of the uri used to serve the media files.</value>
		public string UriHost { get; internal set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="S3StorageConfiguration"/> class.
		/// </summary>
		/// <param name="bucketName">Name of the S3 Bucket.</param>
		/// <param name="uriHost">The scheme/host portion of the uri used to serve the media files.</param>
		/// <param name="keyPrefix">Key prefix to use when adding files to the S3 bucket.</param>
		/// <param name="awsOptions">The AWS options used to create service clients.</param>
		public S3StorageConfiguration(
			string bucketName, 
			string uriHost = "", 
			string keyPrefix = "uploads", 
			AWSOptions awsOptions = null
		)
		{
			// Set BucketName
			if (string.IsNullOrWhiteSpace(bucketName))
			{
				throw new ArgumentNullException(nameof(bucketName));
			}
			BucketName = bucketName;

			// Set UriHost
			if (string.IsNullOrWhiteSpace(uriHost))
			{
				var envUriHost = Environment.GetEnvironmentVariable(UriHostEnvironmentVariable);
				if (string.IsNullOrWhiteSpace(envUriHost))
				{
					throw new ArgumentNullException(nameof(uriHost));
				}
				UriHost = envUriHost;
			}
			else
			{
				UriHost = uriHost;
			}
			// Validate URI Value
			bool isUri = Uri.IsWellFormedUriString(UriHost, UriKind.RelativeOrAbsolute);
			if (!isUri)
			{
				throw new ArgumentException($"{uriHost} is not a valid Uri.", nameof(uriHost));
			}

			// Set KeyPrefix
			KeyPrefix = keyPrefix ?? throw new ArgumentNullException(nameof(keyPrefix));

			// Set AwsOoptions
			AwsOptions = awsOptions;

			// Build the media access url with key			
			PublicUrlPrefix = Url.Combine(UriHost, KeyPrefix);
		}

	}

}
