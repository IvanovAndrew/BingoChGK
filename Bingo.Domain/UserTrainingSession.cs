using BingoChGK;

namespace Bingo.Domain;

/// <summary>
/// История предыдущих сообщений
/// - запоминать изученные бинго
/// - запоминать уже показанные вопросы
/// </summary>
public class UserTrainingSession : AggregateRoot
{
    const int DAY_INTERVAL = 3;
    
    private Dictionary<int, DateTime> _lastLearntBingo = new();
    private Dictionary<int, DateTime> _lastAskedQuestions = new();
    public User User { get; init; }

    public UserTrainingSession(User user, Dictionary<int, DateTime> lastLearntBingo)
    {
        User = user;
        _lastLearntBingo = lastLearntBingo;
    }
    
    public Bingo GetRandomBingo(List<Bingo> allBingos)
    {
        var availableBingo = GetAllNewBingo(allBingos);

        var bingoToShow = Randomizer.GetRandomElement(availableBingo);
        
        _lastLearntBingo[bingoToShow.Id] = DateTime.Now;
        
        AddEvent(new BingoDescriptionShownDomainEvent());

        return bingoToShow;
    }

    private List<Bingo> GetAllNewBingo(List<Bingo> bingos)
    {
        var list = new List<Bingo>(bingos.Count);

        foreach (var bingo in bingos)
        {
            
            if (_lastLearntBingo.TryGetValue(bingo.Id, out var date) && date.AddDays(DAY_INTERVAL) >= DateTime.Today)
            {
                continue;
            }
            
            list.Add(bingo);
        }

        return list;
    }

    public Question GetBingoQuestion(Bingo bingo)
    {
        if (bingo.Questions.Count == 0)
        {
            throw new InvalidOperationException($"Bingo {bingo.Text} doesn't have any questions");
        }

        var newQuestions = GetNewQuestions(bingo.Questions);

        if (newQuestions.Count == 0)
        {
            throw new InvalidOperationException($"Bingo {bingo.Text} doesn't have any new questions");
        }
        
        var questionToAsk = Randomizer.GetRandomElement(newQuestions);
        _lastAskedQuestions[questionToAsk.Id] = DateTime.Now;
        
        AddEvent(new QuestionAskedDomainEvent());

        return questionToAsk;
    }
    
    private List<Question> GetNewQuestions(IReadOnlyList<Question> questions)
    {
        var list = new List<Question>(questions.Count);

        foreach (var question in questions)
        {
            if (_lastAskedQuestions.TryGetValue(question.Id, out var date) && date.AddDays(DAY_INTERVAL) >= DateTime.Today)
            {
                continue;
            }
            
            list.Add(question);
        }

        return list;
    }
}