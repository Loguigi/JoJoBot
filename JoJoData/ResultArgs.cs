namespace JoJoData;

public class ResultArgs(int status, string message)
{
	public StatusCodes Status { get; set; } = (StatusCodes)status;
	public string Message { get; set; } = message;
}

public enum StatusCodes 
{
	SUCCESS = 0,
	ERROR = 1,
	NEEDS_SETUP = 2,
	UNKNOWN = 99
}