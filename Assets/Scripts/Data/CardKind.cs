namespace Data
{
    public class CardKind
    {
        public int value = 0;

        public override string ToString()
        {
            if (value < 0 || value >= 37)
            {
                throw new System.Exception($"Error CardKind{value}");
            }
            return value switch
            {
                < 10 => $"{value - 00}m",
                < 20 => $"{value - 10}p",
                < 30 => $"{value - 20}s",
                _ => $"{value - 30}z",
            };
        }
    }
}