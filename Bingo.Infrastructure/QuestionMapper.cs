using Bingo.Domain;
using Bingo.Infrastructure.Database;

namespace Bingo.Infrastructure;

public static class QuestionMapper
{
    public static Question ToDomain(QuestionDb question)
    {
        return new Question()
        {
            Id = question.Id,
            PackTitle = question.PackTitle,
            Date = DateOnly.FromDateTime(question.Date),
            Number = question.Number,
            Text = question.Text,
            AdditionalMaterialText = question.AdditionalMaterialText,
            AdditionalMaterialPictureUrl = question.AdditionalMaterialPictureUrl,
                    
            Answer = question.Answer,
            AcceptedAnswer = question.AcceptedAnswer,
            NotAcceptedAnswer = question.NotAcceptedAnswer,
                    
            Comment = question.Comment,
            Note = question.Note,
            Author = question.Author,
            Editor = question.Editor,
            BingoId = question.BingoId,
        };
    }

    public static QuestionDb FromDomain(Question question)
    {
        return new QuestionDb()
        {
            Id = question.Id,
            PackTitle = question.PackTitle,
            Date = new DateTime(question.Date.Year, question.Date.Month, question.Date.Day),
            Number = question.Number,
            Text = question.Text,
            AdditionalMaterialText = question.AdditionalMaterialText,
            AdditionalMaterialPictureUrl = question.AdditionalMaterialPictureUrl,
                    
            Answer = question.Answer,
            AcceptedAnswer = question.AcceptedAnswer,
            NotAcceptedAnswer = question.NotAcceptedAnswer,
                    
            Comment = question.Comment,
            Note = question.Note,
            Author = question.Author,
            Editor = question.Editor,
            BingoId = question.BingoId,
        };
    }
}