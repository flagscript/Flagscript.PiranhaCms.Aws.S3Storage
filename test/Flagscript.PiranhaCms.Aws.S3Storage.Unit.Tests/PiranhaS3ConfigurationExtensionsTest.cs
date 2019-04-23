using Xunit;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Unit tests for <see cref="PiranhaS3ConfigurationExtensions"/>.
	/// </summary>
	public class PiranhaS3ConfigurationExtensionsTest : IClassFixture<TestConfigurationFixture>
	{

		/// <summary>
		/// The Test Fixture.
		/// </summary>
		/// <value>The Test Fixture.</value>
		public TestConfigurationFixture TestFixture { get; private set; }

		/// <summary>
		/// Constructor taking test fixture.
		/// </summary>
		/// <param name="testFixture">Test Fixture.</param>
		public PiranhaS3ConfigurationExtensionsTest(TestConfigurationFixture testFixture)
		{
			TestFixture = testFixture;
		}

		/// <summary>
		/// Tests that <see cref="PiranhaS3ConfigurationExtensions.DEFAULT_CONFIG_SECTION"/>
		/// doesn't change unexpectedly.
		/// </summary>
		[Fact]
		public void TestDefaultConfigName()
		{
			Assert.Equal("Flagscript.PiranhaCms.Aws.S3Storage", PiranhaS3ConfigurationExtensions.DEFAULT_CONFIG_SECTION);
		}

		/// <summary>
		/// Unit tests for <see cref="PiranhaS3ConfigurationExtensions.GetS3StorageOptions(Microsoft.Extensions.Configuration.IConfiguration)"/>.
		/// </summary>
		[Fact]
		public void TestGetS3StorageOptions()
		{

			// Valid Config
			var s3StorageOptions = TestFixture.MainConfiguration.GetS3StorageOptions();
            ValidateS3StorageOptions(s3StorageOptions);

			// Root Config
			s3StorageOptions = TestFixture.RootConfiguration.GetS3StorageOptions();
			ValidateNullS3StorageOptions(s3StorageOptions);

			// Other Config
			s3StorageOptions = TestFixture.OtherConfiguration.GetS3StorageOptions();
			ValidateNullS3StorageOptions(s3StorageOptions);

		}

		/// <summary>
		/// Unit tests for <see cref="PiranhaS3ConfigurationExtensions.GetS3StorageOptions(Microsoft.Extensions.Configuration.IConfiguration, string)"/>
		/// when the configuration section name is <c>null</c> or <c>string.Empty</c>.
		/// </summary>
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public void TestGetS3StorageOptionsEmptyName(string emptyConfigName)
		{

			// Valid Named Config
			var s3StorageOptions = TestFixture.MainConfiguration.GetS3StorageOptions(emptyConfigName);
			ValidateNullS3StorageOptions(s3StorageOptions);

			// Root Config
			s3StorageOptions = TestFixture.RootConfiguration.GetS3StorageOptions(emptyConfigName);
			ValidateS3StorageOptions(s3StorageOptions);

		}

		/// <summary>
		/// Unit tests for <see cref="PiranhaS3ConfigurationExtensions.GetS3StorageOptions(Microsoft.Extensions.Configuration.IConfiguration, string)"/>
		/// when the configuration section name is not found.
		/// </summary>
		[Fact]
		public void TestGetS3StorageOptionsUnknownSection()
		{
		
			// Main Config
			var s3StorageOptions = TestFixture.MainConfiguration.GetS3StorageOptions("Not.A.Real.Config.Section");
			ValidateNullS3StorageOptions(s3StorageOptions);

			// Root Config
			s3StorageOptions = TestFixture.RootConfiguration.GetS3StorageOptions("Not.A.Real.Config.Section");
			ValidateNullS3StorageOptions(s3StorageOptions);

			// Other Config
			s3StorageOptions = TestFixture.OtherConfiguration.GetS3StorageOptions("Not.A.Real.Config.Section");
			ValidateNullS3StorageOptions(s3StorageOptions);

		}

		/// <summary>
		/// Unit tests for <see cref="PiranhaS3ConfigurationExtensions.GetS3StorageOptions(Microsoft.Extensions.Configuration.IConfiguration, string)"/>
		/// when the configuration section name is known.
		/// </summary>
		[Fact]
		public void TestGetS3StorageOptionsKnownSection()
		{

			// Main Config
			var s3StorageOptions = TestFixture.MainConfiguration.GetS3StorageOptions("Other.Flagscript.PiranhaCms.Aws.S3Storage");
			ValidateNullS3StorageOptions(s3StorageOptions);

			// Root Config
			s3StorageOptions = TestFixture.RootConfiguration.GetS3StorageOptions("Other.Flagscript.PiranhaCms.Aws.S3Storage");
			ValidateNullS3StorageOptions(s3StorageOptions);

			// Other Config
			s3StorageOptions = TestFixture.OtherConfiguration.GetS3StorageOptions("Other.Flagscript.PiranhaCms.Aws.S3Storage");
			ValidateS3StorageOptions(s3StorageOptions);

		}

        /// <summary>
        /// Validates <see cref="PiranhaS3StorageOptions"/> has the expected test values.
        /// </summary>
        /// <param name="options">The <see cref="S3StorageOptions"/> to test.</param>
        private void ValidateS3StorageOptions(PiranhaS3StorageOptions options)
		{
			Assert.NotNull(options);
			Assert.Equal("BucketNameTest", options.BucketName);
			Assert.Equal("KeyPrefixTest", options.KeyPrefix);
			Assert.Equal("http://flagscript.technology", options.PublicUrlRoot);
		}

        /// <summary>
        /// Validates <see cref="PiranhaS3StorageOptions"/> has the expected test values when
        /// not found.
        /// </summary>
        /// <param name="options">The <see cref="PiranhaS3StorageOptions"/> to test.</param>
        private void ValidateNullS3StorageOptions(PiranhaS3StorageOptions options)
		{
			Assert.NotNull(options);
			Assert.Null(options.BucketName);
			Assert.Null(options.KeyPrefix);
			Assert.Null(options.PublicUrlRoot);
		}

	}

}
