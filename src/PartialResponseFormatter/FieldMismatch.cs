namespace PartialResponseFormatter
{
    public class FieldMismatch
    {
        public FieldMismatch(string fieldPath, string message)
        {
            FieldPath = fieldPath;
            Message = message;
        }
        
        public string FieldPath { get; }
        public string Message { get; set; }
    }
}