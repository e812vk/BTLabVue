namespace webapi.Data
{
    public class DataOperationResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? Args { get; set; }
        public DataOperationResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
        public DataOperationResult(bool isSuccess, object args)
        {
            IsSuccess = isSuccess;
            Args = args;
        }
    }
}
