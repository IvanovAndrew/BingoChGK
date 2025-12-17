using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bingo.Domain;

namespace Bingo.Infrastructure;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private static readonly string[] Formats = new[]
    {
        "yyyy-MM-dd HH:mm:ss.ffffff", 
        "yyyy-MM-dd HH:mm:ss.fff",    
        "yyyy-MM-dd HH:mm:ss"         
    };
    
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString()!;
        if (DateTime.TryParseExact(str, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            return dt;
        }
        
        throw new FormatException($"Unknown date time format: {str}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
    }
}


public static class QuestionJsonParser
{
    private static JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    }; 
    
    public static List<Domain.Question> ParseQuestions(string json)
    {
        var jsonResponse = JsonSerializer.Deserialize<JsonResponse>(json, Options);

        return jsonResponse.Questions.Select(Map).ToList();
    }

    private static Question Map(JsonQuestion question)
    {
        return new Domain.Question()
            {
                GotQuestionId = question.id,
                
                PackTitle = question.packTitle,
                Number = question.number,

                AdditionalMaterialText = ReplaceNewLine(question.razdatkaText),
                AdditionalMaterialPictureUrl = ReplaceUrl(question.razdatkaPic),

                Date = question.endDate.ToDateOnly(),

                Text = ReplaceNewLine(question.text),
                Answer = ReplaceNewLine(question.answer),
                Comment = ReplaceNewLine(question.comment),
                CommentPictureUrl = ReplaceUrl(question.commentPic),

                Note = ReplaceNewLine(question.note),
                AcceptedAnswer = ReplaceNewLine(question.zachet),
                NotAcceptedAnswer = ReplaceNewLine(question.nezachet),

                Sources = [question.source],
                Authors = question.authors?.Select(a => a.name).ToArray()?? [],
                Editor = question.editors?.FirstOrDefault()?.name?? string.Empty,
                
                Teams = question.teams,
                CorrectAnswers = question.correct_answers,
            };

        string ReplaceNewLine(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Replace("\\n", Environment.NewLine);
        }

        string ReplaceUrl(string text)
        {
            return !string.IsNullOrEmpty(text) ? $"https://gotquestions.online{text}" : string.Empty;
        }
    }

    public static Domain.Question? ParseQuestion(string json)
    {
        var question = JsonSerializer.Deserialize<JsonQuestion>(json, Options);

        return Map(question);
    }
}

public class JsonResponse
{
    public RequestParams Params { get; set; }
    public List<JsonQuestion> Questions { get; set; }
        
}

public class RequestParams
{
    
}

public class JsonQuestion
{
    public int id { get; set; }
    public int number { get; set; }
    public string text { get; set; }
    public string razdatkaText { get; set; }
    public string razdatkaPic { get; set; }
    public string audio { get; set; }
    public string commentAudio { get; set; }
    public string answer { get; set; }
    public string answerPic { get; set; }
    public string zachet { get; set; }
    public string nezachet { get; set; }
    public string comment { get; set; }
    public string note { get; set; }
    public string commentPic { get; set; }
    public string source { get; set; }

    public List<Person> authors { get; set; }
    public List<Person> editors { get; set; }
    //public Tour tour { get; set; }
    public int packId { get; set; }
    public string packTitle { get; set; }
    public List<object> controversials { get; set; }
    public List<object> appeals { get; set; }
    public int totalLikes { get; set; }
    public int totalDislikes { get; set; }
    
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime pubDate { get; set; }
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime endDate { get; set; }
    
    public int uploader { get; set; }
    public List<object> tags { get; set; }
    public bool takenDown { get; set; }
    public bool marked { get; set; }
    public int gNum { get; set; }
    public List<int> teams { get; set; }
    public List<double> complexity { get; set; }
    public List<int> correct_answers { get; set; }
    public List<Tournament> tournaments { get; set; }
}

public class Person
{
    public int id { get; set; }
    public string name { get; set; }
    public string gender { get; set; } // "HE", "SHE", "SE", etc.
}

public class Tournament
{
    public int id { get; set; }
    public string title { get; set; }
    public TournamentType typeoft { get; set; }
}

public class TournamentType
{
    public int id { get; set; }
    public string title { get; set; }
}

public class Tour
{
    public int id { get; set; }
    public int number { get; set; }
    public string title { get; set; }
}
