using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.AddBingo;

public class AddBingoCommandHandler(
    IBingoRepository bingoRepository,
    IQuestionRepository questionRepository,
    IQuestionSearcher questionSearcher,
    IMediator mediator,
    ILogger<AddBingoCommandHandler> logger)
    : IRequestHandler<AddBingoCommand>
{
    private readonly ILogger _logger = logger;

    public async Task Handle(AddBingoCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(AddBingoCommandHandler));

        var bingoWord = command.Text.Trim();
        var nextId = (await bingoRepository.GetAllBingos()).Max(c => c.Id) + 1;
        var newBingo = new BingoBuilder(nextId, bingoWord, null).Build();
        await bingoRepository.SaveBingo(newBingo);

        await mediator.Publish(new NewBingoSavedEvent() { ChatId = command.ChatId, MessageId = command.MessageToEdit }, cancellationToken);
        _logger.LogInformation($"Bingo {bingoWord} has been added");
        
        await mediator.Publish(new DownloadingNewBingoQuestionsStartedEvent() { ChatId = command.ChatId, MessageId = command.MessageToEdit }, cancellationToken);
        
        var questionsToAdd = await questionSearcher.GetQuestions(bingoWord, newBingo.Id);
        if (questionsToAdd.Count != 0)
        {
            await questionRepository.InsertQuestions(questionsToAdd);
            _logger.LogInformation(
                $"{questionsToAdd.Count} question{(questionsToAdd.Count > 1 ? "s" : "")} for bingo {bingoWord} {(questionsToAdd.Count > 1 ? "have" : "has")} been added");
        }
        else
        {
            _logger.LogInformation($"There is no questions for bingo {bingoWord}");
        }
        
        await mediator.Publish(new NewBingoQuestionsDownloadedEvent()
        {
            ChatId = command.ChatId, 
            MessageId = command.MessageToEdit, 
            QuestionsCount = questionsToAdd.Count,
            BingoWord = newBingo.Text,
            BingoId = newBingo.Id
        }, cancellationToken);
    }
}