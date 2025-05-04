using UTF8_Display;


namespace UTF_8_Display
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Display display = new Display(new Point(32, 32));
            display.DisplayConfig(ConsoleColor.Green);
            Point p0 = new Point(4, 4);
            Point p1 = new Point(20, 10);
            Point p2 = new Point(28, 25);
            Point p3 = new Point(7, 19);
            //display.DrawQuad(new Quad(p0, p1, p2, p3, "#"), true);
            //display.DrawTriangle(new Triangle(p0, p1, p2, "$"), true);
            //display.DrawRectangle(new Rectangle(p0, p2, "%"), true);
            //display.DrawCircle(new Circle(p2, 5, "^"), true);
            display.DrawText(p0, "HELLO WORLD");
            display.UpdateDisplay();

            Console.ReadKey();
        }
    }
}
