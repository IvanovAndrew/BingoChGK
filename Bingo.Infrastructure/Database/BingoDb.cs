using Postgrest.Attributes;
using Postgrest.Models;

namespace Bingo.Infrastructure.Database;

[Table("chgk.bingo")]
public class BingoDb : BaseModel
{
    [PrimaryKey("id", true)]
    public int Id { get; set; }
    
    [Column("bingo")]
    public string Bingo { get; set; }
    
    [Column("description")]
    public string? Description { get; set; }
    
    [Column("instant_view_url")]
    public string? InstantViewUrl { get; set; }
}