using Bingo.Application.FetchNewQuestions;
using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.GetBingoDescription;

public class BingoInfoCommandHandler(
    IBingoRepository bingoRepository,
    IUserRepository userRepository,
    IMediator mediator,
    ILogger<BingoInfoCommandHandler> logger)
    : IRequestHandler<GetBingoDescriptionCommand>
{
    public async Task Handle(GetBingoDescriptionCommand command, CancellationToken cancellationToken)
    {
        Domain.Bingo? bingo;
        if (command.BingoId != null)
        {
            bingo = await bingoRepository.GetBingoByID(command.BingoId.Value); 
        }
        else //if (command.Text != null)
        {
            bingo = await bingoRepository.GetBingoByText(command.Text);
            if (bingo != null)
            {
                await mediator.Send(new FetchNewQuestionsCommand(bingo.Id), cancellationToken);
            }
        }

        if (bingo != null)
        {
            await userRepository.SaveSession(command.ChatId, bingo.Id, DateTime.Now);

            string description = string.Empty;

            if (bingo?.InstantViewUrl != null)
            {
                description = bingo.InstantViewUrl;
            }
            else if (!string.IsNullOrEmpty(bingo?.Description))
            {
                description = $"{bingo.Text}{Environment.NewLine}{bingo.Description}";
            }

            if (!string.IsNullOrEmpty(description))
            {
                await mediator.Publish(new DescriptionFoundEvent(command.ChatId, bingo.Id, description), cancellationToken);
            }
            else
            {
                await mediator.Publish(new DescriptionNotFoundEvent()
                    { ChatId = command.ChatId, BingoId = bingo.Id, BingoText = bingo?.Text }, cancellationToken);
            }
        }
        else
        {
            await mediator.Publish(new BingoNotFoundEvent() { ChatId = command.ChatId, BingoText = command.Text, ReplyTo = command.MessageId }, cancellationToken);
        }
    }
}