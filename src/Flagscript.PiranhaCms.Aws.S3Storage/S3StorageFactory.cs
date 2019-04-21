using System;

using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flagscript.PiranhaCms.Aws.S3Storage
{

	/// <summary>
	/// Factory to produce <see cref="S3Storage"/> for a given configuration, used in DI.
	/// </summary>
	internal class S3StorageFactory
	{

		/// <summary>
		/// The storage configuration to use.
		/// </summary>
		internal S3StorageConfiguration _storageConfiguration;

		/// <summary>
		/// Creates the factory for a given <see cref="S3StorageConfiguration"/>.
		/// </summary>
		/// <param name="storageConfiguration">Configuration used for <see cref="S3Storage"/>.</param>
		internal S3StorageFactory(S3StorageConfiguration storageConfiguration)
		{
			_storageConfiguration = storageConfiguration ?? throw new ArgumentNullException(nameof(storageConfiguration));
		}

		/// <summary>
		/// Storage object with potential <see cref="AWSOptions"/> from the <see cref="IServiceProvider"/>
		/// if none are set by the caller.
		/// </summary>
		/// <param name="provider">Current service provider.</param>
		/// <returns><see cref="S3Storage"/> to be added to DI.</returns>
		internal S3Storage GetStorageValidateOptions(IServiceProvider provider)
		{
			if (_storageConfiguration.AwsOptions == null)
			{
				var options = provider.GetService<AWSOptions>();
				if (options == null)
				{
					Console.WriteLine("No Profile");
					var configuration = provider.GetService<IConfiguration>();
					if (configuration != null)
					{
						options = configuration.GetAWSOptions();
					}
				}
				_storageConfiguration.AwsOptions = options;
			}

			return new S3Storage(_storageConfiguration);
		}

	}

}
