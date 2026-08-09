namespace eCommerce.Application.Exceptions
{
    public class ConflictException : Exception
    {
        public string Code { get; }
        public ConflictException(string message, string code = "CONFLICT_RULE_EXCEPTION") : base(message)
        {
            Code = code;
        }
    }
}
