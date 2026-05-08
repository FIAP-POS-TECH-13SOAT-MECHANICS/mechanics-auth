using Mechanics.Auth.Application.Consumers;
using Mechanics.Auth.Infra.Data.Repositories;
using Mechanics.Auth.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using UserChangedEvent = Mechanics.Auth.Application.Consumers.UserChangedEvent;

namespace Mechanics.Auth.Tests.Integration.Tests;

[TestClass]
[TestCategory("Consumer")]
public class UserChangedEventConsumerTests
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public async Task Save_WithValidEvent_ShouldPersistNewUser()
    {
        // Arrange
        var userId = new Guid("7f4cfa00-2737-4915-b8aa-46c1097b3231");
        var message = new UserChangedEvent
        {
            Id = userId.ToString(),
            CpfNumber = "91798857057",
            FullName = "NEW CUSTOMER",
            Role = "CUSTOMER_USER",
            SecurityStamp = "567092b9-03ea-4653-8385-9032a919e64e",
            PasswordHash = "AQAAAAIAAYagAAAAEKSeHdHtCfN38pakeil4oyEL0d07GBEySe6csY8jmXIKT3oEZVcZR7Jngd9qxFgmkQ==",
            CustomerId = "93c31695-f562-4316-a6cb-8ebe984f78ec",
            LastUpdate = new DateTimeOffset(2026, 5, 8, 12, 0, 0, TimeSpan.Zero),
        };
        using var scopedProvider = TestProperties.Factory.Services.CreateScope();
        var repository = scopedProvider.ServiceProvider.GetRequiredService<IUserRepository>();
        var consumer = new UserChangedEventConsumer(repository);

        await consumer.Save(message, TestContext.CancellationTokenSource.Token);

        // Assert
        var persisted = await repository.GetById(userId, TestContext.CancellationTokenSource.Token);
        Assert.IsNotNull(persisted);
        Assert.AreEqual(userId, persisted.Id);
        Assert.AreEqual("91798857057", persisted.CpfNumber);
    }

    [TestMethod]
    public async Task Save_WithNewerEvent_ShouldUpdateRecord()
    {
        var userId = new Guid("61b33bf9-6526-4fca-afa4-7c0cacc5fc8e");
        var message = new UserChangedEvent
        {
            Id = userId.ToString(),
            CpfNumber = "91977509053",
            FullName = "CUSTOMER ADMIN",
            Role = "CUSTOMER_ADMIN",
            SecurityStamp = "4d2366c1-abaa-4e73-8856-80cfb8a3011f",
            PasswordHash = "new-hash",
            CustomerId = "ef82dd09-ef58-4d35-84ae-4f82b9bf038d",
            LastUpdate = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero),
        };
        using var scopedProvider = TestProperties.Factory.Services.CreateScope();
        var repository = scopedProvider.ServiceProvider.GetRequiredService<IUserRepository>();
        var consumer = new UserChangedEventConsumer(repository);

        await consumer.Save(message, TestContext.CancellationTokenSource.Token);

        var persisted = await repository.GetById(userId, TestContext.CancellationTokenSource.Token);
        Assert.IsNotNull(persisted);
        Assert.AreEqual("new-hash", persisted.PasswordHash);
        Assert.AreEqual("4d2366c1-abaa-4e73-8856-80cfb8a3011f", persisted.SecurityStamp);
        Assert.AreEqual(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc), persisted.LastUpdate);
    }
}
