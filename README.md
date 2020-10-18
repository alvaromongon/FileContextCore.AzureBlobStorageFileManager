[![Build Status](https://wtwd.visualstudio.com/Ease%20Maker/_apis/build/status/alvaromongon.FileContextCore.AzureBlobStorageFileManager?branchName=main)](https://wtwd.visualstudio.com/Ease%20Maker/_build/latest?definitionId=7&branchName=main)

# Introduction 
Azure blob storage implementation of the IFileManager interface defined in the FileContextCore package.
It allows to have an Entity Framework Core provider which does not need a database, just a blob storage. 
Very useful for lower environments where you do not want to have a full database running.

The FileContextCore package is listed as one of the "official" providers for Entity Framework Core:
[Entity Framerowkr Core Database providers](https://docs.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli)

[File context core package](https://www.nuget.org/packages/FileContextCore/)
[File context core source code](https://github.com/morrisjdev/FileContextCore)

# Getting Started
As @morrisjdev comments, "After adding a custom provider you have to add it as a transient dependency in the dependency injection".
Once the provider is registered in the dependencies container, you can use it as you would with any other IFileManager implementation.

In particular, this implementation constructor just need a valid BlobContainerClient class instance to work.

Keep in mind that FileContextCore is not recomended in production environments and therefore the same applies to this implementation.

# Build and Test
Building is easy since there it not special dependecies, 
but in order to run the tests you have to manually launch the Microsoft Azure Storage Emulator.

Once you have the storage emulator up and running, just run the tests.
