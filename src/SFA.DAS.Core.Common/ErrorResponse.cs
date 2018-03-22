namespace SFA.DAS.Core.Common
{
    public class ErrorResponse
    {
        public string Code { get; set; }
        public int? DomainExceptionId { get; set; }
        public string Message { get; set; }
    }
}
