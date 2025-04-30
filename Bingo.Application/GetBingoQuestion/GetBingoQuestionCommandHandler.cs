using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.GetBingoQuestion;

public class GetBingoQuestionCommandHandler(
    IBingoRepository bingoRepository,
    IUserRepository userRepository,
    ITelegramBot telegramBot,
    ILogger<GetBingoQuestionCommandHandler> logger)
    : IRequestHandler<GetBingoQuestionCommand>
{
    public async Task Handle(GetBingoQuestionCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation(nameof(GetBingoQuestionCommandHandler));

        var trainingSession = await userRepository.GetUserSession(command.ChatId);
        
        var bingo = await bingoRepository.GetBingoByID(command.BingoId);
        
        if (bingo.Questions.Count > 0)
        {
            var question = trainingSession.GetBingoQuestion(bingo);
                    
            var format = QuestionFormatter.FormatQuestion(question);

            var user = trainingSession.User;
            
            TelegramButton[] buttons;
            if (user.CanRemoveQuestion)
            {
                buttons =
                [
                    new() { Text = "Next question", Callback = $"/question {bingo.Id}"},
                    new() { Text = "Delete", Callback = $"/deletequestion {question.Id} bingo {bingo.Id}"},
                    new() { Text = "Next bingo", Callback = "/random"}
                ];
            }
            else
            {
                buttons =
                [
                    new() { Text = "Next question", Callback = $"/question {bingo.Id}"},
                    new() { Text = "Next bingo", Callback = "/random"}
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
            
            logger.LogInformation($"Question id is {question.Id}. Text is {format}");
        }
        else
        {
            await telegramBot.SendTextMessageAsync(command.ChatId, $"There are no questions about {bingo.Text}", useMarkdown:true, cancellationToken: cancellationToken);
        }
    }
}