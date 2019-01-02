namespace ExpressionToSql.Dapper.UnitTests
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using MartinCostello.SqlLocalDb;

    public class TestDatabase : IDisposable
    {
        private bool _disposed;
        private TemporarySqlLocalDbInstance _instance;
        private readonly string _connectionString;

        private TestDatabase(TemporarySqlLocalDbInstance instance, string pathToSeedDb)
        {
            _instance = instance;

            var pathToModel = Path.Combine(SqlLocalDbApi.GetInstancesFolderPath(), instance.Name, Path.GetFileName(pathToSeedDb));
            File.Copy(pathToSeedDb, pathToModel);

            _connectionString = GetConnectionString(instance, pathToModel);
        }

        public static TestDatabase Create(string pathToSeedDb)
        {
            var sqlLocalDbApi = new SqlLocalDbApi
            {
                StopOptions = StopInstanceOptions.KillProcess
            };
            var temporarySqlLocalDbInstance = sqlLocalDbApi.CreateTemporaryInstance(deleteFiles: true);
            return new TestDatabase(temporarySqlLocalDbInstance, pathToSeedDb);
        }

        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_connectionString);
        }

        private string GetConnectionString(ISqlLocalDbApiAdapter instance, string pathToModel)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_instance.ConnectionString);
            sqlConnectionStringBuilder.InitialCatalog = Path.GetFileNameWithoutExtension(pathToModel);
            sqlConnectionStringBuilder.IntegratedSecurity = true;
            sqlConnectionStringBuilder.AttachDBFilename = pathToModel;
            return sqlConnectionStringBuilder.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _instance.Dispose();
                _instance = null;
            }
            _disposed = true;
        }
    }
}