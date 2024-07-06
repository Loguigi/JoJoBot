using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using Dapper;
using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData;
using static Dapper.SqlMapper;
namespace JoJoData.Data;

public class DataAccess 
{
	protected SqlConnection Connection => new(_connectionString);

	public virtual ResultArgs GetData<T>(string sp, DynamicParameters param, out List<T> data) 
	{
		try 
		{
			if (string.IsNullOrEmpty(_connectionString))
			{
				throw new Exception("Connection string is null or empty");
			}

			using var cnn = Connection;
			param.Add("@Status", null, DbType.Int32, ParameterDirection.Output);
			param.Add("@Message", null, DbType.String, ParameterDirection.Output, 500);
			data = cnn.Query<T>(sp, param, commandType: CommandType.StoredProcedure).ToList();

			return new ResultArgs(param.Get<int>("@Status"), param.Get<string>("@Message"));
		} 
		catch (Exception ex) 
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}
	
	public virtual ResultArgs GetMultiple(string sp, DynamicParameters param, out GridReader data) 
	{
		try
		{
			if (string.IsNullOrEmpty(_connectionString))
			{
				throw new Exception("Connection string is null or empty");
			}

			using var cnn = Connection;
			param.Add("@Status", null, DbType.Int32, ParameterDirection.Output);
			param.Add("@Message", null, DbType.String, ParameterDirection.Output, 500);
			data = cnn.QueryMultiple(sp, param, commandType: CommandType.StoredProcedure);

			return new ResultArgs(0, "Success");
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	public virtual ResultArgs SaveData(string sp, DynamicParameters param)
	{
		try 
		{
			if (string.IsNullOrEmpty(_connectionString)) 
			{
				throw new Exception("Connection string is null or empty");
			}
			
			using var cnn = Connection;
			param.Add("@Status", null, DbType.Int32, ParameterDirection.Output);
			param.Add("@Message", null, DbType.String, ParameterDirection.Output, 500);
			cnn.Execute(sp, param, commandType: CommandType.StoredProcedure);

			return new ResultArgs(param.Get<int>("@Status"), param.Get<string>("@Message"));
		} 
		catch (Exception ex) 
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	private readonly string? _connectionString = Config.ConnectionString;
}