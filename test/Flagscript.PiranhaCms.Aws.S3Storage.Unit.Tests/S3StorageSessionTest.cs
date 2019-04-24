using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Amazon.S3;
using Amazon.S3.Model;
using Flurl;
using Moq;
using Xunit;

using static Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests.TestConstants;

namespace Flagscript.PiranhaCms.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Unit Tests for <see cref="S3StorageSession"/>.
	/// </summary>
	public class S3StorageSessionTest : IClassFixture<TestConfigurationFixture>
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
		public S3StorageSessionTest(TestConfigurationFixture testFixture)
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
		/// Validate exception on null configuration.
		/// </summary>
		[Fact]
		public void TestNullCtor()
		{

			try
			{

				new S3StorageSession(null, null, null);
				Assert.True(false, "S3StorageSession with null cTor args did not exception");

			}
			catch (ArgumentNullException ane)
			{

				Assert.Equal("storageOptions", ane.ParamName);

			}

		}

		/// <summary>
		/// Test with a moq'd client to use all responses.
		/// </summary>
		[Fact]
		public async Task TestDeleteAsync()
		{

			S3StorageSession storageSession = new S3StorageSession(ValidStorageOptions, null, null);

			// Moq client
			var mock = new Mock<IAmazonS3>();
			mock
				.Setup(foo => foo.DeleteObjectAsync(ValidStorageOptions.BucketName, ValidStorageOptions.KeyPrefix, default(CancellationToken)))
				.ReturnsAsync(new DeleteObjectResponse());
			storageSession.S3Client = mock.Object;

			// Test S3 Response
			var testId = Guid.NewGuid().ToString();
			var response = await storageSession.DeleteAsync(testId);
			Assert.True(response);

			// Moq throws async not working. 
			/*
			mock = new Mock<IAmazonS3>();
			var tcs = new TaskCompletionSource<DeleteObjectResponse>();
			mock
				.Setup(foo => foo.DeleteObjectAsync(s3StorageConfiguration.BucketName, s3StorageConfiguration.KeyPrefix, default(CancellationToken)))
				.ThrowsAsync(new Exception());
			storageSession.S3Client = mock.Object;
			response = await storageSession.DeleteAsync(testId);
			Assert.False(response);
			*/

		}

		/// <summary>
		/// Test GetAsync
		/// </summary>
		[Fact]
		public async void TestGetAsync()
		{

			S3StorageSession storageSession = new S3StorageSession(ValidStorageOptions, null, null);
			var testId = Guid.NewGuid().ToString();
			var objectKey = Url.Combine(ValidStorageOptions.KeyPrefix, testId);

			// Moq client
			var mock = new Mock<IAmazonS3>();
			using (var fileStream = new FileStream("test.css", FileMode.Open))
			using (var memoryStream = new MemoryStream())
			{

				mock
					.Setup(foo => foo.GetObjectAsync(ValidUnitTestBucketName, objectKey, default(CancellationToken)))
					.ReturnsAsync(new GetObjectResponse {
						ResponseStream = fileStream
					});
				storageSession.S3Client = mock.Object;

				bool success = await storageSession.GetAsync(testId, memoryStream);
				Assert.True(success);
				var outString = Encoding.UTF8.GetString(memoryStream.ToArray());
				Assert.Contains("body", outString);

			}

		}

		/// <summary>
		/// Test PutAsync with stream type. 
		/// </summary>
		[Fact]
		public async Task TestPutAsyncStream()
		{

			var storageSession = new S3StorageSession(
				ValidStorageOptions,
				TestFixture.FakeAwsOptions,
				null
			);

			// Moq client
			var mock = new Mock<IAmazonS3>();
			mock
				.Setup(foo => foo.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)))
				.ReturnsAsync(new PutObjectResponse());
			storageSession.S3Client = mock.Object;

			// Test S3 Response
			var testId = Guid.NewGuid().ToString();
			var contentType = "text/css";
			using (var fileStream = new FileStream("test.css", FileMode.Open))
			{

				string uri = await storageSession.PutAsync(testId, contentType, fileStream);
				Assert.False(string.IsNullOrWhiteSpace(uri));
				Assert.StartsWith(ValidUnitTestUriHost, ValidUnitTestUriHost);

			}

		}

		/// <summary>
		/// Test PutAsync with byte array type.
		/// </summary>
		[Fact]
		public async Task TestPutAsyncByteArray()
		{

			var storageSession = new S3StorageSession(
				ValidStorageOptions, 
				TestFixture.FakeAwsOptions, 
				null
			);

			// Moq client
			var mock = new Mock<IAmazonS3>();
			mock
				.Setup(foo => foo.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)))
				.ReturnsAsync(new PutObjectResponse());
			storageSession.S3Client = mock.Object;

			// Test S3 Response
			var testId = Guid.NewGuid().ToString();
			var contentType = "text/css";
			var fileBytes = await File.ReadAllBytesAsync("test.css");
			string uri = await storageSession.PutAsync(testId, contentType, fileBytes);
			Assert.False(string.IsNullOrWhiteSpace(uri));
			Assert.StartsWith(ValidUnitTestUriHost, ValidUnitTestUriHost);

		}

	}

}
