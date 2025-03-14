namespace Task1___Banking_Service.Models;

public class Transaction
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } 
    public Dictionary<string, string> DescriptionTranslations { get; set; } // Multi-language support
}