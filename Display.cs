using System.Text;


namespace UTF8_Display
{
    public struct Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point GetMidpoint(Point p0, Point p1)
        {
            return new Point(p0.x + p1.x / 2, p0.y + p1.y / 2);
        }

        public static int GetDistance(Point p0, Point p1)
        {
            int d1 = (int)Math.Pow((double)p1.x - p0.x, 2);
            int d2 = (int)Math.Pow((double)p1.y - p0.y, 2);
            return (int)Math.Sqrt(d1 + d2);
        }

        public static Point operator +(Point p0, Point p1)
        {
            return new Point(p0.x + p1.x, p0.y + p1.y);
        }
        public static Point operator -(Point p0, Point p1)
        {
            return new Point(p0.x - p1.x, p0.y - p1.y);
        }
        public static Point operator *(Point p0, Point p1)
        {
            return new Point(p0.x * p1.x, p0.y * p1.y);
        }
        public static Point operator /(Point p0, Point p1)
        {
            return new Point(p0.x / p1.x, p0.y / p1.y);
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }
    }

    public struct DisplayUpdateRequest
    {
        public int x;
        public int y;
        public string character;

        public DisplayUpdateRequest(Point p0, string character)
        {
            this.x = p0.x;
            this.y = p0.y;
            this.character = character;
        }
    }

    public struct Line
    {
        public Point p0;
        public Point p1;
        public string character;

        public Line(Point p0, Point p1, string character)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.character = character;
        }
    }

    public struct Triangle
    {
        public Point p0;
        public Point p1;
        public Point p2;
        public string character;

        public Triangle(Point p0, Point p1, Point p2, string character)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.character = character;
        }
    }
    public struct Rectangle
    {
        public Point p0;
        public Point p1;
        public string character;

        public Rectangle(Point p0, Point p1, string character)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.character = character;
        }
    }

    public struct Quad
    {
        public Point p0;
        public Point p1;
        public Point p2;
        public Point p3;
        public string character;

        public Quad(Point p0, Point p1, Point p2, Point p3, string character)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.character = character;
        }
    }

    public struct Circle
    {
        public Point center;
        public int radius;
        public string character;

        public Circle(Point p0, int radius, string character)
        {
            this.center = p0;
            this.radius = radius;
            this.character = character;
        }
    }

    public class Display
    {
        public Point resolution;
        public List<DisplayUpdateRequest> requests = new List<DisplayUpdateRequest>();
        public string[,] frameBuffer;
        public string emptyCharacter = ".";

        /// <summary>
        /// Updates the display with data in request buffer.
        /// </summary>
        public void UpdateDisplay()
        {
            Console.Clear();
            ClearFrame();

            foreach(DisplayUpdateRequest req in requests)
            {
                frameBuffer[req.x, req.y] = req.character;
            }

            for (int y = 0; y < resolution.y; y++)
            {
                for (int x = 0; x < resolution.x; x++)
                {
                    Console.Write(frameBuffer[x, y] + " ");
                    if (x == resolution.x - 1) Console.WriteLine("");
                }
            }

            requests = new List<DisplayUpdateRequest>();
        }

        /// <summary>
        /// Fills the entire display with emptyCharacters specified in DisplayConfig();
        /// </summary>
        public void ClearFrame()
        {
            for (int y = 0; y < resolution.y; y++)
            {
                for (int x = 0; x < resolution.x; x++)
                {
                    frameBuffer[x, y] = emptyCharacter;
                }
            }
        }

        /// <summary>
        /// Used to configure some settings of the display.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="emptyCharacter"></param>
        public void DisplayConfig(ConsoleColor color, string emptyCharacter = ".")
        {
            Console.ForegroundColor = color;
            this.emptyCharacter = emptyCharacter;
            Console.InputEncoding = Encoding.UTF8;
        }

        /// <summary>
        /// Send a single request to the request buffer (draws a single point).
        /// </summary>
        /// <param name="request"></param>
        public void MakeRequest(DisplayUpdateRequest request)
        {
            if (request.x > resolution.x || request.y > resolution.y) return;
            requests.Add(request);
        }

        public void DrawLine(Line line)
        {
            MakeRequest(new DisplayUpdateRequest(line.p0, line.character));
            MakeRequest(new DisplayUpdateRequest(line.p1, line.character));

            int dx = Math.Abs(line.p1.x - line.p0.x);
            int dy = Math.Abs(line.p1.y - line.p0.y);

            int sx = line.p0.x < line.p1.x ? 1 : -1;
            int sy = line.p0.y < line.p1.y ? 1 : -1;

            int err = dx - dy;

            Point current = new Point(line.p0.x, line.p0.y);

            while (true)
            {
                // Skip the start and end points
                if (!(current.x == line.p0.x && current.y == line.p0.y) && !(current.x == line.p1.x && current.y == line.p1.y))
                {
                    MakeRequest(new DisplayUpdateRequest(current, line.character));
                }

                if (current.x == line.p1.x && current.y == line.p1.y)
                    break;

                int e2 = 2 * err;

                if (e2 > -dy)
                {
                    err -= dy;
                    current.x += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    current.y += sy;
                }
            }
        }


        public void DrawRectangle(Rectangle rect, bool fill)
        {
            int xMax = Math.Max(rect.p0.x, rect.p1.x);
            int xMin = Math.Min(rect.p0.x, rect.p1.x);
            int yMax = Math.Max(rect.p0.y, rect.p1.y);
            int yMin = Math.Min(rect.p0.y, rect.p1.y);

            Point point1 = new Point(xMin, yMin);
            Point point2 = new Point(xMax, yMin);
            Point point3 = new Point(xMax, yMax);
            Point point4 = new Point(xMin, yMax);

            DrawLine(new Line(point1, point2, rect.character));
            DrawLine(new Line(point2, point3, rect.character));
            DrawLine(new Line(point3, point4, rect.character));
            DrawLine(new Line(point4, point1, rect.character));

            if (fill)
            {
                for (int y = yMin + 1; y < yMax; y++)
                {
                    for (int x = xMin + 1; x < xMax; x++)
                    {
                        MakeRequest(new DisplayUpdateRequest(new Point(x, y), rect.character));
                    }
                }
            }
        }

        public void DrawTriangle(Triangle tri, bool fill)
        {
            if (fill)
            {
                int yMax = Math.Max(Math.Max(tri.p0.y, tri.p1.y), tri.p2.y);
                int yMin = Math.Min(Math.Min(tri.p0.y, tri.p1.y), tri.p2.y);
                int xMax = Math.Max(Math.Max(tri.p0.x, tri.p1.x), tri.p2.x);
                int xMin = Math.Min(Math.Min(tri.p0.x, tri.p1.x), tri.p2.x);

                for (int y = yMin; y < yMax; y++)
                {
                    for (int x = xMin; x < xMax; x++)
                    {
                        if (IsInsideTriangle(tri.p0, tri.p1, tri.p2, new Point(x, y)))
                        {
                            MakeRequest(new DisplayUpdateRequest(new Point(x, y), tri.character));
                        }
                    }
                }
            }

            DrawLine(new Line(tri.p0, tri.p1, tri.character));
            DrawLine(new Line(tri.p1, tri.p2, tri.character));
            DrawLine(new Line(tri.p2, tri.p0, tri.character));
        }

        public void DrawQuad(Quad quad, bool fill)
        {
            DrawLine(new Line(quad.p0, quad.p1, quad.character));
            DrawLine(new Line(quad.p1, quad.p2, quad.character));
            DrawLine(new Line(quad.p2, quad.p3, quad.character));
            DrawLine(new Line(quad.p3, quad.p0, quad.character));

            if (fill)
            {
                int xMax = Math.Max(quad.p0.x, Math.Max(quad.p1.x, quad.p2.x));
                int xMin = Math.Min(quad.p0.x, Math.Min(quad.p1.x, quad.p2.x));
                int yMax = Math.Max(quad.p0.y, Math.Max(quad.p1.y, quad.p2.y));
                int yMin = Math.Min(quad.p0.y, Math.Min(quad.p1.y, quad.p2.y));

                for (int y = yMin; y < yMax; y++)
                {
                    for (int x = xMin; x < xMax; x++)
                    {
                        if(IsInsideTriangle(quad.p0, quad.p1, quad.p2, new Point(x, y)))
                        {
                            MakeRequest(new DisplayUpdateRequest(new Point(x, y), quad.character));
                        }
                    }
                }

                xMax = Math.Max(quad.p2.x, Math.Max(quad.p3.x, quad.p0.x));
                xMin = Math.Min(quad.p2.x, Math.Min(quad.p3.x, quad.p0.x));
                yMax = Math.Max(quad.p2.y, Math.Max(quad.p3.y, quad.p0.y));
                yMin = Math.Min(quad.p2.y, Math.Min(quad.p3.y, quad.p0.y));

                for (int y = yMin; y < yMax; y++)
                {
                    for (int x = xMin; x < xMax; x++)
                    {
                        if (IsInsideTriangle(quad.p2, quad.p3, quad.p0, new Point(x, y)))
                        {
                            MakeRequest(new DisplayUpdateRequest(new Point(x, y), quad.character));
                        }
                    }
                }
            }
        }

        public Display(Point resolution)
        {
            this.resolution = resolution;
            frameBuffer = new string[resolution.x, resolution.y];
        }

        /// <summary>
        /// Draws text on the display starting at position p0.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="text"></param>
        public void DrawText(Point p0, string text)
        {
            char[] chars = text.ToCharArray();

            int index = 0;
            int lineOffset = 0;
            foreach(char character in chars)
            {
                if (p0.x + index == resolution.x)
                {
                    lineOffset++;
                    index = -p0.x;
                }

                if(p0.x + index <= resolution.x - 1 && p0.y <= resolution.y - 1)
                {
                    MakeRequest(new DisplayUpdateRequest(new Point(p0.x + index, p0.y + lineOffset), character.ToString()));
                    index++;
                }
                
            }
        }

        public void DrawCircle(Circle cir, bool fill)
        {
            int x = cir.radius;
            int y = 0;
            int decisionOver2 = 1 - x;

            while (y <= x)
            {
                if (fill)
                {
                    // Draw horizontal lines between symmetric points
                    FillCircleLine(cir.center, x, y, cir.character);
                    FillCircleLine(cir.center, y, x, cir.character);
                }
                else
                {
                    PlotCirclePoints(cir.center, x, y, cir.character);
                }

                y++;

                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2 * y + 1;
                }
                else
                {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;
                }
            }
        }

        private void FillCircleLine(Point center, int x, int y, string character)
        {
            for (int i = center.x - x; i <= center.x + x; i++)
            {
                MakeRequest(new DisplayUpdateRequest(new Point(i, center.y + y), character));
                MakeRequest(new DisplayUpdateRequest(new Point(i, center.y - y), character));
            }
        }

        private void PlotCirclePoints(Point center, int x, int y, string character)
        {
            MakeRequest(new DisplayUpdateRequest(new Point(center.x + x, center.y + y), character));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x - x, center.y + y), character));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x + x, center.y - y), character));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x - x, center.y - y), character));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x + y, center.y + x), character));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x - y, center.y + x), character));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x + y, center.y - x), character));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x - y, center.y - x), character));
        }

        /// <summary>
        /// Calculates barycentric coordinates for a point in a triangle p0, p1, p2.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public double[] CalculateBarycentric(Point p0, Point p1, Point p2, Point point)
        {
            double denominator = ((p1.y - p2.y) * (p0.x - p2.x) + (p2.x - p1.x) * (p0.y - p2.y));

            if (denominator == 0)
                return [-1, -1, -1];

            double alpha = ((p1.y - p2.y) * (point.x - p2.x) + (p2.x - p1.x) * (point.y - p2.y)) / denominator;
            double beta = ((p2.y - p0.y) * (point.x - p2.x) + (p0.x - p2.x) * (point.y - p2.y)) / denominator;
            double gamma = 1.0 - alpha - beta;

            return [alpha, beta, gamma];
        }

        /// <summary>
        /// Checks if the point is in the triangle p0, p1, p2.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="pointToCheck"></param>
        /// <returns></returns>
        public bool IsInsideTriangle(Point p0, Point p1, Point p2, Point pointToCheck)
        {
            double[] coorinates = CalculateBarycentric(p0, p1, p2, pointToCheck);

            return coorinates[0] >= 0 && coorinates[1] >= 0 && coorinates[2] >= 0;
        }
    }
}
