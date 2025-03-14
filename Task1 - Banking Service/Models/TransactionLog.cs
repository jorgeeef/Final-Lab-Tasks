using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task1___Banking_Service.Models;

public class TransactionLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public long AccountId { get; set; }
    
    public string TransactionType { get; set; } 
    
    public decimal Amount { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string Status { get; set; } // (Pending, Completed, Failed)
    
    public string Details { get; set; }
    
    public Dictionary<string, string> DescriptionTranslations { get; set; }
}