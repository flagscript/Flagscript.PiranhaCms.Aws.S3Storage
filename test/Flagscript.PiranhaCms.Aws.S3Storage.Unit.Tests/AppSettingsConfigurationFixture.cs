using Microsoft.Extensions.Configuration;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Fixture containing the various appsettings configurations.
	/// </summary>
	public class AppSettingsConfigurationFixture
	{

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
		/// Contructor initializing test fixture.
		/// </summary>
		public AppSettingsConfigurationFixture()
		{

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

		}

	}

}
