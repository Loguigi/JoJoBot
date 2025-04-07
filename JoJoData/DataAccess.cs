using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using Dapper;
using Microsoft.Data.SqlClient;
using static Dapper.SqlMapper;

namespace JoJoData;

public class DataAccess 
{
	public async Task<List<T>> GetData<T>(string sp, params dynamic[] parameters) 
	{
		try 
		{
			if (string.IsNullOrEmpty(_connectionString))
			{
				throw new Exception("Connection string is null or empty");
			}

			await using SqlConnection cnn = new(_connectionString);
			DynamicParameters param = new(parameters);
			param.Add("@Status", null, DbType.Int32, ParameterDirection.Output);
			param.Add("@Message", null, DbType.String, ParameterDirection.Output, 500);
			var data = await cnn.QueryAsync<T>(sp, param, commandType: CommandType.StoredProcedure);
			
			ResultArgs result = new(param.Get<int>("@Status"), param.Get<string>("@Message"));
			if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
			
			return data.ToList();
		} 
		catch (Exception ex) 
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	public async Task SaveData(string sp, params dynamic[] parameters)
	{
		try 
		{
			if (string.IsNullOrEmpty(_connectionString)) 
			{
				throw new Exception("Connection string is null or empty");
			}
			
			await using SqlConnection cnn = new(_connectionString);
			DynamicParameters param = new(parameters);
			param.Add("@Status", null, DbType.Int32, ParameterDirection.Output);
			param.Add("@Message", null, DbType.String, ParameterDirection.Output, 500);
			await cnn.ExecuteAsync(sp, param, commandType: CommandType.StoredProcedure);

			ResultArgs result = new(param.Get<int>("@Status"), param.Get<string>("@Message"));
			if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
		} 
		catch (Exception ex) 
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	private readonly string? _connectionString = Config.ConnectionString;
}