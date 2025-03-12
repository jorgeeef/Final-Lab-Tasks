using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task1___Banking_Service.Models;

public class Logs
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; } 

    [Required]
    public Guid RequestId { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string RequestObject { get; set; }  

    [Required]
    public string RouteURL { get; set; }  

    [Required]
    public DateTime Timestamp { get; set; }  
}