namespace Flagscript.Piranha.Aws.S3Storage.Unit.Tests
{

	/// <summary>
	/// Constants used in various tests.
	/// </summary>
	internal static class TestConstants
	{

		#region Const / Static

		/// <summary>
		/// Valid bucket name to be used in <see cref="S3StorageConfiguration"/> for
		/// positive unit testing.
		/// </summary>
		internal const string ValidUnitTestBucketName = "UnitTestBucket";

		/// <summary>
		/// Valid key prefix to be used in <see cref="S3StorageConfiguration"/> for
		/// positive unit testing.
		/// </summary>
		internal const string ValidUnitTestKeyPrefix = "flagscripttech/prod/uploads";

		/// <summary>
		/// Valid URI host to be used in <see cref="S3StorageConfiguration"/> for 
		/// positive unit testing.
		/// </summary>
		internal const string ValidUnitTestUriHost = "https://flagscript.technology";

		#endregion

	}

}
