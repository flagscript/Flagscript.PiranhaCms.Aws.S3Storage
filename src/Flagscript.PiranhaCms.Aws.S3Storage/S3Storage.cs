using System;
using System.Threading.Tasks;

using Flurl;
using Piranha;

namespace Flagscript.PiranhaCms.Aws.S3Storage
{

	/// <summary>
	/// <see cref="IStorage"/> provider for AWS. 
	/// </summary>
	/// <remarks>
	/// Can be used with S3 Bucket Websites, CloudFront distributions fronting 
	/// S3, which can be behind a Route53 domain.
	/// </remarks>
	public class S3Storage : IStorage
	{

		/// <summary>
		/// Configuration for <see cref="S3Storage"/>.
		/// </summary>
		internal readonly S3StorageConfiguration _configuration;

		public S3Storage(S3StorageConfiguration configuration)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		/// <summary>
		/// Opens a new storage session.
		/// </summary>
		/// <returns>A new open session</returns>
		public Task<IStorageSession> OpenAsync()
		{
			return Task.FromResult<IStorageSession>(new S3StorageSession(_configuration));
		}

		/// <summary>
		/// Gets the public URL for the given media object.
		/// </summary>
		/// <param name="id">The media resource id</param>
		/// <returns>The public URL.</returns>
		public string GetPublicUrl(string id)
		{
			if (!string.IsNullOrWhiteSpace(id))
			{
				return Url.Combine(_configuration.PublicUrlPrefix, id);
			}
			return null;
		}

	}

}