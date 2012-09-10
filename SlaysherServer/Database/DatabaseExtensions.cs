using System.Data.Common;

namespace SlaysherServer.Database
{
    static class DatabaseExtensions
    {
        public static void SetParameter(this DbCommand command, string name, object value)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}
