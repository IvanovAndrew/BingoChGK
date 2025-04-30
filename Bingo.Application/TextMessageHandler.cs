// using Bingo.Application.GetBingoDescription;
// using Bingo.Domain;
// using BingoChGK;
// using Microsoft.Extensions.Logging;
//
// namespace Bingo.Application;
//
// internal class TextMessageHandler : TelegramCommandHandler
// {
//     internal  async Task<IMessagePostAction> Handle(string message, IBingoRepository repository,
//         ITelegramBot botClient,
//         User user,
//         IQuestionSearcher questionSearcher, ILogger logger)
//     {
//         ITelegramMessage sentMessage;
//         
//         var bingo = await repository.GetBingoByID(int.Parse(message));
//         
//         if (bingo != null)
//         {
//             return await new BingoInfoCommandHandler().Handle(bingo.Id.ToString(), repository, botClient, user, questionSearcher, logger);
//         }
//         
//         sentMessage = await botClient.SendTextMessageAsync(user.Id, $"There is no bingo in db. Do you want to add it?",
//             buttons:
//             [
//                 new (){ Text = "Add", Callback = $"/add {message}"},
//                 new (){ Text = "No", Callback = $"/skip"}
//             ]
//         );
//     }
// }