using Mechanics.Auth.Infra.Data.Repositories;

namespace Mechanics.Auth.Application.Consumers;

public class UserChangedEventConsumer(IUserRepository userRepository) : IEventConsumer
{
    public async Task Save(Consumers.UserChangedEvent message, CancellationToken cancellationToken = default) =>
        await userRepository.Upsert(message.ToModel(), cancellationToken);
}
