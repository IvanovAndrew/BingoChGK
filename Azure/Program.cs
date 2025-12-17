using System.Reflection;
using Bingo.Application;
using Bingo.Application.AddBingo;
using Bingo.Application.Services;
using Bingo.Domain;
using Bingo.Infrastructure;
using BingoChGK;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddLogging();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetAssembly(typeof(AddBingoCommand)), Assembly.GetAssembly(typeof(BingoDescriptionShownDomainEvent))));

        var dbClient = new Supabase.Client(
            Environment.GetEnvironmentVariable("SUPABASE_URL"),
            Environment.GetEnvironmentVariable("SUPABASE_KEY"));

        services.AddSingleton<BingoRepository>(s => ActivatorUtilities.CreateInstance<BingoRepository>(s, dbClient));
        services.AddSingleton<IBingoRepository, BingoRepositoryCacheDecorator>(provider =>
            new BingoRepositoryCacheDecorator(provider.GetRequiredService<BingoRepository>(),
                provider.GetRequiredService<ILogger<BingoRepositoryCacheDecorator>>()));

        services.AddSingleton<UserRepository>(s => ActivatorUtilities.CreateInstance<UserRepository>(s, dbClient));
        services.AddSingleton<IUserRepository, UserRepositoryCacheDecorator>(provider =>
            new UserRepositoryCacheDecorator(provider.GetRequiredService<UserRepository>(),
                provider.GetRequiredService<ILogger<UserRepository>>()));

        services.AddSingleton<IQuestionRepository, QuestionRepository>(s =>
            ActivatorUtilities.CreateInstance<QuestionRepository>(s, dbClient));
        services.AddSingleton<QuestionSearcher>();

        services.AddSingleton<AzureStorageOptions>(s => new AzureStorageOptions()
        {
            ConnectionString = Environment.GetEnvironmentVariable("AzureStorage"),
            TableName = Environment.GetEnvironmentVariable("AzureTable"),
        });

        services.AddSingleton<ISessionService, AzureTableSessionService>();
        services.AddSingleton<IConversationFlowManager, ConversationFlowManager>();
        
        services.AddSingleton<IBingoLookupService, BingoLookupService>();

        services.AddSingleton<IQuestionSearcher>(provider =>
            new QuestionsCacheDecorator(provider.GetRequiredService<QuestionSearcher>(),
                provider.GetRequiredService<ILogger<QuestionsCacheDecorator>>()));

        services.AddSingleton<ITelegramBot, TelegramBotImpl>(s =>
            ActivatorUtilities.CreateInstance<TelegramBotImpl>(s, Environment.GetEnvironmentVariable("token")));

        services.AddSingleton<BingoQuestionService>();
        services.AddSingleton<IQuestionService, QuestionService>();

        services.AddSingleton<AzureFunction>();
    })
    .Build();

host.Run();