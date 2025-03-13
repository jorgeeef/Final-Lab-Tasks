using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MediatR;

namespace Task1___Banking_Service.Models;

public class TransactionEvent: INotification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
        
    [Required]
    public long TransactionId { get; set; } 
        
    [Required]
    public string EventType { get; set; } // "Deposit", "Withdrawal" or...
        
    [Required]
    public string Details { get; set; } 
        
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow; 
}