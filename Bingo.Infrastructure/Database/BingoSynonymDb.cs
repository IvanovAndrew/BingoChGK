using Postgrest.Attributes;
using Postgrest.Models;

namespace Bingo.Infrastructure.Database;

[Table("chgk.bingo_synonyms")]
public class BingoSynonymDb : BaseModel
{
    [PrimaryKey("bingo_id", true)]
    public int BingoId { get; set; }
    
    [PrimaryKey("synonym", true)]
    public string Synonym { get; set; }
}