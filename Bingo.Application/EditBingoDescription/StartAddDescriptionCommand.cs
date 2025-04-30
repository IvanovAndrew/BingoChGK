using MediatR;

namespace Bingo.Application.EditBingoDescription;

public class StartAddDescriptionCommand : IRequest
{
    public long ChatId { get; init; }
    public int BingoId { get; init; }
    public int MessageId { get; init; }
}

public class StartAddDescriptionCommandHandler(IConversationFlowManager сonversationFlowManager, ITelegramBot telegramBot) : IRequestHandler<StartAddDescriptionCommand>
{
    public async Task Handle(StartAddDescriptionCommand request, CancellationToken cancellationToken)
    {
        await сonversationFlowManager.SetStepAsync(request.ChatId, "/editdescription", request.BingoId);
        
        await telegramBot.EditTextMessageAsync(request.ChatId, request.MessageId, "Write the description", cancellationToken: cancellationToken);
    }
}