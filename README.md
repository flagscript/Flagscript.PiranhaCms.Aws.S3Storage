# Flagscript.PiranhaCms.Aws.S3Storage

S3/Cloudfront Media Provider for Piranha CMS.

Version | Status
--- | ---
Latest | [![last commit](https://img.shields.io/github/last-commit/flagscript/Flagscript.PiranhaCms.Aws.S3Storage.svg?logo=github)](https://github.com/flagscript/Flagscript.PiranhaCms.Aws.S3Storage) [![build status](https://img.shields.io/appveyor/ci/Flagscript/flagscript-piranhacms-aws-s3storage.svg?logo=appveyor)](https://ci.appveyor.com/project/Flagscript/flagscript-piranhacms-aws-s3storage) [![unit test](https://img.shields.io/appveyor/tests/Flagscript/flagscript-piranhacms-aws-s3storage.svg?label=unit%20tests&logo=appveyor)](https://ci.appveyor.com/project/Flagscript/flagscript-piranhacms-aws-s3storage)
Master | [![last commit](https://img.shields.io/github/last-commit/flagscript/Flagscript.PiranhaCms.Aws.S3Storage/master.svg?logo=github)](https://github.com/flagscript/Flagscript.PiranhaCms.Aws.S3Storage) [![build status](https://img.shields.io/appveyor/ci/Flagscript/flagscript-piranhacms-aws-s3storage/master.svg?logo=appveyor)](https://ci.appveyor.com/project/Flagscript/flagscript-piranhacms-aws-s3storage) [![unit and integration test](https://img.shields.io/appveyor/tests/Flagscript/flagscript-piranhacms-aws-s3storage/master.svg?label=unit/integration%20tests&logo=appveyor)](https://ci.appveyor.com/project/Flagscript/flagscript-piranhacms-aws-s3storage) [![CodeFactor](https://www.codefactor.io/repository/github/flagscript/flagscript.piranhacms.aws.s3storage/badge)](https://www.codefactor.io/repository/github/flagscript/flagscript.piranhacms.aws.s3storage) [![LGTM Total Alerts](https://img.shields.io/lgtm/alerts/g/flagscript/Flagscript.PiranhaCms.Aws.S3Storage.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/flagscript/Flagscript.PiranhaCms.Aws.S3Storage/alerts/)
Pre-Release | [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Flagscript.PiranhaCms.Aws.S3Storage.svg?logo=nuget)](https://www.nuget.org/packages/Flagscript.PiranhaCms.Aws.S3Storage)
Release | [![Nuget (with prereleases)](https://img.shields.io/nuget/v/Flagscript.PiranhaCms.Aws.S3Storage.svg?logo=nuget)](https://www.nuget.org/packages/Flagscript.PiranhaCms.Aws.S3Storage)

## Simple Usage

The S3 File Storage for Piranha CMS operates much the same as [Local File Storage](http://piranhacms.org/docs/components/media-storage/local-file-storage) or [Azure Blob Storage](http://piranhacms.org/docs/components/media-storage/azure-blob-storage). You register S3 File Storage with the default configuration in `ConfigureServices()` with the following code:

```csharp
var storageConfig = new S3StorageConfiguration("my-bucket-name");
services.AddS3Storage(storageConfig);
```

## Documentation

[Documentation](./documentation/DOCUMENTATION.md) on how to use the Flagscript.Piranha.Aws.S3Storage library is available within this repository. 

## Download

Flagscript.PiranhaCms.Aws.S3Storage is available as a NuGet package:

### .NET CLI

```bash
> dotnet add package Flagscript.PiranhaCms.Aws.S3Storage --version 1.0.0-alpha
```

###  .csproj

```xml
<PackageReference Include="Flagscript.PiranhaCms.Aws.S3Storage" Version="1.0.0-alpha" />
```

## Contributing

Although contributions for this project are not yet open, please read 
[CONTRIBUTING](https://github.com/flagscript/Flagscript.Piranha.Aws.S3Storage/blob/master/CONTRIBUTING.md) 
for details on our code of conduct.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see 
the [tags on this repository](https://github.com/flagscript/Flagscript.PiranhaCms.Aws.S3Storage/releases). 

## Authors

* **Greg Kaestle** - [Flagscript](https://flagscript.net)

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/flagscript/Flagscript.PiranhaCms.Aws.S3Storage/blob/master/LICENSE.md) file for details.
