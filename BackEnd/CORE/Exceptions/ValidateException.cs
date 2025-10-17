namespace CORE.Exceptions;

public class ValidateException : Exception
{
    private string MsgError = string.Empty;
    public ValidateException(string error)
    {
        this.MsgError = error;
    }

    public override string Message => this.MsgError;
}
