namespace PartialResponseFormatter
{
    public class Field
    {
        public Field()
        {
            Fields = new Field[0];
        }

        public string Name { get; set; }
        public Field[] Fields { get; set; }
    }
}