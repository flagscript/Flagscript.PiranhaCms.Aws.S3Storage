using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Amazon.Extensions.NETCore.Setup;
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
		/// Configuration options for <see cref="S3Storage"/>.
		/// </summary>
		/// <value>Configuration options for <see cref="S3Storage"/>.</value>
		internal PiranhaS3StorageOptions StorageOptions { get; private set; }

		/// <summary>
		/// The <see cref="IAmazonS3"/> service client to interact with S3.
		/// </summary>
		/// <value>The s3 client.</value>
		internal IAmazonS3 S3Client { get; set; }

		/// <summary>
		/// Namespace <see cref="ILogger"/> used for logging.
		/// </summary>
		/// <value>Namespace <see cref="ILogger"/> used for logging.</value>
		internal ILogger Logger { get; private set; }

		/// <summary>
		/// Creates a new <see cref="S3StorageSession"/> with a specified configuration.
		/// </summary>
		/// <param name="storageOptions"><see cref="PiranhaS3StorageOptions"/> used to configure the Piranda S3 storage.</param>
		/// <param name="awsOptions">The <see cref="AWSOptions"/> used to create the S3 service client.</param>
		/// <param name="logger">Namespace <see cref="ILogger"/> used for logging.</param>
		public S3StorageSession(PiranhaS3StorageOptions storageOptions, AWSOptions awsOptions, ILogger logger)
		{
			StorageOptions = storageOptions ?? throw new ArgumentNullException(nameof(storageOptions));
			if (awsOptions != null)
			{
				S3Client = awsOptions.CreateServiceClient<IAmazonS3>();
			}
			else
			{
				S3Client = new AmazonS3Client();
			}
			Logger = logger;
		}

		/// <summary>
		/// Deletes the content for the specified media.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="id">The unique id</param>
		public async Task<bool> DeleteAsync(string id)
		{
			var objectKey = Url.Combine(StorageOptions.KeyPrefix, id);
			try
			{
				await S3Client.DeleteObjectAsync(StorageOptions.BucketName, objectKey);
				Logger?.LogInformation($"Successfully deleted Piranha S3 Media {id}");
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
			Logger?.LogDebug("Closing Piranha S3 media storage session");
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
			var objectKey = Url.Combine(StorageOptions.KeyPrefix, id);

			try
			{
				using (var getObjectResponse = await S3Client.GetObjectAsync(StorageOptions.BucketName, objectKey))
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
			var objectKey = Url.Combine(StorageOptions.KeyPrefix, id);
			PutObjectRequest putRequest = new PutObjectRequest
			{
				BucketName = StorageOptions.BucketName,
				Key = objectKey,
				ContentType = contentType,
				InputStream = stream
			};
			await S3Client.PutObjectAsync(putRequest);            
			var mediaUrl = Url.Combine(StorageOptions.PublicUrlPrefix, id);
			Logger?.LogInformation($"Successfully added Piranha S3 Media {id}, Content Type {contentType} to public URL {mediaUrl}");
			return mediaUrl;
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
				return await PutAsync(id, contentType, memoryStream).ConfigureAwait(false);
			}
		}

	}

}
