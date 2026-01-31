using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using ProjectWeb.Models;
using ProjectWeb.Interface;

namespace ProjectWeb.Models.Repository
{
    public class ExpenseService : IExpenseService
    {
        private readonly string _connectionString = "Server=localhost;Database=FinanceTrackerDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public void AddExpense(Expense expense)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Expenses (UserId, Amount, CategoryId, DateSpent, PaymentMethod, Description) 
                               VALUES (@UserId, @Amount, @CategoryId, @DateSpent, @PaymentMethod, @Description)";
                db.Execute(sql, new
                {
                    UserId = expense.UserId,
                    CategoryId = expense.CategoryId,
                    Amount = expense.Amount,
                    DateSpent = expense.DateSpent,
                    PaymentMethod = expense.PaymentMethod,
                    Description = expense.Description
                });
            }
        }

        public void UpdateExpense(Expense expense)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Expenses 
                               SET Amount = @Amount, CategoryId = @CategoryId, DateSpent = @DateSpent, 
                                   PaymentMethod = @PaymentMethod, Description = @Description 
                               WHERE ExpenseId = @ExpenseId";
                db.Execute(sql, expense);
            }
        }

        public void DeleteExpense(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Expenses WHERE ExpenseId = @id";
                db.Execute(sql, new { id });
            }
        }

        public async Task<decimal> GetTotalMonthlyExpensesAsync(string userId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT ISNULL(SUM(Amount), 0) FROM Expenses 
                               WHERE UserId = @userId 
                               AND MONTH(DateSpent) = MONTH(GETDATE()) 
                               AND YEAR(DateSpent) = YEAR(GETDATE())";
                return await db.ExecuteScalarAsync<decimal>(sql, new { userId });
            }
        }

        public async Task<IEnumerable<Expense>> GetExpenseHistoryAsync(string userId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Expenses WHERE UserId = @userId ORDER BY DateSpent DESC";
                return await db.QueryAsync<Expense>(sql, new { userId });
            }
        }

        // Get all expense records for a specific category
        public async Task<IEnumerable<Expense>> GetExpensesByCategoryAsync(string userId, int categoryId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var now = DateTime.Now;
                var start = new DateTime(now.Year, now.Month, 1);
                var end = start.AddMonths(1);

                string sql = @"
                    SELECT *
                    FROM Expenses
                    WHERE UserId = @userId
                      AND CategoryId = @categoryId
                      AND DateSpent >= @start
                      AND DateSpent < @end
                    ORDER BY DateSpent DESC";

                return await db.QueryAsync<Expense>(sql, new { userId, categoryId, start, end });
            }
        }

        public async Task<Dictionary<int, decimal>> GetCategoryTotalsAsync(string userId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT CategoryId, SUM(Amount) as Total 
                               FROM Expenses 
                               WHERE UserId = @userId 
                               AND MONTH(DateSpent) = MONTH(GETDATE()) 
                               AND YEAR(DateSpent) = YEAR(GETDATE())
                               GROUP BY CategoryId";

                var results = await db.QueryAsync<(int CategoryId, decimal Total)>(sql, new { userId });
                return results.ToDictionary(x => x.CategoryId, x => x.Total);
            }
        }
    }
}