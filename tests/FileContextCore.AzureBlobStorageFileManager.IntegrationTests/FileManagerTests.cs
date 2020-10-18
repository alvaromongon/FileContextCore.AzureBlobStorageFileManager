using System;
using System.IO;
using Azure.Storage.Blobs;
using FileContextCore.Infrastructure.Internal;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileContextCore.AzureBlobStorageFileManager.IntegrationTests
{
    [TestClass]
    public class FileManagerTests
    {
        private const string _settingsFile = "local.settings.json";
        private static BlobContainerClient _blobContainerClient;
        private static AzureBlobStorageFileManager _sut;
        private EntityType _entityTypeFake;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            var configurationRoot = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile(_settingsFile, optional: false, reloadOnChange: false)
                    .Build();

            var connectionString = configurationRoot.GetValue<string>("AzureStorageConnectionString");
            var containerName = configurationRoot.GetValue<string>("ContainerName");

            _blobContainerClient = new BlobContainerClient(connectionString, containerName);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var tableName = Guid.NewGuid().ToString();
            _entityTypeFake = new EntityType(tableName, new Model(), ConfigurationSource.DataAnnotation);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try
            {
                _sut.Clear();
            }
            catch (Exception) { }
        }

        [TestMethod]
        public void Initialize_ArgumentNullException_WhenNoLocationAndNoDatabaseName()
        {
            // Arrange
            var databaseName = string.Empty;
            var location = string.Empty;
            var fileType = "json";

            IFileContextScopedOptions options = new FileContextScopedOptions(
                databaseName, location, null, null, null, null);

            _sut = new AzureBlobStorageFileManager(_blobContainerClient);

            //Act //Assert
            Assert.ThrowsException<ArgumentNullException>(() => _sut.Initialize(options, _entityTypeFake, fileType));
        }

        [TestMethod]
        public void GetFileName_StartsWithLocation_WhenLocationDefined()
        {
            // Arrange
            var databaseName = string.Empty;
            var location = "SomeLocationForGetFileNameTest";
            var fileType = "json";

            IFileContextScopedOptions options = new FileContextScopedOptions(
                databaseName, location, null, null, null, null);

            _sut = new AzureBlobStorageFileManager(_blobContainerClient);
            _sut.Initialize(options, _entityTypeFake, fileType);

            //Act
            var fileName = _sut.GetFileName();

            // Assert
            fileName.Should().StartWith(location);
        }

        [TestMethod]
        public void GetFileName_StartsWithAppData_AndDatabaseName_WhenLocationIsEmpty()
        {
            // Arrange
            var databaseName = "DatabaseNameForGetFileNameTest";
            var location = string.Empty;
            var fileType = "json";

            IFileContextScopedOptions options = new FileContextScopedOptions(
                databaseName, location, null, null, null, null);

            _sut = new AzureBlobStorageFileManager(_blobContainerClient);
            _sut.Initialize(options, _entityTypeFake, fileType);

            //Act
            var fileName = _sut.GetFileName();

            // Assert
            fileName.Should().StartWith(Path.Combine("appdata", databaseName));
        }

        [TestMethod]
        public void LoadContent_Empty_WhenFileDoesNotExist()
        {
            // Arrange
            var databaseName = string.Empty;
            var location = "SomeLocationForLoadContentTest";
            var fileType = "json";

            IFileContextScopedOptions options = new FileContextScopedOptions(
                databaseName, location, null, null, null, null);

            _sut = new AzureBlobStorageFileManager(_blobContainerClient);
            _sut.Initialize(options, _entityTypeFake, fileType);

            //Act
            var fileName = _sut.LoadContent();

            // Assert
            fileName.Should().BeEmpty();
        }

        [TestMethod]
        public void SaveAndLoadContent_HappyPath_WithLocation()
        {
            // Arrange
            var databaseName = string.Empty;
            var location = "SomeLocationForSaveAndLoadContentTest";
            var fileType = "json";
            var content = "Some content for the file";

            IFileContextScopedOptions options = new FileContextScopedOptions(
                databaseName, location, null, null, null, null);

            _sut = new AzureBlobStorageFileManager(_blobContainerClient);
            _sut.Initialize(options, _entityTypeFake, fileType);

            //Act
            _sut.SaveContent(content);
            var contentLoaded = _sut.LoadContent();

            // Assert
            contentLoaded.Should().BeEquivalentTo(content);
        }

        [TestMethod]
        public void SaveAndLoadContent_HappyPath_WithoutLocation()
        {
            // Arrange
            var databaseName = "SomeDatabaseNameForSaveAndLoadContentTest";
            var location = string.Empty;
            var fileType = "json";
            var content = "Some content for the file";

            IFileContextScopedOptions options = new FileContextScopedOptions(
                databaseName, location, null, null, null, null);

            _sut = new AzureBlobStorageFileManager(_blobContainerClient);
            _sut.Initialize(options, _entityTypeFake, fileType);

            //Act
            _sut.SaveContent(content);
            var contentLoaded = _sut.LoadContent();

            // Assert
            contentLoaded.Should().BeEquivalentTo(content);
        }

        [TestMethod]
        public void Clear_False_WhenFileDoesNotExist()
        {
            // Arrange
            var databaseName = string.Empty;
            var location = "SomeLocationForClearFalseTest";
            var fileType = "json";

            IFileContextScopedOptions options = new FileContextScopedOptions(
                databaseName, location, null, null, null, null);

            _sut = new AzureBlobStorageFileManager(_blobContainerClient);
            _sut.Initialize(options, _entityTypeFake, fileType);

            //Act
            var result = _sut.Clear();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void Clear_True_WhenFileExists()
        {
            // Arrange
            var databaseName = string.Empty;
            var location = "SomeLocationForClearTrueTest";
            var fileType = "json";
            var content = "Some content for the file";

            IFileContextScopedOptions options = new FileContextScopedOptions(
                databaseName, location, null, null, null, null);

            _sut = new AzureBlobStorageFileManager(_blobContainerClient);
            _sut.Initialize(options, _entityTypeFake, fileType);

            //Act
            _sut.SaveContent(content);
            var result = _sut.Clear();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void FileExists_False_WhenFileDoesNotExist()
        {
            // Arrange
            var databaseName = string.Empty;
            var location = "SomeLocationForFileExistsFalseTest";
            var fileType = "json";

            IFileContextScopedOptions options = new FileContextScopedOptions(
                databaseName, location, null, null, null, null);


            _sut = new AzureBlobStorageFileManager(_blobContainerClient);
            _sut.Initialize(options, _entityTypeFake, fileType);

            //Act
            var result = _sut.FileExists();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void FileExists_True_WhenFileExists()
        {
            // Arrange
            var databaseName = string.Empty;
            var location = "SomeLocationForFileExistsTrueTest";
            var fileType = "json";
            var content = "Some content for the file";

            IFileContextScopedOptions options = new FileContextScopedOptions(
                databaseName, location, null, null, null, null);

            _sut = new AzureBlobStorageFileManager(_blobContainerClient);
            _sut.Initialize(options, _entityTypeFake, fileType);

            //Act
            _sut.SaveContent(content);
            var result = _sut.FileExists();

            // Assert
            result.Should().BeTrue();
        }
    }
}
