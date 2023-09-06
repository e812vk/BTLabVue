using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.SqlClient;
using webapi.Data;

namespace webapi.Services
{
    public class DataAccessor
    {
        private readonly ILogger<DataAccessor> _logger;
        private readonly string? _connectionString;
        private readonly string? _tableName;
        private List<Record>? _allRecords;

        public DataAccessor(IConfiguration config, ILogger<DataAccessor> logger)
        {
            _logger = logger;
            _connectionString = config?.GetConnectionString("Default");
            _tableName = config?.GetSection("TableName").Value;
        }

        // Общий обработчик для CRUD SQL-запросов
        private async Task<bool> ExecuteAsync(SqlCommand sqlCommand,
            bool useReader = false,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                sqlCommand.Connection = connection;
                await connection.OpenAsync();
                
                if (useReader)
                { 
                    _allRecords = new List<Record>();
                    var reader = await sqlCommand.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            _allRecords?.Add(new Record()
                            {
                                id = await reader.GetFieldValueAsync<int>(0),
                                reason = await reader.GetFieldValueAsync<Reason>(1),
                                start_date = await reader.GetFieldValueAsync<DateOnly>(2),
                                duration = await reader.GetFieldValueAsync<int>(3),
                                discounted = await reader.GetFieldValueAsync<bool>(4),
                                description = await reader.GetFieldValueAsync<string>(5)
                            });
                        }
                    }
                    await reader.CloseAsync();
                }
                else
                {
                    await sqlCommand.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now} Method {memberName} got an exception:\r\n{ex.Message}");

                return false;
            }
        }

        // Создатель параметров в SQL-запросах, чтобы не допустить SQL-инъекций
        private static SqlCommand GetParametrizedSqlCommand(Record record, string sqlExpression)
        {
            var sqlCommand = new SqlCommand(sqlExpression);

            var sqlParameters = new SqlParameter[] {
                new SqlParameter("@reason", (int)record.reason),
                new SqlParameter("@start_date", record.start_date),
                new SqlParameter("@duration", record.duration),
                new SqlParameter("@discounted", (record.discounted ? 1 : 0)),
                new SqlParameter("@description", record.description) };

            sqlCommand.Parameters.AddRange(sqlParameters);

            return sqlCommand;
        }

        // Создание подходящей таблицы в базе данных
        public async Task<bool> CreateTableAsync()
        {
            var sqlCommand = new SqlCommand($"CREATE TABLE {_tableName}(" +
                "id INT IDENTITY(1,1) PRIMARY KEY, " +
                "reason INT NOT NULL, " +
                "start_date DATE NOT NULL, " +
                "duration INT NOT NULL, " +
                "discounted BIT NOT NULL, " +
                "description NVARCHAR(1024));");

            return await ExecuteAsync(sqlCommand);
        }

        // Добавление новой записи в базу данных
        public async Task<bool> AddRecordAsync(Record record)
        {
            var sqlExpression = $"INSERT INTO {_tableName} " +
                $"(reason, start_date, duration, discounted, description)" +
                $"VALUES (@reason, @start_date, @duration, @discounted, @description);";

            var sqlCommand = GetParametrizedSqlCommand(record, sqlExpression);

            return await ExecuteAsync(sqlCommand);
        }

        // Удаление записи из базы данных
        public async Task<bool> DeleteRecordByIdAsync(int recordId)
        {
            var sqlCommand = new SqlCommand($"DELETE FROM {_tableName} WHERE id={recordId}");

            return await ExecuteAsync(sqlCommand);
        }

        // Обновление записи в базе данных после редактирования
        public async Task<bool> EditRecordAsync(Record record)
        {
            var sqlExpression = $"UPDATE {_tableName} " +
                $"SET reason = @reason, start_date = @start_date, duration = @duration, " +
                $"discounted = @discounted, description = @description " +
                $"WHERE id = {record.id}";

            var sqlCommand = GetParametrizedSqlCommand(record, sqlExpression);

            return await ExecuteAsync(sqlCommand);
        }

        // Получение всех записей из базы данных
        public async Task<DataOperationResult> GetRecordsAsync()
        {
            var sqlCommand = new SqlCommand($"SELECT * FROM {_tableName}");
            var result = await ExecuteAsync(sqlCommand, true);
            var response = new DataOperationResult(result);
            if(response.IsSuccess && _allRecords != null) response.Args = _allRecords;

            return response;
        }
    }
}
