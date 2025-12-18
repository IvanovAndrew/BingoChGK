using System.Net;
using System.Text.Json;
using Bingo.Application;
using Bingo.Application.AddBingo;
using Bingo.Application.DeleteQuestion;
using Bingo.Application.EditBingoDescription;
using Bingo.Application.GetBingoDescription;
using Bingo.Application.GetBingoQuestion;
using Bingo.Application.Info;
using Bingo.Application.RandomBingoDescription;
using Bingo.Application.SendRandomBingoToActiveSubscribers;
using Bingo.Application.Subscribe;
using Bingo.Domain;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = Bingo.Domain.User;

namespace BingoChGK;

public class AzureFunction(IMediator mediator, ITelegramBot telegramBot, IConversationFlowManager conversationFlowManager, IUserRepository userRepository, ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<AzureFunction>();

    [Function("GetBingoQuestion")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation($"Request is {requestBody}");

            // await telegramBot.SetMyCommandsAsync(
            //     TelegramConstants.Commands.Select(kvp => new BotCommand(kvp.Key, kvp.Value))
            //         .ToArray()
            // );

            Update request = JsonSerializer.Deserialize<Update>(requestBody, JsonBotAPI.Options);

            long chatId = 0;
            int? messageId = null;
            string text = string.Empty;

            if (request.Type == UpdateType.CallbackQuery)
            {
                var message = request.CallbackQuery!;
                chatId = message.Message.Chat.Id;
                messageId = message.Message?.MessageId;
                text = message.Data;
                
                if (messageId.HasValue)
                {
                    _logger.LogInformation("Callback pressed. Removing buttons");
                    await telegramBot.EditMessageReplyMarkupAsync(chatId, messageId.Value, buttons: null);
                }
            }
            else if (request.Type == UpdateType.Message)
            {
                chatId = request.Message.Chat.Id;
                text = request.Message?.Text;
            }
            else
            {
                _logger.LogWarning($"Unknown message type: {request.Type}");
                return await CreateResponse(req);
            }

            if (chatId < 0)
            {
                _logger.LogInformation($"This is chat {chatId}");
            }

            var user = await userRepository.GetUserById(chatId);
            if (user == null)
            {
                _logger.LogInformation($"New user with id {chatId}");
                
                await userRepository.CreateUser(new User(chatId, false, false));

                await mediator.Send(new SendInfoCommand() { ChatId = chatId });
                
                _logger.LogInformation($"New user with id {chatId} has been created");
            }

            _logger.LogInformation(
                $"Chat id is {chatId}. Message is {(messageId?.ToString() ?? "<null>")}. Text is {text}");

            string prefix = string.Empty;

            string fullText = text ?? string.Empty;
            if (!string.IsNullOrEmpty(prefix))
            {
                _logger.LogInformation($"Prefix is {prefix}");
                fullText = $"{prefix} {text}";
            }

            var arr = fullText.Split(" ");

            var textCommand = arr[0].Trim();
            string parameter = !string.IsNullOrEmpty(arr[0])? fullText.Replace(arr[0], "").Trim() : string.Empty;

            _logger.LogInformation($"Command is {textCommand} Parameter is {parameter}");

            IRequest command = null;

            if (!string.IsNullOrWhiteSpace(text) && !text.StartsWith("/"))
            {
                var (stepCommand, bingoId) = await conversationFlowManager.GetStepAsync(chatId);
                
                _logger.LogInformation($"Step command: {stepCommand} BingoId {bingoId}");
                
                if (!string.IsNullOrEmpty(stepCommand))
                {
                    switch (stepCommand)
                    {
                        case "/editdescription":
                            command = new EditBingoDescriptionCommand
                            {
                                ChatId = chatId,
                                MessageId = messageId ?? 0,
                                BingoId = bingoId,
                                Description = text
                            };
                            await conversationFlowManager.ClearStepAsync(chatId);
                            break;
                    }
                }
            }
            
            if (command is null)
            {
                _logger.LogInformation($"Command is null. Checking textCommand {textCommand}");
                
                switch (textCommand)
                {
                    case "/add":
                        command = new AddBingoCommand()
                            { ChatId = chatId, MessageToEdit = (int)messageId!, Text = parameter };
                        break;
                    case "/question":
                        command = new GetBingoQuestionCommand() { ChatId = chatId, BingoId = int.Parse(parameter) };
                        break;
                    case "/deletequestion":
                        var parameters = parameter.Replace("bingo", string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        (int questionId, int bingoId) = (int.Parse(parameters[0]), int.Parse(parameters[1]));
                        _logger.LogInformation($"QuestionId = {questionId} BingoId = {bingoId}");
                        command = new DeleteQuestionCommand()
                            { ChatId = chatId, BingoID = bingoId, QuestionID = questionId };
                        break;
                    case "/random":
                        command = new RandomBingoInfoCommand() { ChatId = chatId };
                        break;
                    case "/editdescription":
                        var editDescriptionParameters = parameter.Replace("bingo", string.Empty).Trim();
                        bingoId = int.Parse(editDescriptionParameters.Split(' ', StringSplitOptions.TrimEntries)[0]);

                        command = new StartAddDescriptionCommand()
                        {
                            ChatId = chatId, MessageId = (int)messageId!, BingoId = bingoId
                        };
                        break;
                    
                    case "/subscribe":
                        command = new SubscribeCommand()
                        {
                            ChatId = chatId, 
                        };
                        break;
                    
                    case "/unsubscribe":
                        command = new UnsubscribeCommand()
                        {
                            ChatId = chatId, 
                        };
                        break;
                    
                    case "/start":
                    case "/info":
                        command = new SendInfoCommand(){ChatId = chatId};
                        break;
                        
                    case "/skip":
                        // do nothing
                        break;
                    default:
                        if (!string.IsNullOrEmpty(text))
                        {
                            command = new GetBingoDescriptionCommand() { ChatId = chatId, Text = text };
                        }
                        break;
                }
            }
            
            if (command is not null)
            {
                await mediator.Send(command);
            }
            else
            {
                _logger.LogInformation("Command is empty");
            }
            
            return await CreateResponse(req);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());

            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync("An error occurred while processing your request. " + e);
            return response;
        }
    }

    [Function("GetRandomBingoQuestion")]
    public async Task RunByTimer([TimerTrigger("0 0 6-18/6 * * *")] TimerInfo timerInfo)
    {
        try
        {
            await mediator.Send(new SendRandomBingoToActiveSubscribersCommand());
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
        }
    }

    private static async Task<HttpResponseData> CreateResponse(HttpRequestData request)
    {
        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        await response.WriteStringAsync("Message was handled");

        return response;
    }
}