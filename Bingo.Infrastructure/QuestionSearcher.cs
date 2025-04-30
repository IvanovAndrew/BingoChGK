using Bingo.Domain;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Bingo.Infrastructure;

internal class Labels
{
    internal const string Question = "Вопрос";
    internal const string Answer = "Ответ: ";
    internal const string AcceptableAnswer = "Зачёт: ";
    internal const string NotAcceptableAnswer = "Незачёт: ";
    internal const string Comment = "Комментарий: ";
    internal const string Note = "Замечания: ";
    internal const string AdditionalMaterial = "Раздаточный материал:";
    internal const string Sources = "Источники: ";
    internal const string Author = "Автор: ";
}

public class QuestionSearcher : IQuestionSearcher
{
    private readonly ILogger _logger;
    private static readonly HttpClient HttpClient = new();
    private const string DateTimeMask = "yyyy-MM-dd";

    public QuestionSearcher(ILogger<QuestionSearcher> logger)
    {
        _logger = logger;
    }

    public async Task<List<Question>> GetQuestions(string query, int bingoId, DateOnly lastUpdate = default)
    {
        query = query?.Trim();

        if (query.StartsWith("/"))
        {
            return [];
        }
        
        DateOnly? startPlayed = lastUpdate != default? lastUpdate.AddDays(1) : null;  

        string url =
            $"https://gotquestions.online/search?search={Uri.EscapeDataString(query)}&sSort=pDate&pSort=p&sortDirection=desc&editor=&solo=false&or=false&author=&fromD=0&toD=100&uD=true&fromTrueDL=0&toTrueDL=10&uDL=true&withdrawn=false&ansSearch=true&textSearch=true&commSearch=true&sourceSearch=false&startPlayed={(startPlayed != null? startPlayed.Value.ToString(DateTimeMask) : string.Empty)}&endPlayed=&type=questions&limit=100";

        HttpResponseMessage response = await HttpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string html = await response.Content.ReadAsStringAsync();
        
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        var scriptNode = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'flex') and contains(@class, 'justify-start') and contains(@class, 'w-full')]/div");

        _logger.LogInformation($"https://gotquestions.online/search returned {scriptNode?.Count} questions");

        var questions = new List<Question>();
        var htmlParser = new QuestionHtmlParser();
        
        if (scriptNode != null)
        {
            foreach (var node in scriptNode)
            {
                try
                {
                    var question = htmlParser.Parse(node);
                    question.BingoId = bingoId;
                    questions.Add(question);
                }
                catch (Exception e)
                {
                    _logger.LogError($"The error happened: {e}");
                }
            }
        }

        return questions;
    }
}