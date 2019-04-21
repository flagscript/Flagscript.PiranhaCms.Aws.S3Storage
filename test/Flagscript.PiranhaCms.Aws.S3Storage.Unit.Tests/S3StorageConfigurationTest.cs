using System;

using Amazon.Extensions.NETCore.Setup;
using Xunit;

using static Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests.TestConstants;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Unit tests for <see cref="S3StorageConfigurationTest"/>.
	/// </summary>
	public class S3StorageConfigurationTest
	{

		#region Unit Tests

		/// <summary>
		/// Ensure we didn't change the UriHostEnvironmentVariable without knowing,
		/// this could have major impacts and would need a major revisions.
		/// </summary>
		[Fact]
		public void TestUriHostEnvironmentVariable()
		{
			Assert.Equal("PIRANHA_AWS_URI_HOST", S3StorageConfiguration.UriHostEnvironmentVariable);
		}

		/// <summary>
		/// Validate that bucketName must be set, and sets the <see cref="S3StorageConfiguration.BucketName"/>
		/// property correctly.
		/// </summary>
		[Fact]
        public void TestBucketName()
        {
			// Test null bucketName
			try
			{
				S3StorageConfiguration badBucketConfig = new S3StorageConfiguration(null);
				Assert.True(false, "null bucketName did not throw ArgumentNullException");
			}
			catch (ArgumentNullException ane) 
			{
				Assert.Equal("bucketName", ane.ParamName);
			}

			// Test Empty bucketName
			try
			{
				S3StorageConfiguration badBucketConfig = new S3StorageConfiguration("");
				Assert.True(false, "empty bucketName did not throw ArgumentNullException");
			}
			catch (ArgumentNullException ane) 
			{
				Assert.Equal("bucketName", ane.ParamName);
			}

			// Test Valid bucketName
			S3StorageConfiguration goodBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, ValidUnitTestUriHost);
			Assert.Equal(ValidUnitTestBucketName, goodBucketConfig.BucketName);
		}

		/// <summary>
		/// Validate that keyPrefix must be set, and sets the <see cref="S3StorageConfiguration.KeyPrefix"/>
		/// property correctly.
		/// </summary>
		[Fact]
		public void TestKeyPrefix()
		{
			// Test null keyPrefix
			try
			{
				S3StorageConfiguration badBucketConfig = new S3StorageConfiguration(
					ValidUnitTestBucketName, 
					ValidUnitTestUriHost,
					keyPrefix: null
				);
				Assert.True(false, "null keyPrefix did not throw ArgumentNullException");
			}
			catch (ArgumentNullException ane) 
			{
				Assert.Equal("keyPrefix", ane.ParamName);
			}

			// Test Default keyPrefix
			S3StorageConfiguration goodBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, ValidUnitTestUriHost);
			Assert.Equal("uploads", goodBucketConfig.KeyPrefix);

			// Test Valid keyPrefix
			goodBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, ValidUnitTestUriHost, keyPrefix: ValidUnitTestKeyPrefix);
			Assert.Equal(ValidUnitTestKeyPrefix, goodBucketConfig.KeyPrefix);
		}

		/// <summary>
		/// Validate that awsOptions can or can not be set, and sets the <see cref="S3StorageConfiguration.AwsOptions"/>
		/// property correctly.
		/// </summary>
		[Fact]
		public void TestAwsOptions()
		{
			// Test null awsOptions
			S3StorageConfiguration goodBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, ValidUnitTestUriHost);
			Assert.Null(goodBucketConfig.AwsOptions);

			// Test set awsOptions
			var testAwsOptions = new AWSOptions();
			goodBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, ValidUnitTestUriHost, awsOptions: testAwsOptions);
			Assert.Same(testAwsOptions, goodBucketConfig.AwsOptions);
		}

		/// <summary>
		/// Validate that uriHost must be set, and sets the <see cref="S3StorageConfiguration.UriHost"/>
		/// property correctly.
		/// </summary>
		[Fact]
		public void TestUriHost()
		{
			// Test null uriHost and env var
			try
			{
				S3StorageConfiguration badBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, null);
				Assert.True(false, "null uriHost and envvar did not throw ArgumentNullException");
			}
			catch (ArgumentNullException ane)
			{
				Assert.Equal("uriHost", ane.ParamName);
			}

			// Test empty uriHost and null env var
			try
			{
				S3StorageConfiguration badBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, "");
				Assert.True(false, "empty uriHost and null envvar did not throw ArgumentNullException");
			}
			catch (ArgumentNullException ane)
			{
				Assert.Equal("uriHost", ane.ParamName);
			}

			// Test null uriHost and empty env var
			try
			{
				Environment.SetEnvironmentVariable(S3StorageConfiguration.UriHostEnvironmentVariable, "");
				S3StorageConfiguration badBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, null);
				Assert.True(false, "null uriHost and empty envvar did not throw ArgumentNullException");
			}
			catch (ArgumentNullException ane)
			{
				Assert.Equal("uriHost", ane.ParamName);
			}
			finally
			{
				Environment.SetEnvironmentVariable(S3StorageConfiguration.UriHostEnvironmentVariable, null);
			}

			// Test empty uriHost and empty env var
			try
			{
				Environment.SetEnvironmentVariable(S3StorageConfiguration.UriHostEnvironmentVariable, "");
				S3StorageConfiguration badBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, "");
				Assert.True(false, "empty uriHost and envvar did not throw ArgumentNullException");
			}
			catch (ArgumentNullException ane)
			{
				Assert.Equal("uriHost", ane.ParamName);
			}
			finally
			{
				Environment.SetEnvironmentVariable(S3StorageConfiguration.UriHostEnvironmentVariable, null);
			}

			// Test Malformed uriHost
			try
			{
				S3StorageConfiguration badBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, "not a uri");
				Assert.True(false, "malformed uriHost and did not throw ArgumentNullException");
			}
			catch (ArgumentException ae)
			{
				Assert.Equal("uriHost", ae.ParamName);
			}

			// Test Malformed envvar
			try
			{
				Environment.SetEnvironmentVariable(S3StorageConfiguration.UriHostEnvironmentVariable, "not a uri");
				S3StorageConfiguration badBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, "");
				Assert.True(false, "malformed envvar and did not throw ArgumentNullException");
			}
			catch (ArgumentException ae)
			{
				Assert.Equal("uriHost", ae.ParamName);
			}

			// Valid UriHost
			S3StorageConfiguration goodBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, ValidUnitTestUriHost);
			Assert.Equal(ValidUnitTestUriHost, goodBucketConfig.UriHost);

			// Valid envvar
			try
			{
				Environment.SetEnvironmentVariable(S3StorageConfiguration.UriHostEnvironmentVariable, ValidUnitTestUriHost);
				goodBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, "");
				Assert.Equal(ValidUnitTestUriHost, goodBucketConfig.UriHost);
			}
			finally
			{
				Environment.SetEnvironmentVariable(S3StorageConfiguration.UriHostEnvironmentVariable, null);
			}

			// Presidence
			try
			{
				var newUri = "https://flagscript.tech";
				Environment.SetEnvironmentVariable(S3StorageConfiguration.UriHostEnvironmentVariable, ValidUnitTestUriHost);
				goodBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, newUri);
				Assert.Equal(newUri, goodBucketConfig.UriHost);
			}
			finally
			{
				Environment.SetEnvironmentVariable(S3StorageConfiguration.UriHostEnvironmentVariable, null);
			}
		}

		/// <summary>
		/// Validate that on a proper <see cref="S3StorageConfiguration"/> that 
		/// <see cref="S3StorageConfiguration.PublicUrlPrefix"/> is properly constructed.
		/// </summary>
		[Fact]
		public void TestPublicUrlPrefix()
		{
			// Create good config and test PublicUrlPrefix
			S3StorageConfiguration goodBucketConfig = new S3StorageConfiguration(ValidUnitTestBucketName, ValidUnitTestUriHost, keyPrefix: ValidUnitTestKeyPrefix);
			Assert.Equal($"{ValidUnitTestUriHost}/{ValidUnitTestKeyPrefix}", goodBucketConfig.PublicUrlPrefix);
		}

		#endregion

	}

}
