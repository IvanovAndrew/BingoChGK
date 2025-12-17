using Postgrest.Attributes;
using Postgrest.Models;

namespace Bingo.Infrastructure.Database;

[Table("chgk.bingo_question")]
public class BingoQuestionLinkDb : BaseModel
{
    [PrimaryKey("bingo_id", true)]
    public int BingoId { get; set; }
    
    [PrimaryKey("gotquestion_id", true)]
    public int GotQuestionId { get; set; }
}