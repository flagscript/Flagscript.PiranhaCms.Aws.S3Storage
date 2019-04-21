using System;

using Microsoft.Extensions.DependencyInjection;
using Piranha;

namespace Flagscript.Piranha.Aws.S3Storage
{

	public static class S3StorageExtensions
	{

		/// <summary>
		/// Adds the services for the AWS S3 service.
		/// </summary>
		/// <param name="services">The current service collection.</param>
		/// <param name="storageConfiguration">Configuration items for <see cref="S3Storage"/>..</param>
		/// <param name="scope">The optional service scope. Default is singleton.</param>
		/// <returns>The service collection.</returns>
		public static IServiceCollection AddS3Storage(
			this IServiceCollection services,
			S3StorageConfiguration storageConfiguration,
			ServiceLifetime scope = ServiceLifetime.Singleton
		)
		{			
			// DI , adding AWS Credentials if in service chain
			Func<IServiceProvider, object> factory = new S3StorageFactory(storageConfiguration).GetStorageValidateOptions;
			services.Add(new ServiceDescriptor(typeof(IStorage), factory, scope));
			return services;
		}

	}

}
