namespace GraylogInitializer.Console.Dtos.GetInputs;

public class GetInputsDto
{
    public int Total { get; set; }
    public List<InputDto> Inputs { get; set; } = new();

    public class InputDto
    {
        public string Type { get; set; }
        public AttributesDto Attributes { get; set; }

        public class AttributesDto
        {
            public int Port { get; set; }
        }
    }
}