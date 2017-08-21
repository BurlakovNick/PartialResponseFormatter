namespace PartialResponseFormatter
{
    public class MissingField
    {
        public MissingField(string fieldPath)
        {
            FieldPath = fieldPath;
        }
        
        public string FieldPath { get; }
    }
}