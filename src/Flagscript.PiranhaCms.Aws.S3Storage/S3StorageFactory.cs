using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Amazon.Extensions.NETCore.Setup;

namespace Flagscript.PiranhaCms.Aws.S3Storage
{

	/// <summary>
	/// Factory to produce <see cref="S3Storage"/> for a given configuration, used in DI.
	/// </summary>
	internal class S3StorageFactory
	{

		/// <summary>
		/// Override <see cref="PiranhaS3StorageOptions"/>.
		/// </summary>
		/// <value>The override <see cref="PiranhaS3StorageOptions"/>.</value>
		internal PiranhaS3StorageOptions S3StorageOptions { get; private set; }

		/// <summary>
		/// Override <see cref="AWSOptions"/>.
		/// </summary>
		/// <value>The override <see cref="AWSOptions"/>.</value>
		internal AWSOptions AwsOptions { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s3StorageOptions">
		/// The <see cref="S3StorageOptions"/> used to override the default options added using 
		/// <see cref="PiranhaS3ServiceCollectionExtensions.AddPiranhaS3StorageOptions"/>.
		/// </param>
		/// <param name="awsOptions">
		/// The <see cref="AWSOptions"/> used to create the service client overriding 
		/// the default AWS options added using <see cref="ServiceCollectionExtensions.AddDefaultAWSOptions(IServiceCollection, AWSOptions)"/>.
		/// </param>
		internal S3StorageFactory(PiranhaS3StorageOptions s3StorageOptions, AWSOptions awsOptions)
		{
			S3StorageOptions = s3StorageOptions;
			AwsOptions = awsOptions;
		}

		/// <summary>
		/// Creates a <see cref="S3Storage"/> provider searching the service provider
		/// for configuration.
		/// </summary>
		/// <param name="provider">The dependency injection provider.</param>
		/// <returns><see cref="S3Storage"/> to be added to DI.</returns>
		internal S3Storage CreateS3Storage(IServiceProvider provider)
		{

            var loggerFactory = provider.GetService<ILoggerFactory>();
			var logger = loggerFactory?.CreateLogger("Flagscript.PiranhaCms.Aws.S3Storage");

			// Obtain S3 Storage Options
			var s3StorageOptions = S3StorageOptions ?? provider.GetService<PiranhaS3StorageOptions>();
			if (s3StorageOptions == null)
			{
				var configuration = provider.GetService<IConfiguration>();
				if (configuration != null)
				{
					s3StorageOptions = configuration.GetPiranhaS3StorageOptions();
					if (s3StorageOptions != null)
					{
						logger?.LogInformation("Found Piranha S3 Options in IConfiguration");
					}
                    else
                    {
                        s3StorageOptions = new PiranhaS3StorageOptions();
                    }
				}
			}

            // If storage configuration is still null, initialize an empty configuration
            // for environment variable fallback.
            if (s3StorageOptions == null)
            {
                s3StorageOptions = new PiranhaS3StorageOptions();
            }


            // Obtain Aws Options
			var awsOptions = AwsOptions ?? provider.GetService<AWSOptions>();
			if (awsOptions == null)
			{
				var configuration = provider.GetService<IConfiguration>();
				if (configuration != null)
				{
					awsOptions = configuration.GetAWSOptions();
					if (awsOptions != null)
					{
						logger?.LogInformation("Found AWS options in IConfiguration");
					}
				}
			}

			return CreateS3Storage(logger, s3StorageOptions, awsOptions);

		}

		/// <summary>
		/// Creates the AWS service client that implements the service client interface. The AWSOptions object
		/// will be searched for in the IServiceProvider.
		/// </summary>
		/// <param name="logger">Flagscript.PiranhaCms.Aws.S3Storage category logger.</param>
		/// <param name="s3StorageOptions"><see cref="PiranhaS3StorageOptions"/> used to configure the <see cref="S3Storage"/>.</param>
		/// <param name="awsOptions">The <see cref="AWSOptions"/> used to configure the S3 service client.</param>
		/// <returns>The configured <see cref="S3Storage"/>.</returns>
		internal S3Storage CreateS3Storage(ILogger logger, PiranhaS3StorageOptions s3StorageOptions, AWSOptions awsOptions)
		{
		           
			var finalS3StorateOptions = new PiranhaS3StorageOptions
			{
				BucketName = s3StorageOptions.BucketName,
				KeyPrefix = s3StorageOptions.KeyPrefix,
				PublicUrlRoot = s3StorageOptions.PublicUrlRoot
            };

            // Obtain/Validate S3StorageOptions BucketName
            if (string.IsNullOrWhiteSpace(finalS3StorateOptions.BucketName))
			{

				finalS3StorateOptions.BucketName = Environment.GetEnvironmentVariable(PiranhaS3StorageOptions.BucketEnvironmentVariable);

				if (string.IsNullOrEmpty(finalS3StorateOptions.BucketName))
				{
					throw new FlagscriptException("Piranha S3 BucketName not configured");
				}
				else
				{
					logger?.LogInformation("Piranha S3 BucketName found in environment variables");
				}

			}

            // Obtain/Default S3StorageOptions KeyPrefix
            // Validate S3StorageOptions BucketName
            if (string.IsNullOrWhiteSpace(finalS3StorateOptions.KeyPrefix))
            {

                finalS3StorateOptions.KeyPrefix = Environment.GetEnvironmentVariable(PiranhaS3StorageOptions.KeyPrefixEnvironmentVariable);

                if (string.IsNullOrEmpty(finalS3StorateOptions.KeyPrefix))
                {
                    logger?.LogInformation($"Piranha S3 KeyPrefix configured to default => {PiranhaS3StorageOptions.KeyPrefixDefault}");
                }
                else
                {
                    logger?.LogInformation("Piranha S3 KeyPrefix found in environment variables");
                }

                // Default if not found
                if (string.IsNullOrWhiteSpace(finalS3StorateOptions.KeyPrefix))
                {
                    finalS3StorateOptions.KeyPrefix = "uploads";
                }

            }

            // Obtain/Validate S3StorageOptions PublicUrlRoot
            if (string.IsNullOrWhiteSpace(finalS3StorateOptions.PublicUrlRoot))
            {

                finalS3StorateOptions.PublicUrlRoot = Environment.GetEnvironmentVariable(PiranhaS3StorageOptions.UrlRootEnvironmentVariable);

                if (string.IsNullOrEmpty(finalS3StorateOptions.PublicUrlRoot))
                {
                    throw new FlagscriptException("Piranha S3 PublicUrlRoot not configured");
                }
                else
                {
                    logger?.LogInformation("Piranha S3 PublicUrlRoot found in environment variables");
                }

            }

            // Validate we have a valid public uri root. 
            if (!Uri.IsWellFormedUriString(finalS3StorateOptions.PublicUrlRoot, UriKind.Absolute))
            {
                throw new FlagscriptException("Piranha S3 PublicUrlRoot is not a valid endpoint");
            }

            // Validate we have a valid public uri prefix. 
            if (!Uri.IsWellFormedUriString(finalS3StorateOptions.PublicUrlPrefix, UriKind.Absolute))
            {
                throw new FlagscriptException("Piranha S3 PublicUrlPrefix is not a valid endpoint");
            }

            return new S3Storage(finalS3StorateOptions, awsOptions, logger);

        }

	}

}
