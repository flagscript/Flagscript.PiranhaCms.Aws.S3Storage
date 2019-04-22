using System;

namespace Flagscript.PiranhaCms.Aws.S3Storage
{

	/// <summary>
	/// The options used to configure <see cref="S3Storage"/>.
	/// </summary>
	public class PiranhaS3StorageOptions
	{

		/// <summary>
		/// Standard prefix for fallback environment variables.
		/// </summary>
		private const string EnvironmentVariablePrefix = "PIRANHA_S3_";

		/// <summary>
		/// Fallback environment variable to pull <see cref="BucketName"/> from.
		/// </summary>
		public const string BuucketEnvironmentVariable = EnvironmentVariablePrefix + "_BUCKET_NAME";

		/// <summary>
		/// Fallback environment variable to pull <see cref="KeyPrefix"/> from.
		/// </summary>
		public const string KeyPrefixEnvironmentVariable = EnvironmentVariablePrefix + "_KEY_PREFIX";

		/// <summary>
		/// Fallback environment variable to pull <see cref="UrlRoot"/> from.
		/// </summary>
		public const string UrlRootEnvironmentVariable = EnvironmentVariablePrefix + "_URL_ROOT";

		/// <summary>
		/// Name of the S3 bucket where media files will be stored.
		/// </summary>
		/// <value>The name of the S3 bucket where media files will be stored.</value>
		public string BucketName { get; set; }

		/// <summary>
		/// Key prefix to use when adding files to the S3 bucket.
		/// </summary>
		/// <value>The key prefix to use when adding files to the S3 bucket.</value>
		public string KeyPrefix { get; set; }

		/// <summary>
		/// The scheme/host portion of the uri used to serve the media files.
		/// </summary>
		/// <remarks>e.g. S3 Bucket Website URL or CloudFront distribution URL.</remarks>
		/// <value>The scheme/host portion of the uri used to serve the media files.</value>
		public string UrlRoot { get; set; }

		/// <summary>
		/// Indicates whether or not the <see cref="PiranhaS3StorageOptions"/> are 
		/// valid.
		/// </summary>
		/// <returns><c>true</c> if the settings are valid, <c>false</c> otherwise.</returns>
		internal bool IsValid()
		{

			// Check that BucketName and UrlRoot are set.
			if (string.IsNullOrWhiteSpace(BucketName) || string.IsNullOrWhiteSpace(UrlRoot))
			{
				return false;
			}

			// Check that UrlRoot exist
			if (!Uri.IsWellFormedUriString(UrlRoot, UriKind.RelativeOrAbsolute))
			{
				return false;
			}

			return true;
		}

	}

}
