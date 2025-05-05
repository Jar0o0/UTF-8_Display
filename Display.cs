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
        public ConsoleColor color;

        public DisplayUpdateRequest(Point p0, string character, ConsoleColor color = ConsoleColor.White)
        {
            this.x = p0.x;
            this.y = p0.y;
            this.character = character;
            this.color = color;
        }
    }

    public struct Line
    {
        public Point p0;
        public Point p1;

        public Line(Point p0, Point p1)
        {
            this.p0 = p0;
            this.p1 = p1;
        }

        public Rectangle GetBoundingBox()
        {
            int xMax = Math.Max(p0.x, p1.x);
            int xMin = Math.Min(p0.x, p1.x);
            int yMax = Math.Max(p0.y, p1.y);
            int yMin = Math.Min(p0.y, p1.y);

            return new Rectangle(new Point(xMin, yMax), new Point(xMax, yMin));
        }
    }

    public struct Triangle
    {
        public Point p0;
        public Point p1;
        public Point p2;

        public Triangle(Point p0, Point p1, Point p2)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
        }

        public Rectangle GetBoundingBox()
        {
            int xMax = Math.Max(p0.x, Math.Max(p1.x, p2.x));
            int xMin = Math.Min(p0.x, Math.Min(p1.x, p2.x));
            int yMax = Math.Max(p0.y, Math.Max(p1.y, p2.y));
            int yMin = Math.Min(p0.y, Math.Min(p1.y, p2.y));

            return new Rectangle(new Point(xMin, yMax), new Point(xMax, yMin));
        }
    }
    public struct Rectangle
    {
        public Point p0;
        public Point p1;

        public Rectangle(Point p0, Point p1)
        {
            this.p0 = p0;
            this.p1 = p1;
        }
    }

    public struct Quad
    {
        public Point p0;
        public Point p1;
        public Point p2;
        public Point p3;

        public Quad(Point p0, Point p1, Point p2, Point p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        public Rectangle GetBoundingBox()
        {
            int xMax = Math.Max(p0.x, Math.Max(p1.x, Math.Max(p2.x, p3.x)));
            int xMin = Math.Min(p0.x, Math.Min(p1.x, Math.Min(p2.x, p3.x)));
            int yMax = Math.Max(p0.y, Math.Max(p1.y, Math.Max(p2.y, p3.y)));
            int yMin = Math.Min(p0.y, Math.Min(p1.y, Math.Min(p2.y, p3.y)));

            return new Rectangle(new Point(xMin, yMax), new Point(xMax, yMin));
        }
    }

    public struct Circle
    {
        public Point center;
        public int radius;

        public Circle(Point p0, int radius)
        {
            this.center = p0;
            this.radius = radius;
        }

        public Rectangle GetBoundingBox()
        {
            int xMax = center.x + radius;
            int xMin = center.x - radius;
            int yMax = center.y + radius;
            int yMin = center.y - radius;

            return new Rectangle(new Point(xMin, yMax), new Point(xMax, yMin));
        }
    }

    public struct Frame
    {
        public string[,] characters;
        public ConsoleColor[,] colors;

        public Frame(string[,] sourceChars, ConsoleColor[,] sourceColors)
        {
            int width = sourceChars.GetLength(0);
            int height = sourceChars.GetLength(1);
            characters = new string[width, height];
            colors = new ConsoleColor[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    characters[x, y] = sourceChars[x, y];
                    colors[x, y] = sourceColors[x, y];
                }
            }
        }
    }

    public class Display
    {
        public Point resolution;
        public List<DisplayUpdateRequest> requests = new List<DisplayUpdateRequest>();
        public string[,] frameBuffer;
        public ConsoleColor[,] colorBuffer;
        public string emptyCharacter = ".";
        public ConsoleColor baseColor;

        /// <summary>
        /// Updates the display with data in request buffer.
        /// </summary>
        public void UpdateDisplay()
        {
            ClearFrame();

            foreach(DisplayUpdateRequest req in requests)
            {
                frameBuffer[req.x, req.y] = req.character;
                colorBuffer[req.x, req.y] = req.color;
            }

            for (int y = 0; y < resolution.y; y++)
            {
                for (int x = 0; x < resolution.x; x++)
                {
                    if (colorBuffer[x, y] != baseColor) Console.ForegroundColor = colorBuffer[x, y];
                    Console.Write(frameBuffer[x, y] + " ");
                    Console.ForegroundColor = baseColor;
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
            Console.Clear();
            for (int y = 0; y < resolution.y; y++)
            {
                for (int x = 0; x < resolution.x; x++)
                {
                    frameBuffer[x, y] = emptyCharacter;
                    colorBuffer[x, y] = baseColor;
                }
            }
        }

        /// <summary>
        /// Used to configure some settings of the display.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="emptyCharacter"></param>
        public void DisplayConfig(ConsoleColor color = ConsoleColor.White, string emptyCharacter = ".")
        {
            baseColor = color;
            Console.ForegroundColor = color;
            this.emptyCharacter = emptyCharacter;
            Console.InputEncoding = Encoding.UTF8;
            UpdateDisplay();
        }

        /// <summary>
        /// Send a single request to the request buffer (draws a single point).
        /// </summary>
        /// <param name="request"></param>
        public void MakeRequest(DisplayUpdateRequest request)
        {
            if (request.x >= resolution.x || request.y >= resolution.y || request.x < 0 || request.y < 0) return;
            requests.Add(request);
        }

        public void DrawLine(Line line, string character, ConsoleColor color = ConsoleColor.White)
        {
            MakeRequest(new DisplayUpdateRequest(line.p0, character, color));
            MakeRequest(new DisplayUpdateRequest(line.p1, character, color));

            int dx = Math.Abs(line.p1.x - line.p0.x);
            int dy = Math.Abs(line.p1.y - line.p0.y);

            int sx = line.p0.x < line.p1.x ? 1 : -1;
            int sy = line.p0.y < line.p1.y ? 1 : -1;

            int err = dx - dy;

            Point current = new Point(line.p0.x, line.p0.y);

            while (true)
            {
                if (!(current.x == line.p0.x && current.y == line.p0.y) && !(current.x == line.p1.x && current.y == line.p1.y))
                {
                    MakeRequest(new DisplayUpdateRequest(current, character, color));
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


        public void DrawRectangle(Rectangle rect, string character ,bool fill, ConsoleColor color = ConsoleColor.White)
        {
            int xMax = Math.Max(rect.p0.x, rect.p1.x);
            int xMin = Math.Min(rect.p0.x, rect.p1.x);
            int yMax = Math.Max(rect.p0.y, rect.p1.y);
            int yMin = Math.Min(rect.p0.y, rect.p1.y);

            Point point1 = new Point(xMin, yMin);
            Point point2 = new Point(xMax, yMin);
            Point point3 = new Point(xMax, yMax);
            Point point4 = new Point(xMin, yMax);

            DrawLine(new Line(point1, point2), character, color);
            DrawLine(new Line(point2, point3), character, color);
            DrawLine(new Line(point3, point4), character, color);
            DrawLine(new Line(point4, point1), character, color);

            if (fill)
            {
                for (int y = yMin + 1; y < yMax; y++)
                {
                    for (int x = xMin + 1; x < xMax; x++)
                    {
                        MakeRequest(new DisplayUpdateRequest(new Point(x, y), character, color));
                    }
                }
            }
        }

        public void DrawTriangle(Triangle tri, string character ,bool fill, ConsoleColor color = ConsoleColor.White)
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
                            MakeRequest(new DisplayUpdateRequest(new Point(x, y), character, color));
                        }
                    }
                }
            }

            DrawLine(new Line(tri.p0, tri.p1), character, color);
            DrawLine(new Line(tri.p1, tri.p2), character, color);
            DrawLine(new Line(tri.p2, tri.p0), character, color);
        }

        public void DrawQuad(Quad quad, string character ,bool fill, ConsoleColor color = ConsoleColor.White)
        {
            DrawLine(new Line(quad.p0, quad.p1), character, color);
            DrawLine(new Line(quad.p1, quad.p2), character, color);
            DrawLine(new Line(quad.p2, quad.p3), character, color);
            DrawLine(new Line(quad.p3, quad.p0), character, color);

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
                            MakeRequest(new DisplayUpdateRequest(new Point(x, y), character, color));
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
                            MakeRequest(new DisplayUpdateRequest(new Point(x, y), character, color));
                        }
                    }
                }
            }
        }

        public Display(Point resolution)
        {
            this.resolution = resolution;
            frameBuffer = new string[resolution.x, resolution.y];
            colorBuffer = new ConsoleColor[resolution.x, resolution.y];
        }

        /// <summary>
        /// Draws text on the display starting at position p0.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="text"></param>
        public void DrawText(Point p0, string text, ConsoleColor color = ConsoleColor.White)
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
                    MakeRequest(new DisplayUpdateRequest(new Point(p0.x + index, p0.y + lineOffset), character.ToString(), color));
                    index++;
                }
                
            }
        }

        public void DrawCircle(Circle cir, string character, bool fill, ConsoleColor color = ConsoleColor.White)
        {
            int x = cir.radius;
            int y = 0;
            int decisionOver2 = 1 - x;

            while (y <= x)
            {
                if (fill)
                {
                    // Draw horizontal lines between symmetric points
                    FillCircleLine(cir.center, x, y, character, color);
                    FillCircleLine(cir.center, y, x, character, color);
                }
                else
                {
                    PlotCirclePoints(cir.center, x, y, character, color);
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

        public void LoadFrame(Frame frame)
        {
            for (int y = 0; y < resolution.y - 1; y++)
            {
                for (int x = 0; x < resolution.x - 1; x++)
                {
                    MakeRequest(new DisplayUpdateRequest(new Point(x, y), frame.characters[x, y], frame.colors[x, y]));
                }
            }
        }

        private void FillCircleLine(Point center, int x, int y, string character, ConsoleColor color = ConsoleColor.White)
        {
            for (int i = center.x - x; i <= center.x + x; i++)
            {
                MakeRequest(new DisplayUpdateRequest(new Point(i, center.y + y), character, color));
                MakeRequest(new DisplayUpdateRequest(new Point(i, center.y - y), character, color));
            }
        }

        private void PlotCirclePoints(Point center, int x, int y, string character, ConsoleColor color = ConsoleColor.White)
        {
            MakeRequest(new DisplayUpdateRequest(new Point(center.x + x, center.y + y), character, color));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x - x, center.y + y), character, color));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x + x, center.y - y), character, color));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x - x, center.y - y), character, color));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x + y, center.y + x), character, color));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x - y, center.y + x), character, color));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x + y, center.y - x), character, color));
            MakeRequest(new DisplayUpdateRequest(new Point(center.x - y, center.y - x), character, color));
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
