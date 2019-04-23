using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Piranha;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Integration.Tests
{

	/// <summary>
	/// Fixture for <see cref="BucketIntegration"/>.
	/// </summary>
	public class BucketIntegrationFixture : IDisposable
	{

		/// <summary>
		/// Bucket Name used for integration testing.
		/// </summary>
		/// <value>The name of the bucket used for integration testing..</value>
		public string BucketName { get; private set; }

		/// <summary>
		/// The Bucket Website URL used for integration testing.
		/// </summary>
		/// <value>The integration bucket website URL.</value>
		public string BucketWebsite { get; private set; }

		/// <summary>
		/// URL for the integration test bucket.
		/// </summary>
		/// <value>The integration test bucket URL.</value>
		public string BucketUrl { get; private set; }

		/// <summary>
		/// <see cref="S3Storage"/> used for testing.
		/// </summary>
		/// <value>The <see cref="S3Storage"/>used for testing.</value>
		public IStorage S3Storage { get; private set; }

		/// <summary>
		/// Amazon Client to use for outside validation.
		/// </summary>
		/// <value>Amazon S3 client used for validation.</value>
		public IAmazonS3 S3Client { get; private set; }

        /// <summary>
        /// Contructor initializing test fixture.
        /// </summary>
        public BucketIntegrationFixture()
        {

            // Setup Configuration
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false);
            var configuration = configBuilder.Build();

            AWSOptions options = configuration.GetAWSOptions();

            // S3 Configuration
            BucketName = Environment.GetEnvironmentVariable("BUCKET_NAME");
            BucketWebsite = $"http://{BucketName}.s3-website.{options.Region.SystemName}.amazonaws.com/";

            var storageConfig = new PiranhaS3StorageOptions {
                BucketName = BucketName,
                PublicUrlRoot = BucketWebsite
            };

			// Service Provider
			IServiceCollection services = new ServiceCollection();
            services.AddPiranhaS3StorageOptions(storageConfig);
			services.AddDefaultAWSOptions(configuration.GetAWSOptions());
			services.AddPiranhaS3Storage();
			services.AddAWSService<IAmazonS3>();
			var serviceProvider = services.BuildServiceProvider();

			// Get the services
			S3Storage = serviceProvider.GetService<IStorage>();
			S3Client = serviceProvider.GetService<IAmazonS3>();

		}

		/// <summary>
		/// Releases all resource used by the <see cref="BucketIntegrationFixture"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="BucketIntegrationFixture"/>. 
		/// The <see cref="Dispose"/> method leaves the <see cref="BucketIntegrationFixture"/> in an unusable state.
		/// After calling <see cref="Dispose"/>, you must release all references to the <see cref="BucketIntegrationFixture"/> 
		/// so the garbage collector can reclaim the memory that the <see cref=" BucketIntegrationFixture"/> was occupying.</remarks>
		public void Dispose()
		{
			S3Client.Dispose();
		}

	}

}
