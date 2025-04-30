using System.Globalization;
using System.Text;
using Bingo.Domain;

namespace Bingo.Application;

internal static class QuestionFormatter
{
    private static readonly string[] EscapableSymbols =
        { "_", "*", "[", "]", "(", ")", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };

    internal static string FormatQuestion(Question question)
    {
        var escapableQuestion = new EscapableQuestion(question);

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(Bold(escapableQuestion.PackTitle));
        stringBuilder.AppendLine(
            EscapableQuestion.EscapeSymbols(question.Date.ToString("Y", new CultureInfo("Ru-ru"))));
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(Bold($"Вопрос {escapableQuestion.Number}\\."));

        if (!string.IsNullOrEmpty(escapableQuestion.AdditionalMaterialText))
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"{Italic("Раздаточный материал:")}");
            stringBuilder.AppendLine($"{escapableQuestion.AdditionalMaterialText}");
        }

        stringBuilder.AppendLine();
        stringBuilder.AppendLine($"{escapableQuestion.Text}");

        stringBuilder.AppendLine();
        stringBuilder.AppendLine($"{Spoiler(Bold("Ответ: ") + escapableQuestion.Answer)}");
        if (!string.IsNullOrEmpty(escapableQuestion.AcceptedAnswer))
        {
            stringBuilder.AppendLine($"{Spoiler(Bold("Зачёт: ") + escapableQuestion.AcceptedAnswer)}");
        }

        if (!string.IsNullOrEmpty(escapableQuestion.NotAcceptedAnswer))
        {
            stringBuilder.AppendLine($"{Spoiler(Bold("Незачёт: ") + escapableQuestion.NotAcceptedAnswer)}");
        }

        if (!string.IsNullOrEmpty(escapableQuestion.Comment))
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"{Spoiler(Bold("Комментарий: ") + escapableQuestion.Comment)}");
        }

        return stringBuilder.ToString();

        string Italic(string s) => $"_{s}_";
        string Bold(string s) => $"*{s}*";
        string Spoiler(string s) => $"||{s}||";
    }

    private class EscapableQuestion
    {
        private readonly Question _question;

        internal EscapableQuestion(Question question)
        {
            _question = question;
        }

        internal int Number => _question.Number;
        public string Text => EscapeSymbols(_question.Text);
        public string AdditionalMaterialText => EscapeSymbols(_question.AdditionalMaterialText);
        public string Answer => EscapeSymbols(_question.Answer);
        public string AcceptedAnswer => EscapeSymbols(_question.AcceptedAnswer);
        public string NotAcceptedAnswer => EscapeSymbols(_question.NotAcceptedAnswer);
        public string Comment => EscapeSymbols(_question.Comment);
        public string Note => EscapeSymbols(_question.Note);
        public string PackTitle => EscapeSymbols(_question.PackTitle);
        public DateOnly Date => _question.Date;

        internal static string EscapeSymbols(string originalString)
        {
            string result = originalString?? string.Empty;
            foreach (var symbolToReplace in EscapableSymbols)
            {
                result = result.Replace(symbolToReplace, $"\\{symbolToReplace}");
            }

            return result;
        }
    }
}