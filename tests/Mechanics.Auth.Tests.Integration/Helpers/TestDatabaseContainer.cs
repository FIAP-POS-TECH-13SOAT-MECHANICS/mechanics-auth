using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace Mechanics.Auth.Tests.Integration.Helpers;

public class TestDatabaseContainer : IAsyncDisposable
{
    public MsSqlContainer Container { get; } = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2025-latest")
        .WithPassword("b0I6h9G%1zJo")
        .WithName($"test-containers-db-{Guid.NewGuid()}")
        .WithEnvironment("MSSQL_PID", "Express")
        .WithCleanUp(true)
        .Build();

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await Container.DisposeAsync();
    }

    public async Task SeedDatabase(CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(Container.GetConnectionString());
        await connection.OpenAsync(cancellationToken);

        await ExecuteCommand("CREATE SCHEMA Mechanics;");
        await ExecuteCommand(Seed.Command);

        await connection.CloseAsync();
        return;

        Task ExecuteCommand(string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            return command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private static class Seed
    {
        internal const string Command = """
                                        CREATE TABLE Mechanics.Roles
                                        (
                                            Id   UNIQUEIDENTIFIER PRIMARY KEY,
                                            Name NVARCHAR(50) NOT NULL
                                        );

                                        INSERT INTO Mechanics.Roles (Id, Name)
                                        VALUES ('2afde195-550b-498e-a63d-7a6d556b25ba', 'ADMINISTRATOR'),
                                               ('a1097867-aa3e-416c-8685-190516b62a12', 'ATTENDANT'),
                                               ('f6027484-89a4-49f6-a9cb-4d1733c2bab7', 'MECHANIC'),
                                               ('f61b4ae9-cc8f-4fda-a39f-f70bb3c0840f', 'CUSTOMER_USER'),
                                               ('f31bca41-0895-4af5-976f-ac892f833b1b', 'CUSTOMER_ADMIN');

                                        CREATE TABLE Mechanics.Users
                                        (
                                            Id            UNIQUEIDENTIFIER PRIMARY KEY,
                                            FullName      NVARCHAR(100)     NOT NULL,
                                            Email         NVARCHAR(100)     NOT NULL,
                                            CpfNumber     NVARCHAR(11)     NOT NULL,
                                            PasswordHash  NVARCHAR(256)    NOT NULL,
                                            SecurityStamp NVARCHAR(64)     NOT NULL,
                                            CustomerId    UNIQUEIDENTIFIER,
                                            RoleId        UNIQUEIDENTIFIER NOT NULL,
                                            CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Mechanics.Roles (Id)
                                        );

                                        INSERT INTO Mechanics.Users (Id, FullName, Email, CpfNumber, PasswordHash, SecurityStamp, RoleId, CustomerId)
                                        VALUES ('db27b85d-b0f3-4300-bb45-7841f0d11617', 'Administrator User', 'administrator@mechanics.com', '12345678909', 'AQAAAAIAAYagAAAAEOgOg1jaFQeJ6GxRgIcBZkDQANVOaUkUxVex6dLUmv5YRYf0Of7sVIYubKx7btB1iQ==', 'efcaaf76-0535-45fc-a79c-06ab92c064bb', '2afde195-550b-498e-a63d-7a6d556b25ba', NULL),
                                               ('46d6372c-84a9-47d0-9f6d-f28502bbd523', 'Customer User', 'customer.user@mechanics.com', '90526359005', 'AQAAAAIAAYagAAAAEKSeHdHtCfN38pakeil4oyEL0d07GBEySe6csY8jmXIKT3oEZVcZR7Jngd9qxFgmkQ==', '0023a37b-0f7f-4d73-8020-0bf576b2dfc3', 'f61b4ae9-cc8f-4fda-a39f-f70bb3c0840f', '47e311e1-d8d0-4746-b12c-f72b8aba57ca');
                                        """;
    }
}
