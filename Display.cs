using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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
            requests.Add(request);
        }

        /// <summary>
        /// Draws a line between points p0 and p1.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="character"></param>
        public void DrawLine(Point p0, Point p1, string character)
        {
            MakeRequest(new DisplayUpdateRequest(p0, character));
            MakeRequest(new DisplayUpdateRequest(p1, character));

            int dx = Math.Abs(p1.x - p0.x);
            int dy = Math.Abs(p1.y - p0.y);

            int sx = p0.x < p1.x ? 1 : -1;
            int sy = p0.y < p1.y ? 1 : -1;

            int err = dx - dy;

            Point current = new Point(p0.x, p0.y);

            while (true)
            {
                // Skip the start and end points
                if (!(current.x == p0.x && current.y == p0.y) && !(current.x == p1.x && current.y == p1.y))
                {
                    MakeRequest(new DisplayUpdateRequest(current, character));
                }

                if (current.x == p1.x && current.y == p1.y)
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

        /// <summary>
        /// Draws a rectangle with opposite corners p0 and p1.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="renderWalls"></param>
        /// <param name="wallCharacter"></param>
        /// <param name="fill"></param>
        /// <param name="fillCharacter"></param>
        public void DrawRectangle(Point p0, Point p1, bool renderWalls ,string wallCharacter, bool fill, string fillCharacter)
        {
            int xMax = Math.Max(p0.x, p1.x);
            int xMin = Math.Min(p0.x, p1.x);
            int yMax = Math.Max(p0.y, p1.y);
            int yMin = Math.Min(p0.y, p1.y);

            Point point1 = new Point(xMin, yMin);
            Point point2 = new Point(xMax, yMin);
            Point point3 = new Point(xMax, yMax);
            Point point4 = new Point(xMin, yMax);

            if (renderWalls)
            {
                DrawLine(point1, point2, wallCharacter);
                DrawLine(point2, point3, wallCharacter);
                DrawLine(point3, point4, wallCharacter);
                DrawLine(point4, point1, wallCharacter);
            }

            if (fill)
            {
                for (int y = yMin + 1; y < yMax; y++)
                {
                    for (int x = xMin + 1; x < xMax; x++)
                    {
                        MakeRequest(new DisplayUpdateRequest(new Point(x, y), fillCharacter));
                    }
                }
            }
        }

        /// <summary>
        /// Draws a triangle on points p0, p1, p2.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="wallCharacter"></param>
        /// <param name="fill"></param>
        /// <param name="fillCharacter"></param>
        public void DrawTriangle(Point p0, Point p1, Point p2, string wallCharacter, bool fill, string fillCharacter)
        {
            if (fill)
            {
                int yMax = Math.Max(Math.Max(p0.y, p1.y), p2.y);
                int yMin = Math.Min(Math.Min(p0.y, p1.y), p2.y);
                int xMax = Math.Max(Math.Max(p0.x, p1.x), p2.x);
                int xMin = Math.Min(Math.Min(p0.x, p1.x), p2.x);

                for (int y = yMin; y < yMax; y++)
                {
                    for (int x = xMin; x < xMax; x++)
                    {
                        if (IsInsideTriangle(p0, p1, p2, new Point(x, y)))
                        {
                            MakeRequest(new DisplayUpdateRequest(new Point(x, y), fillCharacter));
                        }
                    }
                }
            }

            DrawLine(p0, p1, wallCharacter);
            DrawLine(p1, p2, wallCharacter);
            DrawLine(p2, p0, wallCharacter);
        }

        /// <summary>
        /// Draws a quad (Points should be put clockwise)
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="wallCharacter"></param>
        public void DrawQuad(Point p0, Point p1, Point p2, Point p3, string wallCharacter)
        {
            DrawLine(p0, p1, wallCharacter);
            DrawLine(p1, p2, wallCharacter);
            DrawLine(p2, p3, wallCharacter);
            DrawLine(p3, p0, wallCharacter);
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
