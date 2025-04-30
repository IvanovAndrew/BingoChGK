using System.Globalization;
using Bingo.Domain;
using HtmlAgilityPack;

namespace Bingo.Infrastructure;

public class QuestionHtmlParser
{
    private readonly CultureInfo _cultureInfo = new CultureInfo("ru-RU");
    private const string DateMask = "MMMM yyyy";
    
    private static readonly Dictionary<string, Action<Question, string>> _handlers = new()
    {
        [Labels.Answer] = (q, v) => q.Answer = v,
        [Labels.AcceptableAnswer] = (q, v) => q.AcceptedAnswer = v,
        [Labels.NotAcceptableAnswer] = (q, v) => q.NotAcceptedAnswer = v,
        [Labels.Comment] = (q, v) => q.Comment = v,
        [Labels.Note] = (q, v) => q.Note = v,
        [Labels.Sources] = (q, v) => q.Sources = [v],
        [Labels.Author] = (q, v) => q.Authors = [v],
    };

    
    public Question Parse(HtmlNode node)
    {
        var question = new Question { Number = -1 };

        foreach (var child in node.ChildNodes)
        {
            if (question.Number == -1)
            {
                FillPackageInfo(child, question);
            }
            else
            {
                if (question.AdditionalMaterialText == null && question.AdditionalMaterialPictureUrl == null &&
                    child.InnerText.Contains(Labels.AdditionalMaterial))
                {
                    FillAdditionalMaterial(child, question);
                }
                else
                {
                    FillQuestion(question, child);
                }
            }
        }

        return question;
    }

    private void FillQuestion(Question question, HtmlNode child)
    {
        if (string.IsNullOrEmpty(question.Text))
        {
            question.Text = Escape(child.InnerText);
        }
        else if (!string.IsNullOrEmpty(child.InnerText))
        {
            var nodeWithData = child.FirstChild;

            var data = nodeWithData.ChildNodes.Select(c => c.InnerText).ToList();

            foreach (var info in data)
            {
                var handler = _handlers.FirstOrDefault(h => info.StartsWith(h.Key));
                if (handler.Key != null)
                {
                    var value = Escape(info.Replace(handler.Key, string.Empty));
                    handler.Value(question, value);
                }
            }
        }
    }

    private void FillAdditionalMaterial(HtmlNode node, Question question)
    {
        var img = node.SelectSingleNode(".//div[contains(@class, 'relative') and contains(@class, 'border')]//img");
        if (img != null)
        {
            question.AdditionalMaterialPictureUrl = img.GetAttributeValue("src", string.Empty);
            question.AdditionalMaterialText = string.Empty;
        }
        else
        {
            var spans = node.SelectNodes(".//span[not(ancestor::button)]");
            foreach (var span in spans)
            {
                if (span.InnerText.StartsWith(Labels.AdditionalMaterial)) continue;
                
                var text = span.InnerText.Replace(Labels.AdditionalMaterial, string.Empty).Trim();
                question.AdditionalMaterialText = Escape(text);
            }
        }
    }

    private void FillPackageInfo(HtmlNode node, Question question)
    {
        // the first node
        string questionNumberAsAString = node.ChildNodes[0].InnerText.Replace(Labels.Question, string.Empty).Trim();
        question.Number = int.Parse(questionNumberAsAString);

        var grandChildren = node.ChildNodes[1].FirstChild.FirstChild.ChildNodes;

        question.PackTitle = Escape(grandChildren[0].ChildNodes[0].InnerText);
        if (DateOnly.TryParseExact(grandChildren[0].ChildNodes[1].InnerText.Trim().Trim('·').Trim(), DateMask,
                _cultureInfo, DateTimeStyles.None, out var date))
        {
            question.Date = date;
        }
    }

    private string Escape(string s)
    {
        return HtmlEntity.DeEntitize(s);
    }
}