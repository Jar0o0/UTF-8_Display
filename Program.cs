using UTF8_Display;


namespace UTF_8_Display
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Display display = new Display(new Point(28, 28));
            display.DisplayConfig(ConsoleColor.DarkCyan);
            display.DrawTriangle(new Point(4, 5), new Point(15, 10), new Point(7, 20), "#", true, "%");
            //display.DrawQuad(new Point(4, 5), new Point(16, 8), new Point(20, 24), new Point(7, 21), "#");


            display.UpdateDisplay();

            Console.ReadKey();
        }
    }
}
