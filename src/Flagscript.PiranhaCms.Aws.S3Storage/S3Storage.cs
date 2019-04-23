using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Amazon.Extensions.NETCore.Setup;
using Flurl;
using Piranha;

namespace Flagscript.PiranhaCms.Aws.S3Storage
{

	/// <summary>
	/// <see cref="IStorage"/> provider for AWS. 
	/// </summary>
	/// <remarks>
	/// Can be used with S3 Bucket Websites, CloudFront distributions fronting 
	/// S3, which can be behind a Route53 domain.
	/// </remarks>
	public class S3Storage : IStorage
	{

        /// <summary>
        /// Configuration options for <see cref="S3Storage"/>.
        /// </summary>
        /// <value>Configuration options for <see cref="S3Storage"/>.</value>
        internal PiranhaS3StorageOptions StorageOptions { get; private set; }

        /// <summary>
        /// Configuration options for the Amazon S3 Client.
        /// </summary>
        /// <value>Configuration options for the Amazon S3 Client.</value>
        internal AWSOptions AwsOptions { get; private set; }

        /// <summary>
        /// Namespace <see cref="ILogger"/> used for logging.
        /// </summary>
        /// <value>Namespace <see cref="ILogger"/> used for logging.</value>
        internal ILogger Logger { get; private set; }

        /// <summary>
        /// Creates a new <see cref="S3Storage"/> with a specified configuration.
        /// </summary>
		/// <param name="storageOptions"><see cref="S3StorageOptions"/> used to configure the Piranda S3 storage.</param>
		/// <param name="awsOptions">The <see cref="AWSOptions"/> used to create the S3 service client.</param>
        /// <param name="logger">Namespace <see cref="ILogger"/> used for logging.</param>
		internal S3Storage(PiranhaS3StorageOptions storageOptions, AWSOptions awsOptions, ILogger logger)
		{
            StorageOptions = storageOptions ?? throw new ArgumentNullException(nameof(storageOptions));
            AwsOptions = awsOptions;

        }

		/// <summary>
		/// Opens a new storage session.
		/// </summary>
		/// <returns>A new open session</returns>
		public Task<IStorageSession> OpenAsync()
		{
            Logger?.LogDebug("Opening Piranha S3 media storage session");
			return Task.FromResult<IStorageSession>(new S3StorageSession(StorageOptions, AwsOptions, Logger));
		}

		/// <summary>
		/// Gets the public URL for the given media object.
		/// </summary>
		/// <param name="id">The media resource id</param>
		/// <returns>The public URL.</returns>
		public string GetPublicUrl(string id)
		{
			if (!string.IsNullOrWhiteSpace(id))
			{
				return Url.Combine(StorageOptions.PublicUrlPrefix, id);
			}
			return null;
		}

	}

}