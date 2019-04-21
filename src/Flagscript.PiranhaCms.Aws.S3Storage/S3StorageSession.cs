using System;
using System.IO;
using System.Threading.Tasks;

using Amazon.S3;
using Amazon.S3.Model;
using Flurl;
using Piranha;

namespace Flagscript.PiranhaCms.Aws.S3Storage
{

	/// <summary>
	/// <see cref="IStorageSession"/> provider for AWS. 
	/// </summary>
	/// <remarks>
	/// Interacts with a public S3 bucket to manage files. 
	/// </remarks>
	public class S3StorageSession : IStorageSession
	{

		/// <summary>
		/// Configuration for <see cref="S3Storage"/>.
		/// </summary>
		/// <value>The configuration.</value>
		internal S3StorageConfiguration Configuration { get; set; }

		/// <summary>
		/// The <see cref="IAmazonS3"/> service client to interact with S3.
		/// </summary>
		/// <value>The s3 client.</value>
		internal IAmazonS3 S3Client { get; set; }

		/// <summary>
		/// Creates a new <see cref="S3StorageSession"/> with a specified configuration.
		/// </summary>
		/// <param name="configuration">Configuration for the <see cref="S3StorageSession"/>.</param>
		public S3StorageSession(S3StorageConfiguration configuration)
		{
			Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			if (Configuration.AwsOptions != null)
			{
				S3Client = Configuration.AwsOptions.CreateServiceClient<IAmazonS3>();
			}
			else
			{
				S3Client = new AmazonS3Client();
			}
		}

		/// <summary>
		/// Deletes the content for the specified media.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="id">The unique id</param>
		public async Task<bool> DeleteAsync(string id)
		{
			var objectKey = Url.Combine(Configuration.KeyPrefix, id);
			try
			{
				await S3Client.DeleteObjectAsync(Configuration.BucketName, objectKey);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Disposes the session.
		/// </summary>
		public void Dispose()
		{
			S3Client.Dispose();
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Writes the content for the specified media content to the given stream.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="stream">The output stream</param>
		/// <returns>If the media was found</returns>
		public async Task<bool> GetAsync(string id, Stream stream)
		{
			var objectKey = Url.Combine(Configuration.KeyPrefix, id);

			try
			{
                using (var getObjectResponse = await S3Client.GetObjectAsync(Configuration.BucketName, objectKey))
                using (var responseStream = getObjectResponse.ResponseStream)
                {
                    responseStream.CopyTo(stream);
                }
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Stores the given media content.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="contentType">The content type</param>
		/// <param name="stream">The input stream</param>
		/// <returns>The public URL</returns>
		public async Task<string> PutAsync(string id, string contentType, Stream stream)
		{
			var objectKey = Url.Combine(Configuration.KeyPrefix, id);
			PutObjectRequest putRequest = new PutObjectRequest
			{
				BucketName = Configuration.BucketName,
				Key = objectKey,
				ContentType = contentType,
				InputStream = stream
			};
			await S3Client.PutObjectAsync(putRequest);
			return Url.Combine(Configuration.PublicUrlPrefix, id);
		}

		/// <summary>
		/// Stores the given media content.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="contentType">The content type</param>
		/// <param name="bytes">The binary data</param>
		/// <returns>The public URL</returns>
		public async Task<string> PutAsync(string id, string contentType, byte[] bytes)
		{
			using (var memoryStream = new MemoryStream(bytes))
			{
				return await PutAsync(id, contentType, memoryStream);
			}
		}

	}

}
