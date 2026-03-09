using Mechanics.Auth.Infra.Data.Models;

namespace Mechanics.Auth.Tests.Unit.Mocks;

public static class RoleMocks
{
    public static IEnumerable<RoleModel> Roles =>
    [
        new() { Id = new Guid("2afde195-550b-498e-a63d-7a6d556b25ba"), Name = "ADMINISTRATOR" },
        new() { Id = new Guid("a1097867-aa3e-416c-8685-190516b62a12"), Name = "ATTENDANT" },
        new() { Id = new Guid("f6027484-89a4-49f6-a9cb-4d1733c2bab7"), Name = "MECHANIC" },
        new() { Id = new Guid("f61b4ae9-cc8f-4fda-a39f-f70bb3c0840f"), Name = "CUSTOMER_USER" },
        new() { Id = new Guid("f31bca41-0895-4af5-976f-ac892f833b1b"), Name = "CUSTOMER_ADMIN" },
    ];

    public static class Names
    {
        /// <summary>
        ///     Acesso total ao sistema.
        /// </summary>
        public const string Administrator = "ADMINISTRATOR";

        /// <summary>
        ///     Permite cadastrar clientes e veículos e gerar ordens de serviços.
        /// </summary>
        public const string Attendant = "ATTENDANT";

        /// <summary>
        ///     Permite gerenciar ordens de serviços e realizar manutenções.
        /// </summary>
        public const string Mechanic = "MECHANIC";

        /// <summary>
        ///     Cliente comum.
        ///     Permite consultar ordens de serviço do seu cliente.
        /// </summary>
        public const string CustomerUser = "CUSTOMER_USER";

        /// <summary>
        ///     Cliente administrador.
        ///     Permite gerenciar outros usuários do mesmo cliente.
        /// </summary>
        public const string CustomerAdmin = "CUSTOMER_ADMIN";
    }
}
