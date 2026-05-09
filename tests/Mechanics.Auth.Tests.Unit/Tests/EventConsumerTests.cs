using Amazon.DynamoDBv2.Model;
using Mechanics.Auth.Application.Consumers;
using Mechanics.Auth.Infra.Data.Models;
using Mechanics.Auth.Infra.Data.Repositories;
using Moq;

namespace Mechanics.Auth.Tests.Unit.Tests;

[TestClass]
[TestCategory("Consumer")]
public class EventConsumerTests
{
    public TestContext TestContext { get; set; }

    [TestMethod("Salvar usuário deve chamar Upsert no repositório")]
    public async Task It_ShouldCallUpsert_WhenSavingUser()
    {
        var repositoryStub = new Mock<IUserRepository>();
        var consumer = new UserChangedEventConsumer(repositoryStub.Object);
        var message = new UserChangedEvent
        {
            Id = Guid.NewGuid().ToString(),
            CpfNumber = "12345678909",
            FullName = "Customer User",
            Role = "CUSTOMER_USER",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "hash",
            LastUpdate = DateTimeOffset.UtcNow,
        };

        await consumer.Save(message, TestContext.CancellationTokenSource.Token);

        repositoryStub.Verify(repository => repository.Upsert(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod("Salvar usuário desatualizado deve lançar ConditionalCheckFailedException")]
    public async Task It_ShouldIgnore_WhenRecordIsNewer()
    {
        var repositoryStub = new Mock<IUserRepository>();
        repositoryStub
            .Setup(repository => repository.Upsert(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConditionalCheckFailedException("Conditional check failed"));
        var consumer = new UserChangedEventConsumer(repositoryStub.Object);
        var message = new UserChangedEvent
        {
            Id = Guid.NewGuid().ToString(),
            CpfNumber = "12345678909",
            FullName = "Customer User",
            Role = "CUSTOMER_USER",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "hash",
            LastUpdate = DateTimeOffset.UtcNow,
        };

        await Assert.ThrowsAsync<ConditionalCheckFailedException>(async () =>
            await consumer.Save(message, TestContext.CancellationTokenSource.Token));

        repositoryStub.Verify(repository => repository.Upsert(It.IsAny<UserModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
