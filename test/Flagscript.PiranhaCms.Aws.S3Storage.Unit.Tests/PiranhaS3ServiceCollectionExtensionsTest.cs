using System;

using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Models;
using Xunit;

using static Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests.TestConstants;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Unit tests for <see cref="ServiceCollectionExtensions"/>.
	/// </summary>
	public class PiranhaS3ServiceCollectionExtensionsTest : IClassFixture<TestConfigurationFixture>
	{

		/// <summary>
		/// Valid Storage Options used in tests.
		/// </summary>
		/// <value>Valid Storage Options used in tests.</value>
		private static PiranhaS3StorageOptions ValidStorageOptions => new PiranhaS3StorageOptions
		{
			BucketName = ValidUnitTestBucketName,
			KeyPrefix = ValidUnitTestKeyPrefix,
			PublicUrlRoot = ValidUnitTestUriHost
		};
        

		/// <summary>
		/// The Test Fixture.
		/// </summary>
		/// <value>The Test Fixture.</value>
		public TestConfigurationFixture TestFixture { get; private set; }

		/// <summary>
		/// Constructor taking test fixture.
		/// </summary>
		/// <param name="testFixture">Test Fixture.</param>
		public PiranhaS3ServiceCollectionExtensionsTest(TestConfigurationFixture testFixture)
		{
			TestFixture = testFixture;
		}

		/// <summary>
		/// Ensures <see cref="PiranhaS3ServiceCollectionExtensions.AddPiranhaS3StorageOptions(IServiceCollection, PiranhaS3StorageOptions)"/>
		/// properly registers <see cref="PiranhaS3StorageOptions"/>.
		/// </summary>
		[Fact]
		public void TestAddS3StorageOptions()
		{

			var configuration = TestFixture.MainConfiguration;
			var s3StorageOptions = configuration.GetPiranhaS3StorageOptions();
			var services = new ServiceCollection();
			services.AddPiranhaS3StorageOptions(s3StorageOptions);
			var serviceProvider = services.BuildServiceProvider();
			var returnedOptions = serviceProvider.GetService<PiranhaS3StorageOptions>();
			Assert.Same(s3StorageOptions, returnedOptions);

		}

		/// <summary>
		/// Validates the service will register with the proper interface.
		/// </summary>
		[Fact]
		public void TestAddS3StorageProperInterface()
		{
			var services = new ServiceCollection();

			services.AddPiranhaS3Storage(ValidStorageOptions, TestFixture.FakeAwsOptions);
			IServiceProvider serviceProvider = services.BuildServiceProvider();

			var storageProvider = serviceProvider.GetService<IStorage>();
			Assert.NotNull(storageProvider);
			var testId = Guid.NewGuid();
			var m = new Media() { Id = testId, Filename = "SomeFile" };
			var testUrl = storageProvider.GetPublicUrl(m, "SomeFile");
			Assert.False(string.IsNullOrWhiteSpace(testUrl));
		}

	}

}
