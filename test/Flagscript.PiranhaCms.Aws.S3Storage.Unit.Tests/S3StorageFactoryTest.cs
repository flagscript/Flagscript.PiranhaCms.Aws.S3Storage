using System;

using Microsoft.Extensions.DependencyInjection;

using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Xunit;

using static Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests.TestConstants;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Unit Tests for <see cref="S3StorageFactory"/>.
	/// </summary>
	public class S3StorageFactoryTest : IClassFixture<TestConfigurationFixture>
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
        public S3StorageFactoryTest(TestConfigurationFixture testFixture)
        {
            TestFixture = testFixture;
        }

        /// <summary>
        /// Valid Storage Options used in tests.
        /// </summary>
        /// <value>Valid Storage Options used in tests.</value>
        private static PiranhaS3StorageOptions ValidStorageOptions => new PiranhaS3StorageOptions
        {
            BucketName = ValidUnitTestBucketName,
            KeyPrefix = "uploads",
            PublicUrlRoot = ValidUnitTestUriHost
        };

        /// <summary>
        /// Validate the factory works if S3Configuration and AWS Options are passed in.
        /// </summary>
        [Fact]
		public void TestGetStorageBothPassedInOptions()
		{

			IServiceCollection services = new ServiceCollection();
			var storageFactory = new S3StorageFactory(ValidStorageOptions, TestFixture.FakeAwsOptions);
            Func<IServiceProvider, object> factory = storageFactory.CreateS3Storage;
			services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
			var serviceProvider = services.BuildServiceProvider();

			var s3Storage = serviceProvider.GetService<S3Storage>();
			Assert.Equal(ValidStorageOptions.BucketName, s3Storage.StorageOptions.BucketName);
            Assert.Equal(ValidStorageOptions.KeyPrefix, s3Storage.StorageOptions.KeyPrefix);
            Assert.Equal(ValidStorageOptions.PublicUrlRoot, s3Storage.StorageOptions.PublicUrlRoot);
            Assert.Same(TestFixture.FakeAwsOptions, s3Storage.AwsOptions);
        }

        /// <summary>
        /// Validate the factory works if AWS Options are in the service chain.
        /// </summary>
        [Fact]
        public void TestGetStorageServiceAwsOptions()
        {

            IServiceCollection services = new ServiceCollection();
            var storageFactory = new S3StorageFactory(ValidStorageOptions, null);
            services.AddDefaultAWSOptions(TestFixture.FakeAwsOptions);
            Func<IServiceProvider, object> factory = storageFactory.CreateS3Storage;
            services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
            var serviceProvider = services.BuildServiceProvider();

            var s3Storage = serviceProvider.GetService<S3Storage>();
            Assert.Equal(ValidStorageOptions.BucketName, s3Storage.StorageOptions.BucketName);
            Assert.Equal(ValidStorageOptions.KeyPrefix, s3Storage.StorageOptions.KeyPrefix);
            Assert.Equal(ValidStorageOptions.PublicUrlRoot, s3Storage.StorageOptions.PublicUrlRoot);
            Assert.Same(TestFixture.FakeAwsOptions, s3Storage.AwsOptions);
        }

        /// <summary>
        /// Validate the factory works if Storage Options are int the service chain.
        /// </summary>
        [Fact]
        public void TestGetStorageServiceStorageOptions()
        {

            IServiceCollection services = new ServiceCollection();
            services.AddPiranhaS3StorageOptions(ValidStorageOptions);
            var storageFactory = new S3StorageFactory(null, TestFixture.FakeAwsOptions);
            Func<IServiceProvider, object> factory = storageFactory.CreateS3Storage;
            services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
            var serviceProvider = services.BuildServiceProvider();

            var s3Storage = serviceProvider.GetService<S3Storage>();
            Assert.Equal(ValidStorageOptions.BucketName, s3Storage.StorageOptions.BucketName);
            Assert.Equal(ValidStorageOptions.KeyPrefix, s3Storage.StorageOptions.KeyPrefix);
            Assert.Equal(ValidStorageOptions.PublicUrlRoot, s3Storage.StorageOptions.PublicUrlRoot);

        }

        /// <summary>
        /// Validate the factory works if AWS Options in configuration
        /// </summary>
        [Fact]
        public void TestGetStorageConfigurationAwsOptions()
        {

            IServiceCollection services = new ServiceCollection();            
            var storageFactory = new S3StorageFactory(ValidStorageOptions, null);
            services.AddSingleton(TestFixture.MainConfiguration);
            Func<IServiceProvider, object> factory = storageFactory.CreateS3Storage;
            services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
            var serviceProvider = services.BuildServiceProvider();

            var s3Storage = serviceProvider.GetService<S3Storage>();
            Assert.Equal("unit-test-profile", s3Storage.AwsOptions.Profile);
        }

        /// <summary>
        /// Validate the factory works if Storage Options in configuration
        /// </summary>
        [Fact]
        public void TestGetStorageConfigurationStorageOptions()
        {

            IServiceCollection services = new ServiceCollection();
            var storageFactory = new S3StorageFactory(null, TestFixture.FakeAwsOptions);
            services.AddSingleton(TestFixture.MainConfiguration);
            Func<IServiceProvider, object> factory = storageFactory.CreateS3Storage;
            services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
            var serviceProvider = services.BuildServiceProvider();

            var s3Storage = serviceProvider.GetService<S3Storage>();
            Assert.Equal("BucketNameTest", s3Storage.StorageOptions.BucketName);
        }

        /// <summary>
        /// Validate the factory works if AWS Options in configuration
        /// </summary>
        [Fact]
        public void TestGetStorageConfigurationEnvVarOptions()
        {
            try
            {

                Environment.SetEnvironmentVariable(PiranhaS3StorageOptions.BucketEnvironmentVariable, "buck");
                Environment.SetEnvironmentVariable(PiranhaS3StorageOptions.KeyPrefixEnvironmentVariable, "kp");
                Environment.SetEnvironmentVariable(PiranhaS3StorageOptions.UrlRootEnvironmentVariable, "http://www.deckersbrands.com");

                IServiceCollection services = new ServiceCollection();
                var storageFactory = new S3StorageFactory(null, TestFixture.FakeAwsOptions);
                Func<IServiceProvider, object> factory = storageFactory.CreateS3Storage;
                services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
                var serviceProvider = services.BuildServiceProvider();

                var s3Storage = serviceProvider.GetService<S3Storage>();
                Assert.Equal("buck", s3Storage.StorageOptions.BucketName);
                Assert.Equal("kp", s3Storage.StorageOptions.KeyPrefix);
                Assert.Equal("http://www.deckersbrands.com", s3Storage.StorageOptions.PublicUrlRoot);

            }
            finally
            {
                Environment.SetEnvironmentVariable(PiranhaS3StorageOptions.BucketEnvironmentVariable, null);
                Environment.SetEnvironmentVariable(PiranhaS3StorageOptions.KeyPrefixEnvironmentVariable, null);
                Environment.SetEnvironmentVariable(PiranhaS3StorageOptions.UrlRootEnvironmentVariable, null);
            }

        }

        /// <summary>
        /// Validate the key prefix default fallback.
        /// </summary>
        [Fact]
        public void TestGetStorageConfigurationKeyPrefixFallback()
        {
            try
            {

                Environment.SetEnvironmentVariable(PiranhaS3StorageOptions.BucketEnvironmentVariable, "buck");
                Environment.SetEnvironmentVariable(PiranhaS3StorageOptions.UrlRootEnvironmentVariable, "http://www.deckersbrands.com");

                IServiceCollection services = new ServiceCollection();
                var storageFactory = new S3StorageFactory(null, TestFixture.FakeAwsOptions);
                Func<IServiceProvider, object> factory = storageFactory.CreateS3Storage;
                services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
                var serviceProvider = services.BuildServiceProvider();

                var s3Storage = serviceProvider.GetService<S3Storage>();
                Assert.Equal("buck", s3Storage.StorageOptions.BucketName);
                Assert.Equal("uploads", s3Storage.StorageOptions.KeyPrefix);
                Assert.Equal("http://www.deckersbrands.com", s3Storage.StorageOptions.PublicUrlRoot);

            }
            finally
            {
                Environment.SetEnvironmentVariable(PiranhaS3StorageOptions.BucketEnvironmentVariable, null);
                Environment.SetEnvironmentVariable(PiranhaS3StorageOptions.UrlRootEnvironmentVariable, null);
            }

        }

        /// <summary>
        /// Validate exception if no bucket is set.
        /// </summary>
        [Fact]
        public void TestGetStorageConfigurationNoBucket()
        {

            var noBucketStorageOptions = new PiranhaS3StorageOptions
            {
                KeyPrefix = ValidUnitTestKeyPrefix,
                PublicUrlRoot = ValidUnitTestUriHost
            };

            try
            {

                IServiceCollection services = new ServiceCollection();
                var storageFactory = new S3StorageFactory(noBucketStorageOptions, TestFixture.FakeAwsOptions);
                Func<IServiceProvider, object> factory = storageFactory.CreateS3Storage;
                services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
                var serviceProvider = services.BuildServiceProvider();

                var s3Storage = serviceProvider.GetService<S3Storage>();
                Assert.False(true, "No BucketName did not throw an exception");

            }
            catch (Exception ex)
            {
                Assert.IsAssignableFrom<FlagscriptException>(ex);
            }

        }

        /// <summary>
        /// Validate exception if no public url root
        /// </summary>
        [Fact]
        public void TestGetStorageConfigurationNoPublicUrlRoot()
        {

            var noBucketStorageOptions = new PiranhaS3StorageOptions
            {
                BucketName = ValidUnitTestBucketName,
                KeyPrefix = ValidUnitTestKeyPrefix
            };

            try
            {

                IServiceCollection services = new ServiceCollection();
                var storageFactory = new S3StorageFactory(noBucketStorageOptions, TestFixture.FakeAwsOptions);
                Func<IServiceProvider, object> factory = storageFactory.CreateS3Storage;
                services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
                var serviceProvider = services.BuildServiceProvider();

                var s3Storage = serviceProvider.GetService<S3Storage>();
                Assert.False(true, "No PublicUrlRoot did not throw an exception");

            }
            catch (Exception ex)
            {
                Assert.IsAssignableFrom<FlagscriptException>(ex);
            }

        }

        /// <summary>
        /// Validate exception if bad public url root
        /// </summary>
        [Fact]
        public void TestGetStorageConfigurationBadPublicUrlRoot()
        {

            var noBucketStorageOptions = new PiranhaS3StorageOptions
            {
                BucketName = ValidUnitTestBucketName,
                KeyPrefix = ValidUnitTestKeyPrefix,
                PublicUrlRoot = "notaurl"
            };

            try
            {

                IServiceCollection services = new ServiceCollection();
                var storageFactory = new S3StorageFactory(noBucketStorageOptions, TestFixture.FakeAwsOptions);
                Func<IServiceProvider, object> factory = storageFactory.CreateS3Storage;
                services.Add(new ServiceDescriptor(typeof(S3Storage), factory, ServiceLifetime.Singleton));
                var serviceProvider = services.BuildServiceProvider();

                serviceProvider.GetService<S3Storage>();
                Assert.False(true, "Bad PublicUrlRoot did not throw an exception");

            }
            catch (Exception ex)
            {
                Assert.IsAssignableFrom<FlagscriptException>(ex);
            }

        }

    }

}
