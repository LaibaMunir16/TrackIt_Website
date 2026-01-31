using System;
using System.Data;
using Dapper;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ProjectWeb.Models;
using ProjectWeb.Interface;
using System.Threading.Tasks;

namespace ProjectWeb.Models.Repository
{
    public class IncomeService : IIncomeService
    {
        private readonly string _connectionString = "Server=localhost;Database=FinanceTrackerDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public void InsertIncome(Income income)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Remove the 'if count == 0' check entirely
                string insertSql = @"
                    INSERT INTO Incomes 
                    (UserID, DateReceived, Amount, Source, Description)
                    VALUES 
                    (@UserID, @DateReceived, @Amount, @Source, @Description)";

                db.Execute(insertSql, income);
            }
        }

        public async Task<decimal> GetCurrentMonthIncomeAsync(string userId)
        {
            const string sql = @"
                SELECT ISNULL(SUM(Amount), 0)
                FROM Incomes
                WHERE UserID = @UserID
                AND MONTH(DateReceived) = MONTH(GETDATE())
                AND YEAR(DateReceived) = YEAR(GETDATE());";

            await using var conn = new SqlConnection(_connectionString);
            return await conn.ExecuteScalarAsync<decimal>(sql, new { UserID = userId });
        }
        
        public void UpsertIncome(Income income)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"
                    IF EXISTS (SELECT 1 FROM Incomes 
                               WHERE UserID = @UserID 
                               AND MONTH(DateReceived) = MONTH(@DateReceived) 
                               AND YEAR(DateReceived) = YEAR(@DateReceived))
                    BEGIN
                        UPDATE Incomes 
                        SET Amount = @Amount,
                            Source = @Source,
                            Description = @Description,
                            DateReceived = @DateReceived -- Updates to the specific day provided
                        WHERE UserID = @UserID 
                        AND MONTH(DateReceived) = MONTH(@DateReceived) 
                        AND YEAR(DateReceived) = YEAR(@DateReceived)
                    END
                    ELSE
                    BEGIN
                        INSERT INTO Incomes (UserID, DateReceived, Amount, Source, Description)
                        VALUES (@UserID, @DateReceived, @Amount, @Source, @Description)
                    END";

                db.Execute(sql, new
                {
                    income.UserID,
                    income.DateReceived,
                    income.Amount,
                    income.Source,
                    income.Description
                });
            }
        }
    }
}