namespace PartialResponseFormatter
{
    public class Field
    {
        public Field()
        {
            Fields = new Field[0];
        }

        //todo: to lowercase?
        public string Name { get; set; }
        public Field[] Fields { get; set; }
    }
}