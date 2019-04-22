using Microsoft.Extensions.Configuration;

namespace Flagscript.PiranhaCms.Aws.S3Storage
{

	/// <summary>
	/// This class adds extension methods to IConfiguration making it easier to pull out
	/// S3StorageConfiguration configuration options.
	/// </summary>
	public static class PiranhaS3ConfigurationExtensions
	{

		/// <summary>
		/// The default section where settings are read from the IConfiguration object. 
		/// This is set to "Flagscript.PiranhaCms.Aws.S3Storage".
		/// </summary>
		public const string DEFAULT_CONFIG_SECTION = "Flagscript.PiranhaCms.Aws.S3Storage";

		/// <summary>
		/// Constructs an <see cref="PiranhaS3StorageOptions"/> class with the options 
		/// specifed in the "Flagscript.PiranhaCms.Aws.S3Storage" section in the IConfiguration object.
		/// </summary>
		/// <param name="config"><see cref="IConfiguration"/> to check</param>
		/// <returns>The <see cref="PiranhaS3StorageOptions"/> containing the values set in configuration system.</returns>
		public static PiranhaS3StorageOptions GetS3StorageOptions(this IConfiguration config)
		{
			return GetS3StorageOptions(config, DEFAULT_CONFIG_SECTION);
		}

		/// <summary>
		/// Constructs an <see cref="PiranhaS3StorageOptions"/> class with the options 
		/// specifed in the "Flagscript.PiranhaCms.Aws.S3Storage" section in the IConfiguration object.
		/// </summary>
		/// <param name="config"><see cref="IConfiguration"/> to check</param>
		/// <param name="configSection">The config section to extract <see cref="PiranhaS3StorageOptions"/> from.</param>
		/// <returns>The <see cref="PiranhaS3StorageOptions"/> containing the values set in configuration system.</returns>
		public static PiranhaS3StorageOptions GetS3StorageOptions(this IConfiguration config, string configSection)
		{
			var options = new PiranhaS3StorageOptions();

			// Find the config section
			IConfiguration section;
			if (string.IsNullOrEmpty(configSection))
			{
				section = config;
			}
			else
			{
				section = config.GetSection(configSection);
			}

			// Not found
			if (section == null)
			{
				return options;
			}

			// Extract Settings
			if (!string.IsNullOrEmpty(section["BucketName"]))
			{
				options.BucketName = section["BucketName"];
			}
			if (!string.IsNullOrEmpty(section["KeyPrefix"]))
			{
				options.KeyPrefix = section["KeyPrefix"];
			}
			if (!string.IsNullOrEmpty(section["UrlRoot"]))
			{
				options.UrlRoot = section["UrlRoot"];
			}

			// Return options
			return options;

		}

	}

}
