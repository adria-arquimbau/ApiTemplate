namespace ApiTemplate.Values.Api.ProblemDetails
{
    public class KeyTooLongProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        public string Key { get; set; }
        public int Length { get; set; }
    }
}
