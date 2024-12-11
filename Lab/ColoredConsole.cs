namespace ConsoleClient
{
    public static class ColoredConsole
    {
        public static void WriteLine(string message, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            SetColor(foregroundColor, backgroundColor);

            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Write(string message, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            SetColor(foregroundColor, backgroundColor);

            Console.Write(message);
            Console.ResetColor();
        }

        public static void WriteErrorLine(string message, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {            
            Write("ERROR: ", ConsoleColor.Black, ConsoleColor.Red);

            SetColor(foregroundColor, backgroundColor);

            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void SetColor(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
        }
    }
}
