using System;
using System.Threading.Tasks;

using Amazon;
using Amazon.Runtime;
using Amazon.Extensions.NETCore.Setup;
using Flurl;
using Xunit;

using static Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests.TestConstants;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Unit Tests for <see cref="S3Storage"/>.
	/// </summary>
	public class S3StorageTest
	{

		/// <summary>
		/// Valid Storage Options used in tests.
		/// </summary>
		/// <value>Valid Storage Options used in tests.</value>
		private static PiranhaS3StorageOptions ValidStorageOptions => new PiranhaS3StorageOptions
		{
			BucketName = ValidUnitTestBucketName,
			KeyPrefix = ValidUnitTestKeyPrefix,
			PublicUrlRoot = ValidUnitTestUriHost
		};

		/// <summary>
		/// Valdiates exception on creation with no configuration object.
		/// </summary>
		[Fact]
		public void TestCtor()
		{

			try
			{

				new S3Storage(null, null, null);
				Assert.True(false, "null storageOptions did not throw ArgumentNullException");

			}
			catch (ArgumentNullException ane)
			{

				Assert.Equal("storageOptions", ane.ParamName);

			}

		}

		/// <summary>
		/// Validates a storage session is returned with fake aws credentials.
		/// </summary>
		[Fact]
		public async Task ValidateOpenAsync()
		{

			// Validate with passed in fake AwsOptions
			var awsOptions = new AWSOptions
			{
				Region = RegionEndpoint.USWest2,
				Credentials = new BasicAWSCredentials("accessId", "secretKey")
			};

			var s3Storage = new S3Storage(ValidStorageOptions, awsOptions, null);

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

				var s3StorageNoCreds = new S3Storage(ValidStorageOptions, null, null);

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

			var storageConfiguration = new PiranhaS3StorageOptions
			{
				BucketName = ValidUnitTestBucketName,
				KeyPrefix = ValidUnitTestKeyPrefix,
				PublicUrlRoot = ValidUnitTestUriHost
			};
			S3Storage s3Storage = new S3Storage(storageConfiguration, null, null);

			// Test null id input
			var returnUri = s3Storage.GetPublicUrl(null, null);
			Assert.Null(returnUri);

			// Test empty id input
			returnUri = s3Storage.GetPublicUrl(new Piranha.Models.Media(), " ");
			Assert.Null(returnUri);

			// Test Valid Url generation
			Piranha.Models.Media media = new Piranha.Models.Media
			{
				Id = Guid.NewGuid(),
				Filename = "MockFileName.txt"
			};
            returnUri = s3Storage.GetPublicUrl(media, media.Filename);
			var expectedUri = Url.Combine(ValidUnitTestUriHost, ValidUnitTestKeyPrefix, media.Id.ToString(), media.Filename);
			Assert.Equal(expectedUri, returnUri);

		}

	}

}
