using Postgrest.Attributes;
using Postgrest.Models;

namespace Bingo.Infrastructure.Database;

[Table("chgk.question")]
public class QuestionDb : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    
    [Column("pack_title")]
    public string PackTitle { get; set; }
    
    [Column("date")]
    public DateTime Date { get; set; }
    
    [Column("number")]
    public int Number { get; set; }
    
    [Column("text")]
    public string Text { get; set; }
    
    [Column("additional_material_text")]
    public string AdditionalMaterialText { get; set; }
    
    [Column("additional_material_picture_url")]
    public string? AdditionalMaterialPictureUrl { get; set; }
    
    [Column("answer")]
    public string Answer { get; set; }
    
    [Column("accepted_answer")]
    public string AcceptedAnswer { get; set; }
    
    [Column("not_accepted_answer")]
    public string NotAcceptedAnswer { get; set; }
    
    [Column("comment")]
    public string Comment { get; set; }
    
    [Column("note")]
    public string Note { get; set; }
    
    [Column("author")]
    public string Author { get; set; }
    
    [Column("editor")]
    public string Editor { get; set; }
    
    [Column("bingo_id")]
    public int BingoId { get; set; }
}