using System;
using System.Threading.Tasks;

using Amazon;
using Amazon.Runtime;
using Amazon.Extensions.NETCore.Setup;
using Flurl;
using Xunit;

using static Flagscript.Piranha.Aws.S3Storage.Unit.Tests.TestConstants;

namespace Flagscript.Piranha.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Unit Tests for <see cref="S3Storage"/>.
	/// </summary>
	public class S3StorageTest
	{

		/// <summary>
		/// Valdiates exception on creation with no configuration object.
		/// </summary>
		[Fact]
		public void TestCtor()
		{
			try
			{
				S3Storage s3Storage = new S3Storage(null);
				Assert.True(false, "null configuration did not throw ArgumentNullException");
			}
			catch (ArgumentNullException ane)
			{
				Assert.Equal("configuration", ane.ParamName);
			}
		}

		/// <summary>
		/// Validates a storage session is returned with fake aws credentials.
		/// </summary>
		[Fact]
		public async Task ValidateOpenAsync()
		{
			// Validate with passed in fake AwsOptions
			var s3StorageConfiguration = new S3StorageConfiguration(
				ValidUnitTestBucketName,
				ValidUnitTestUriHost,
				keyPrefix: ValidUnitTestKeyPrefix,
				awsOptions: new AWSOptions { Region = RegionEndpoint.USWest2, Credentials = new BasicAWSCredentials("accessId", "secretKey") }
			);
			var s3Storage = new S3Storage(s3StorageConfiguration);

			using (var s3StorageSession = await s3Storage.OpenAsync())
			{
				Assert.NotNull(s3StorageSession);
			}

			// Validate without creds passed in - we'll use env vars.
			try
			{

				Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "accessId");
				Environment.SetEnvironmentVariable("AWS_SECRET_KEY", "secretKey");
				Environment.SetEnvironmentVariable("AWS_REGION", "us-west-2");

				var s3StorageConfigurationNoCred = new S3StorageConfiguration(
					ValidUnitTestBucketName,
					ValidUnitTestUriHost,
					keyPrefix: ValidUnitTestKeyPrefix
				);
				var s3StorageNoCreds = new S3Storage(s3StorageConfigurationNoCred);

				using (var s3StorageSessionNoCreds = await s3StorageNoCreds.OpenAsync())
				{
					Assert.NotNull(s3StorageSessionNoCreds);
				}

			}
			finally
			{
				Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", null);
				Environment.SetEnvironmentVariable("AWS_SECRET_KEY", null);
				Environment.SetEnvironmentVariable("AWS_REGION", null);
			}
		}

		/// <summary>
		/// Validates the correct Url is generated and other null conditions.
		/// </summary>
		[Fact]
		public void ValidateGetPublicUrl()
		{
			S3StorageConfiguration s3StorageConfiguration = new S3StorageConfiguration(
				ValidUnitTestBucketName,
				ValidUnitTestUriHost,
				keyPrefix: ValidUnitTestKeyPrefix
			);
			S3Storage s3Storage = new S3Storage(s3StorageConfiguration);

			// Test null id input
			var returnUri = s3Storage.GetPublicUrl(null);
			Assert.Null(returnUri);

			// Test empty id input
			returnUri = s3Storage.GetPublicUrl(" ");
			Assert.Null(returnUri);

			// Test Valid Url generation
			string testId = Guid.NewGuid().ToString();
			returnUri = s3Storage.GetPublicUrl(testId);
			var expectedUri = Url.Combine(ValidUnitTestUriHost, ValidUnitTestKeyPrefix, testId);
			Assert.Equal(expectedUri, returnUri);
		}

	}

}
