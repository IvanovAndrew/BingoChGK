using Bingo.Application.FetchNewQuestions;
using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.GetBingoQuestion;

public class GetBingoQuestionCommandHandler(
    BingoQuestionService bingoQuestionService,
    IUserRepository userRepository,
    ITelegramBot telegramBot,
    IMediator mediator,
    ILogger<GetBingoQuestionCommandHandler> logger)
    : IRequestHandler<GetBingoQuestionCommand>
{
    public async Task Handle(GetBingoQuestionCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Started {command}");

        var trainingSession = await userRepository.GetUserSession(command.ChatId);
        
        var questions = await bingoQuestionService.GetQuestionsForBingoAsync(command.BingoId);
        
        if (questions.Any())
        {
            var question = trainingSession.GetBingoQuestion(command.BingoId, questions);
                    
            var format = QuestionFormatter.FormatQuestion(question);

            var user = trainingSession.User;
            
            TelegramButton[] buttons;
            if (user.CanRemoveQuestion)
            {
                buttons =
                [
                    TelegramButton.ShowAQuestionButton(command.BingoId, "Next question"),
                    TelegramButton.DeleteQuestionButton(command.BingoId, question.GotQuestionId, "Delete"),
                    TelegramButton.ShowRandomBingoButton("Next bingo"),
                ];
            }
            else
            {
                buttons =
                [
                    TelegramButton.ShowAQuestionButton(command.BingoId, "Next question"),
                    TelegramButton.ShowRandomBingoButton("Next bingo"),
                ];
            }

            ITelegramMessage sentMessage; 
            
            if (!string.IsNullOrEmpty(question.AdditionalMaterialPictureUrl))
            {
                sentMessage = await telegramBot.SendPhotoAsync(user.Id, new Uri(question.AdditionalMaterialPictureUrl), caption: format, useMarkdown: true, buttons: buttons, cancellationToken: cancellationToken);
            }
            else
            {
                sentMessage = await telegramBot.SendTextMessageAsync(user.Id, format, useMarkdown:true, buttons: buttons, cancellationToken: cancellationToken);
            }
            
            logger.LogInformation($"Question id is {question.GotQuestionId}. Text is {format}");
        }
        else
        {
            if (command.RetryIfNoQuestions)
            {
                await mediator.Send(new FetchNewQuestionsCommand(){BingoId = command.BingoId}, cancellationToken);
                await mediator.Send(command with {RetryIfNoQuestions = false}, cancellationToken);
            }
            else
            {
                await telegramBot.SendTextMessageAsync(command.ChatId, $"There are no questions for the bingo", useMarkdown:true, cancellationToken: cancellationToken);
            }
        }
    }
}