public static class GlobalUtils
{
    public static void ConsoleWriteColor(ConsoleColor num, string message)
    {
        Console.ForegroundColor = num;
        Console.Write(message + " ");
        Console.ResetColor();
    }
}
