using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Amazon.S3.Model;
using Flurl;
using Flurl.Http;
using Piranha;
using Xunit;

namespace Flagscript.Piranha.Aws.S3Storage.Integration.Tests
{

	/// <summary>
	/// Integration Tests with AWS for the Flagscript.Piranha.Aws.S3Storage
	/// package.
	/// </summary>
	public class BucketIntegration : IClassFixture<BucketIntegrationFixture>
    {

		/// <summary>
		/// The Test Fixture.
		/// </summary>
		/// <value>The Test Fixture.</value>
		public BucketIntegrationFixture TestFixture { get; private set; }

		/// <summary>
		/// Constructor taking test fixture.
		/// </summary>
		/// <param name="testFixture">Test Fixture.</param>
		public BucketIntegration(BucketIntegrationFixture testFixture)
		{
			TestFixture = testFixture;
		}

		/// <summary>
		/// Integration test for <see cref="IStorageSession.DeleteAsync(string)"/>.
		/// </summary>
		[Fact]
		public async Task TestDeleteAsync()
		{

			// Create an object with the S3 Client
			var objectId = Guid.NewGuid().ToString();
			var objectKey = $"uploads/{objectId}";
			using (var stringStream = new MemoryStream(Encoding.UTF8.GetBytes("hi")))
			{
				PutObjectRequest putRequest = new PutObjectRequest
				{
					BucketName = TestFixture.BucketName,
					Key = objectKey,
					ContentType = "text/plain",
					InputStream = stringStream
				};
				await TestFixture.S3Client.PutObjectAsync(putRequest);
			}

			// Delete with IStorage
			using (var storageSession = await TestFixture.S3Storage.OpenAsync())
			{
				await storageSession.DeleteAsync(objectId);
			}

			// See if it deleted
			try
			{
				var objectPath = Url.Combine(TestFixture.BucketWebsite, objectKey);
				await objectPath.GetAsync();
				Assert.False(true, $"storageSession.DeleteAsync did not remove {objectPath}");
			}
			catch (FlurlHttpException fhe)
			{
				Assert.Equal(HttpStatusCode.NotFound, fhe.Call.HttpStatus);
			}

		}

		/// <summary>
		/// Integration test for <see cref="IStorageSession.GetAsync(string, Stream)"/>;
		/// </summary>
		[Fact]
		public async Task TestGetAsync()
		{

			// Create an object with the S3 Client
			var objectId = Guid.NewGuid().ToString();
			var objectKey = $"uploads/{objectId}";
			using (var stringStream = new MemoryStream(Encoding.UTF8.GetBytes("hi")))
			{
				PutObjectRequest putRequest = new PutObjectRequest
				{
					BucketName = TestFixture.BucketName,
					Key = objectKey,
					ContentType = "text/plain",
					InputStream = stringStream
				};
				await TestFixture.S3Client.PutObjectAsync(putRequest);
			}

			// Test Get to a memory stream.
			using (var storageSession = await TestFixture.S3Storage.OpenAsync())
			using (var memoryStream = new MemoryStream())
			{
				bool success = await storageSession.GetAsync(objectId, memoryStream);
				Assert.True(success);
				Assert.Equal("hi", Encoding.ASCII.GetString(memoryStream.ToArray()));
			}

			// Try to delete it. If not - no biggie - int bucket lifecycle will.
			await DeleteObject(objectId);

		}

		/// <summary>
		/// Integration test for <see cref="IStorageSession.PutAsync(string, string, byte[])"/>
		/// </summary>
        [Fact]
        public async Task TestPutAsyncBytes()
        {

			string putUrl;
			var objectId = Guid.NewGuid().ToString();

			// Put using the S3 IStorage
			using (var storageSession = await TestFixture.S3Storage.OpenAsync())
			{
				putUrl = await storageSession.PutAsync(objectId, "text/plain", Encoding.ASCII.GetBytes("hi"));
			}

			// Test existance and equality
			string text = await putUrl.GetStringAsync();
			Assert.Equal("hi", text);

			// Try to delete it. If not - no biggie - int bucket lifecycle will.
			await DeleteObject(objectId);

		}

		/// <summary>
		/// Integration test for <see cref="IStorageSession.PutAsync(string, string, byte[])"/>
		/// </summary>
		[Fact]
		public async Task TestPutAsyncStream()
		{

			string putUrl;
			var objectId = Guid.NewGuid().ToString();

			// Put using the S3 IStorage
			using (var storageSession = await TestFixture.S3Storage.OpenAsync())
			using (var stringStream = new MemoryStream(Encoding.UTF8.GetBytes("hi")))
			{
				putUrl = await storageSession.PutAsync(objectId, "text/plain", stringStream);
			}

			// Test existance and equality
			string text = await putUrl.GetStringAsync();
			Assert.Equal("hi", text);

			// Try to delete it. If not - no biggie - int bucket lifecycle will.
			await DeleteObject(objectId);

		}

		/// <summary>
		/// Helper method to cleanup objects with the amazon s3 client.
		/// </summary>
		/// <param name="id">Identifier.</param>
		private async Task DeleteObject(string id)
		{

			DeleteObjectRequest delRequest = new DeleteObjectRequest
			{
				BucketName = TestFixture.BucketName,
				Key = $"uploads/{id}"
			};

			try
			{
				await TestFixture.S3Client.DeleteObjectAsync(delRequest);
			}
			catch
			{
				Console.WriteLine($"Failed to delete integration object {delRequest.Key}");
			}

		}

    }

}
