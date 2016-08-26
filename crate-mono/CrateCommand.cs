using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Net;
using Crate.Client.Extensions;

namespace Crate.Client
{
    public class CrateCommand : IDbCommand
    {
        private readonly CrateConnection _connection;
        private readonly CrateParameterCollection _parameters = new CrateParameterCollection();

        public string CommandText { get; set; }
        public int CommandTimeout { get; set; }

        public IDbConnection Connection
        {
            get
            {
                return _connection;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public CrateCommand(string commandText, CrateConnection connection)
        {
            CommandText = commandText;
            _connection = connection;
        }

        #region IDbCommand implementation

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public IDbDataParameter CreateParameter()
        {
            return new CrateParameter();
        }

        public int ExecuteNonQuery()
        {
            try
            {
                var task = ExecuteNonQueryAsync();
                task.Wait();
                return task.Result;
            }
            catch (AggregateException aggrEx)
            {
                aggrEx = aggrEx.Flatten();

                throw aggrEx.InnerExceptions.First();
            }
        }

        public async Task<int> ExecuteNonQueryAsync()
        {
            var resp = await ExecuteAsync();

            if (resp.Error == null)
                return resp.RowCount;

            throw new CrateException(resp.Error);
        }

        public IDataReader ExecuteReader()
        {
            var task = ExecuteReaderAsync();

            try
            {
                task.Wait();
                return task.Result;
            }
            catch (AggregateException aggrEx)
            {
                aggrEx = aggrEx.Flatten();

                throw aggrEx.InnerExceptions.First();
            }
        }

        protected async Task<SqlResponse> ExecuteAsync(int currentRetry = 0)
        {
            var server = _connection.NextServer();
            try
            {
                return await SqlClient.ExecuteAsync(
                        server.SqlUri(),
                        new SqlRequest(CommandText, _parameters.Select(x => x.Value).ToArray()));
            }
            catch (WebException)
            {
                _connection.MarkAsFailed(server);
                if (currentRetry > 3)
                {
                    return "Connection retry count exceeded".ToSqlResponse();
                }

                return await ExecuteAsync(currentRetry++);
            }
        }

        public async Task<IDataReader> ExecuteReaderAsync()
        {
            var resp = await ExecuteAsync();

            if (resp.Error == null)
                return new CrateDataReader(await ExecuteAsync());

            throw new CrateException(resp.Error);
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return ExecuteReader();
        }

        public object ExecuteScalar()
        {
            using (var reader = ExecuteReader())
            {
                reader.Read();
                return reader[0];
            }
        }

        public void Prepare()
        {
        }


        public CommandType CommandType { get; set; }

        public IDataParameterCollection Parameters => _parameters;

        public IDbTransaction Transaction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
        }

        #endregion
    }
}

