namespace RESTServiceProject.Models
{
    public class BadRequest
    {
        public string Message { get; set; }
        public string? FieldName { get; set; }
        public ErrorType DBCode { get; set; }
    }

    public enum ErrorType
    {
        Nil = 0,
        MissingField,
        InvalidData,
        NullOrEmptyField,
    }
}
