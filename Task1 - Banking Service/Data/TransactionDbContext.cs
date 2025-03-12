﻿using Microsoft.EntityFrameworkCore;
using Task1___Banking_Service.Models;

namespace Task1___Banking_Service.Data;

public class TransactionDbContext: DbContext
{
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options) { }

    public DbSet<TransactionLog> TransactionLogs { get; set; }
    public DbSet<AccountTransaction> AccountTransaction { get; set; }
    
    public DbSet<Logs> Logs { get; set; } 
}