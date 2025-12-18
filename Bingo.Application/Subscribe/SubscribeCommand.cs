using Bingo.Domain;
using MediatR;

namespace Bingo.Application.Subscribe;

public record SubscribeCommand : IRequest
{
    public long ChatId { get; init; }
}

public class SubscribeCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<SubscribeCommand>
{
    public async Task Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserById(request.ChatId);

        if (user == null)
            return;
        
        user.MakeActive();
        await userRepository.Update(user);

        await unitOfWork.CommitAsync(cancellationToken);
    }
}