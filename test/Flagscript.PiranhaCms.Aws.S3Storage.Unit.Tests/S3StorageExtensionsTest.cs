using System;

using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Xunit;

using static Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests.TestConstants;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Unit tests for <see cref="S3StorageExtensions"/>.
	/// </summary>
	public class S3StorageExtensionsTest
	{

		/// <summary>
		/// Ensures service can not be registered with null configuration.
		/// </summary>
		[Fact]
		public void TestAddS3StorageNullConfiguration()
		{
			var services = new ServiceCollection();			

			try
			{
				services.AddS3Storage(null);
				var provider = services.BuildServiceProvider();
				var s3Storage = provider.GetService<IStorage>();
				Assert.True(false, "null storageConfiguration passed to service provider and no exception");
			}
			catch (ArgumentNullException ane)
			{
				Assert.Equal("storageConfiguration", ane.ParamName);
			}
		}

		/// <summary>
		/// Validates the service will register with the proper interface.
		/// </summary>
		[Fact]
		public void TestAddS3StorageProperInterface()
		{
			var services = new ServiceCollection();

			var fakeAwsOptions = new AWSOptions
			{
				Region = RegionEndpoint.USWest2,
				Credentials = new BasicAWSCredentials("accessId", "secretKey")
			};

			var s3StorageConfiguration = new S3StorageConfiguration(
				ValidUnitTestBucketName,
				ValidUnitTestUriHost,
				awsOptions: fakeAwsOptions
			);

			services.AddS3Storage(s3StorageConfiguration);
			IServiceProvider serviceProvider = services.BuildServiceProvider();

			var storageProvider = serviceProvider.GetService<IStorage>();
			Assert.NotNull(storageProvider);
			var testId = Guid.NewGuid().ToString();
			var testUrl = storageProvider.GetPublicUrl(testId);
			Assert.False(string.IsNullOrWhiteSpace(testUrl));
		}

	}

}
