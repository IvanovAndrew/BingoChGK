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

public class QuestionSearcher(ILogger<QuestionSearcher> logger) : IQuestionSearcher
{
    private readonly ILogger _logger;
    private static readonly HttpClient HttpClient = new();
    private const string DateTimeMask = "yyyy-MM-dd";

    public async Task<List<Question>> GetQuestions(string query, DateOnly lastUpdate = default)
    {
        query = query?.Trim();

        if (query.StartsWith("/"))
        {
            return [];
        }

        if (query.Length <= 4 && query.All(c => c != ' '))
        {
            query += $"\"{query}\"";
        }

        DateOnly? startPlayed = lastUpdate != default ? lastUpdate.AddDays(1) : null;

        string url =
            $"https://gotquestions.online/search?search={Uri.EscapeDataString(query)}&sSort=pDate&pSort=p&sortDirection=desc&editor=&solo=false&or=false&author=&fromD=0&toD=100&uD=true&fromTrueDL=0&toTrueDL=10&uDL=true&withdrawn=false&ansSearch=true&textSearch=false&commSearch=true&sourceSearch=false&startPlayed={(startPlayed != null ? startPlayed.Value.ToString(DateTimeMask) : string.Empty)}&endPlayed=&type=questions&limit=100";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Accept", "application/json");

        HttpResponseMessage response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string html = await response.Content.ReadAsStringAsync();
        //_logger.LogInformation($"Response is {html}");

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        var body = htmlDoc.DocumentNode.SelectSingleNode("//body");

        List<Question> questions = new List<Question>();

        if (body != null)
        {
            var scripts = body.SelectNodes("script");

            if (scripts != null && scripts.Count > 0)
            {
                var lastScript = scripts.Last();
                string scriptContent = lastScript.InnerText;

                //var questionsJson = ExtractQuestionSubstring(scriptContent, Marker.MultipleQuestions);
                var startIndex = scriptContent.IndexOf('{', scriptContent.IndexOf('{') + 1);
                var endIndex = scriptContent.LastIndexOf('}', scriptContent.LastIndexOf('}') - 1);

                var json = scriptContent.Substring(startIndex, endIndex - startIndex + 1);
                json = json.Replace("\\\\\\\"", "\\\\\"").Replace("\\\"", "\"");

                questions = QuestionJsonParser.ParseQuestions(json);
            }
        }

        return questions;
    }

    public async Task<List<Question>> GetQuestions(IReadOnlyList<int> ids)
    {
        var tasks = ids.Select(id => GetQuestionById(id));
        var questions = await Task.WhenAll(tasks);
        return questions.Where(q => q != null).ToList();
    }

    public async Task<Question?> GetQuestionById(int id)
    {
        string url = $"https://gotquestions.online/question/{id}";
        
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Accept", "application/json");

        HttpResponseMessage response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string html = await response.Content.ReadAsStringAsync();
        //_logger.LogInformation($"Response is {html}");
        
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        var body = htmlDoc.DocumentNode.SelectSingleNode("//body");

        Question? question = null;

        if (body != null)
        {
            var scripts = body.SelectNodes("script");

            if (scripts != null && scripts.Count > 0)
            {
                var t = scripts.Where(n => n.InnerText.Contains("question") || n.InnerText.Contains("tournaments"))
                    .ToList();

                var innerText = string.Empty;
                // combine
                if (t.Count == 3)
                {
                    t.RemoveAt(0);
                }
                
                if (t.Count == 2)
                {
                    var firstPart = t[0].InnerText.Replace("self.__next_f.push", "");
                    firstPart = firstPart.Substring(2, firstPart.Length - 5);
                    
                    var secondPart = t[1].InnerText.Replace("self.__next_f.push", "");
                    secondPart = secondPart.Substring(5, secondPart.Length - 8);

                    innerText = firstPart + secondPart;
                }
                
                var questionsJson = ExtractQuestionSubstring(innerText, Marker.SingleQuestion);

                question = QuestionJsonParser.ParseQuestion(questionsJson);
            }
        }

        return question;
    }

    private string ExtractQuestionSubstring(string content, Marker marker)
    {
        content = content.Replace("\\\\\\\"", "\\\\\"").Replace("\\\"", "\"");

        int startIndex = content.IndexOf(marker.Word, StringComparison.InvariantCultureIgnoreCase);
        if (startIndex == -1)
        {
            startIndex = content.IndexOf(marker.OpeningBracket, StringComparison.InvariantCultureIgnoreCase);
        }
        else
        {
            startIndex = content.IndexOf(marker.OpeningBracket, startIndex);
        }

        if (startIndex == -1)
        {
            return string.Empty;
        }
        
        Stack<char> stack = new Stack<char>();

        for (int i = startIndex; i < content.Length; i++)
        {
            char c = content[i];

            if (c == marker.OpeningBracket)
            {
                stack.Push(c);
            }
            else if (c == marker.ClosingBracket)
            {
                if (stack.Count == 0)
                {
                    continue;
                    // _logger.LogError("Ошибка: несбалансированные скобки.");
                    // return string.Empty;
                }

                stack.Pop();

                // Когда стек опустел — конец массива
                if (stack.Count == 0)
                {
                    int endIndex = i + 1;
                    string jsonArray = content.Substring(startIndex, endIndex - startIndex);

                    return jsonArray;
                }
            }
        }

        return string.Empty;
    }
}

internal struct Marker
{
    internal readonly string Word;
    internal readonly char OpeningBracket;
    internal readonly char ClosingBracket;
    internal static Marker SingleQuestion = new Marker("\"question\"", '{', '}');
    internal static Marker MultipleQuestions = new Marker("\"questions\"", '[', ']');

    private Marker(string word, char openingBracket, char closingBracket)
    {
        Word = word;
        OpeningBracket = openingBracket;
        ClosingBracket = closingBracket;
    }
}