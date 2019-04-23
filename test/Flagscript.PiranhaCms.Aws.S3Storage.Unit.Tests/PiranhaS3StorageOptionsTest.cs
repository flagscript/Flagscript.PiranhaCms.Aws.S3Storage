using Xunit;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests
{

    /// <summary>
    /// Unit tests for <see cref="PiranhaS3StorageOptions"/>.
    /// </summary>
    public class PiranhaS3StorageOptionsTest
    {

        /// <summary>
        /// Tests that <see cref="PiranhaS3StorageOptions"/> environment and configuration constants do not change unexpectedly.
        /// </summary>
        [Fact]
        public void TestConstants()
        {
            Assert.Equal("PIRANHA_S3_BUCKET_NAME", PiranhaS3StorageOptions.BucketEnvironmentVariable);
            Assert.Equal("PIRANHA_S3_KEY_PREFIX", PiranhaS3StorageOptions.KeyPrefixEnvironmentVariable);
            Assert.Equal("PIRANHA_S3_URL_ROOT", PiranhaS3StorageOptions.UrlRootEnvironmentVariable);
            Assert.Equal("uploads", PiranhaS3StorageOptions.KeyPrefixDefault);
        }

        /// <summary>
        /// Validates <see cref="PiranhaS3StorageOptions.PublicUrlPrefix"/> correctly
        /// combines URLs. 
        /// </summary>
        [Fact]
        public void TestPublicUrlPrefix()
        {
            var storageOptions = new PiranhaS3StorageOptions
            {
                PublicUrlRoot = "https://deckers.com",
                KeyPrefix = "uploads"
            };
            Assert.Equal("https://deckers.com/uploads", storageOptions.PublicUrlPrefix);
        }

    }
}
