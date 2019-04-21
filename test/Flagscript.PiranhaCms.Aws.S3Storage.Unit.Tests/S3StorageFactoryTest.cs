using System;

using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using static Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests.TestConstants;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Unit Tests for <see cref="S3StorageFactory"/>.
	/// </summary>
	public class S3StorageFactoryTest
	{

		/// <summary>
		/// Test construtor behavior with null Storage Configuration.
		/// </summary>
		[Fact]
		public void TestCtorNull()
		{
			try
			{
				var storageFactory = new S3StorageFactory(null);
			}
			catch(ArgumentNullException ane)
			{
				Assert.Equal("storageConfiguration", ane.ParamName);
			}
		}

		/// <summary>
		/// Validate the factory works if AWS Options are passed in.
		/// </summary>
		[Fact]
		public void TestGetStorageValidateOptionsPassedInAwsOptions()
		{
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

			IServiceCollection services = new ServiceCollection();
			var storageFactory = new S3StorageFactory(s3StorageConfiguration);
			Func<IServiceProvider, object> factory = storageFactory.GetStorageValidateOptions;
			services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
			var serviceProvider = services.BuildServiceProvider();

			var s3Storage = serviceProvider.GetService<S3Storage>();
			Assert.Same(s3Storage._configuration, s3StorageConfiguration);
		}

		/// <summary>
		/// Validate the factory works if AWS Options are chained in the service provider.
		/// </summary>
		[Fact]
		public void TestGetStorageValidateOptionsAwsOptionsInServiceProvider()
		{
			var fakeAwsOptions = new AWSOptions
			{
				Region = RegionEndpoint.USWest2,
				Credentials = new BasicAWSCredentials("accessId", "secretKey")
			};

			var s3StorageConfiguration = new S3StorageConfiguration(
				ValidUnitTestBucketName,
				ValidUnitTestUriHost
			);

			IServiceCollection services = new ServiceCollection();
			services.AddDefaultAWSOptions(fakeAwsOptions);
			var storageFactory = new S3StorageFactory(s3StorageConfiguration);
			Func<IServiceProvider, object> factory = storageFactory.GetStorageValidateOptions;
			services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
			var serviceProvider = services.BuildServiceProvider();

			var s3Storage = serviceProvider.GetService<S3Storage>();
			Assert.Same(fakeAwsOptions, s3Storage._configuration.AwsOptions);
		}

		/// <summary>
		/// Validate the factory works if AWS Options are in configuration.
		/// </summary>
		[Fact]
		public void TestGetStorageValidateOptionsAwsOptionsInConfiguration()
		{
			var fakeAwsOptions = new AWSOptions
			{
				Region = RegionEndpoint.USWest2,
				Credentials = new BasicAWSCredentials("accessId", "secretKey")
			};

			var s3StorageConfiguration = new S3StorageConfiguration(
				ValidUnitTestBucketName,
				ValidUnitTestUriHost
			);

			SharedCredentialsFile credsFile = new SharedCredentialsFile();
			CredentialProfileOptions credsProfileOptions = new CredentialProfileOptions
			{
				AccessKey = "accessKey",
				SecretKey = "secretKey"
			};
			CredentialProfile credsProfile = new CredentialProfile("unit-test-profile", credsProfileOptions);
			credsFile.RegisterProfile(credsProfile);

			var configBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false);
			IConfiguration configuration = configBuilder.Build();

			IServiceCollection services = new ServiceCollection();
			services.AddSingleton(configuration);
			var storageFactory = new S3StorageFactory(s3StorageConfiguration);
			Func<IServiceProvider, object> factory = storageFactory.GetStorageValidateOptions;
			services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
			var serviceProvider = services.BuildServiceProvider();

			var s3Storage = serviceProvider.GetService<S3Storage>();
			Assert.Equal("unit-test-profile", s3Storage._configuration.AwsOptions.Profile);
		}

	}

}
