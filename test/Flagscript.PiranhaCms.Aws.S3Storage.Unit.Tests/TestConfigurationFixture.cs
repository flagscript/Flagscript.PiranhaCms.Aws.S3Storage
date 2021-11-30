using Microsoft.Extensions.Configuration;

using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Fixture containing the various appsettings configurations.
	/// </summary>
	public class TestConfigurationFixture
	{

		/// <summary>
		/// Fake <see cref="AWSOptions"/> to use for testing.
		/// </summary>
		/// <value>The appsettings.json configuration.</value>
		public AWSOptions FakeAwsOptions { get; set; }

		/// <summary>
		/// Configuration from appsettings.json.
		/// </summary>
		/// <value>The appsettings.json configuration.</value>
		public IConfiguration MainConfiguration { get; private set; }

		/// <summary>
		/// Configuration from appsettings.root.json.
		/// </summary>
		/// <value>The appsettings.root.json configuration.</value>
		public IConfiguration RootConfiguration { get; private set; }

		/// <summary>
		/// Configuration from appsettings.other.json.
		/// </summary>
		/// <value>The appsettings.other.json configuration.</value>
		public IConfiguration OtherConfiguration { get; private set; }

		/// <summary>
		/// Storage required to test the session
		/// </summary>
		public S3Storage S3Storage { get; private set; }

		/// <summary>
		/// Contructor initializing test fixture.
		/// </summary>
		public TestConfigurationFixture()
		{

			// Fake AWS Options
			FakeAwsOptions = new AWSOptions
			{
				Region = RegionEndpoint.USWest2,
				Credentials = new BasicAWSCredentials("accessId", "secretKey")
			};

			// Main Configuration
			var configBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false);
			MainConfiguration = configBuilder.Build();

			// Root Configuration
			configBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.root.json", optional: false);
			RootConfiguration = configBuilder.Build();

			// Other Configuration
			configBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.other.json", optional: false);
			OtherConfiguration = configBuilder.Build();

			PiranhaS3StorageOptions piranhaS3StorageOptions = new PiranhaS3StorageOptions()
			{
				BucketName = TestConstants.ValidUnitTestBucketName,
				KeyPrefix = TestConstants.ValidUnitTestKeyPrefix,
				PublicUrlRoot = TestConstants.ValidUnitTestUriHost
			};

			S3Storage = new S3Storage(piranhaS3StorageOptions, FakeAwsOptions, null);	

		}

	}

}
