﻿using System;

using Microsoft.Extensions.DependencyInjection;

using Amazon.Extensions.NETCore.Setup;
using Piranha;

namespace Flagscript.PiranhaCms.Aws.S3Storage
{

	public static class PiranhaS3ServiceCollectionExtensions
	{

		/// <summary>
		/// Adds a <see cref="PiranhaS3StorageOptions"/> object to the dependency injection 
		/// framework.
		/// </summary>
		/// <param name="services">The current service collection.</param>
		/// <param name="options">A <see cref="PiranhaS3StorageOptions"/> used to configure <see cref="S3Storage"/>.</param>
		/// <returns>Returns back the <see cref="IServiceCollection"/> to continue the fluent system of service configuration.</returns>
		public static IServiceCollection AddS3StorageOptions(this IServiceCollection services, PiranhaS3StorageOptions options)
		{
			services.Add(new ServiceDescriptor(typeof(PiranhaS3StorageOptions), options));
			return services;
		}

		/// <summary>
		/// Adds AWS S3 Storage as the provider for Piranha CMS.
		/// </summary>
		/// <param name="services">The current service collection.</param>
		/// <param name="scope">The optional service scope. Default is singleton.</param>
		/// <returns>Returns back the <see cref="IServiceCollection"/> to continue the fluent system of service configuration.</returns>
		public static IServiceCollection AddS3Storage(
			this IServiceCollection services,
			ServiceLifetime scope = ServiceLifetime.Singleton
		)
		{

			return AddS3Storage(services, null, null, scope);

		}

		/// <summary>
		/// Adds AWS S3 Storage as the provider for Piranha CMS using specified options.
		/// </summary>
		/// <param name="services">The current service collection.</param>
		/// <param name="s3StorageOptions">Configuration items for <see cref="S3Storage"/>.</param>
		/// <param name="scope">The optional service scope. Default is singleton.</param>
		/// <returns>Returns back the <see cref="IServiceCollection"/> to continue the fluent system of service configuration.</returns>
		public static IServiceCollection AddS3Storage(
			this IServiceCollection services,
			PiranhaS3StorageOptions s3StorageOptions,
			ServiceLifetime scope = ServiceLifetime.Singleton
		)
		{

			return AddS3Storage(services, s3StorageOptions, null, scope);

		}

		/// <summary>
		/// Adds AWS S3 Storage as the provider for Piranha CMS using specified options.
		/// </summary>
		/// <param name="services">The current service collection.</param>
		/// <param name="awsOptions">Configuration for the AWS S3 service client.</param>
		/// <param name="scope">The optional service scope. Default is singleton.</param>
		/// <returns>Returns back the <see cref="IServiceCollection"/> to continue the fluent system of service configuration.</returns>
		public static IServiceCollection AddS3Storage(
			this IServiceCollection services,
			AWSOptions awsOptions,
			ServiceLifetime scope = ServiceLifetime.Singleton
		)
		{

			return AddS3Storage(services, null, awsOptions, scope);

		}

		/// <summary>
		/// Adds AWS S3 Storage as the provider for Piranha CMS using specified options.
		/// </summary>
		/// <param name="services">The current service collection.</param>
		/// <param name="s3StorageOptions">Configuration items for <see cref="S3Storage"/>.</param>
		/// <param name="awsOptions">Configuration for the AWS S3 service client.</param>
		/// <param name="scope">The optional service scope. Default is singleton.</param>
		/// <returns>Returns back the <see cref="IServiceCollection"/> to continue the fluent system of service configuration.</returns>
		public static IServiceCollection AddS3Storage(
			this IServiceCollection services,
			PiranhaS3StorageOptions s3StorageOptions,
			AWSOptions awsOptions,
			ServiceLifetime scope = ServiceLifetime.Singleton
		)
		{			
			// DI , adding AWS Credentials if in service chain
			Func<IServiceProvider, object> factory = new S3StorageFactory(s3StorageOptions, awsOptions).CreateS3Storage;
			services.Add(new ServiceDescriptor(typeof(IStorage), factory, scope));
			return services;
		}

	}

}
