using Dapper;
using DecidioTestExcersice.Models;
using Npgsql;
using System.Data;

namespace DecidioTestExcersice.Repositiories
{
    public class MailAddressRepository
    {
        private readonly string connectionString;

        public MailAddressRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<MailAddressEntry>> GetEntriesByEmail(IEnumerable<string> emails)
        {
            using (var connection = await this.GetConnection().ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction())
            {
                IEnumerable<string> emailsNorm = emails.Select(email => $"\'{email.ToUpperInvariant()}\'").ToList();
                string emailsList = string.Join(',', emailsNorm);

                IEnumerable<MailAddressEntry> result = await connection.QueryAsync<MailAddressEntry>(
                    $"SELECT email, lastsent FROM emails WHERE email in ({emailsList})",
                    transaction: transaction
                ).ConfigureAwait(false);

                return result;
            }
        }

        public async Task UpdateMailEntryTimestamp(string email)
        {
            var now = DateTime.UtcNow;
            using (var connection = await this.GetConnection().ConfigureAwait(false))
            using (var transaction = connection.BeginTransaction())
            {
                await connection.ExecuteAsync(
                    $"INSERT INTO emails (email, lastsent) VALUES (\'{email.ToUpperInvariant()}\', :lastsent) ON CONFLICT (email) DO UPDATE SET lastsent = :lastsent",
                    transaction: transaction,
                    param: new
                    {
                        lastsent = now,
                    }
                ).ConfigureAwait(false);
                transaction.Commit();
            }
        }

        private async Task<IDbConnection> GetConnection()
        {
            var connection = new NpgsqlConnection(this.connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
