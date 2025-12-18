using Bingo.Domain;
using MediatR;

namespace Bingo.Application.Subscribe;

public record UnsubscribeCommand : IRequest
{
    public long ChatId { get; init; }
}

public class UnsubscribeCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<UnsubscribeCommand>
{
    public async Task Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserById(request.ChatId);

        if (user == null)
            return;
        
        user.MakeInactive();
        await userRepository.Update(user);

        await unitOfWork.CommitAsync(cancellationToken);
    }
}