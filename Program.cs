using UTF8_Display;


namespace UTF_8_Display
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Display display = new Display(new Point(28, 28));
            display.DisplayConfig(ConsoleColor.White, ".");
            Point p0 = new Point(4, 4);
            Point p1 = new Point(20, 10);
            Point p2 = new Point(23, 25);
            Point p3 = new Point(7, 19);
            Point p4 = new Point(13, 13);


            Line line = new Line(p0, p1);
            Triangle tri = new Triangle(p0, p3, p1);
            Quad quad = new Quad(p0, p1, p2, p3);
            Circle circle = new Circle(p4, 10);
            Rectangle rect = new Rectangle(p0, p1);

            display.DrawRectangle(rect, "&", true, ConsoleColor.Red, 1);
            display.DrawRectangle(rect, "&", true, ConsoleColor.Blue, 0);
            display.UpdateDisplay();

            Console.ReadKey();
        }
    }
}
