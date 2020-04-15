using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGMParser
{
    public enum TextTokenType
    {
        Space, Symbol, Text,  // simple
        Comment, String, Number // complex
    }

    public class TextToken
    {
        public TextTokenType Type;
        public string Text;

        public TextToken(TextTokenType type, string text)
        {
            Type = type;
            Text = text;
        }

        public override string ToString()
        {
            return Type.ToString() + "  " + Text;
        }

        public bool IsNumber()
        {
            if (Type != TextTokenType.Text) return false;
            if (Type == TextTokenType.Number) return true;
            bool isNumber = false;
            foreach (var c in Text)
            {
                switch (c)
                {
                    case '-':
                    case '.':
                    case '+':
                        // nothing
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        isNumber = true;
                        break;
                    default:
                        return false;
                }
            }
            return isNumber;
        }
    }

    /// <summary>
    /// Parses raw text and convert it to logical information - tokens.
    /// </summary>
    public class TextParser
    {
        public string Text { get; set; }
        public List<TextToken> Tokens { get; set; }

        public TextParser(string text)
        {
            Text = text;
            Tokens = new List<TextToken>();
        }

        public void Parse()
        {
            string symbols = "%;(),\"";

            foreach (var c in Text)
            {
                if (c == ' ')
                {
                    Tokens.Add(new TextToken(TextTokenType.Space, " "));
                    continue;
                }
                if (symbols.IndexOf(c) != -1)
                {
                    Tokens.Add(new TextToken(TextTokenType.Symbol, "" + c));
                    continue;
                }

                if (Tokens.Count > 0 && Tokens.Last().Type == TextTokenType.Text)
                {
                    Tokens.Last().Text += "" + c;
                }
                else
                {
                    Tokens.Add(new TextToken(TextTokenType.Text, "" + c));
                }
            }
        }
    }

    /// <summary>
    /// Command consist of some set of Tokens - it stores general information.
    /// Command will be later converted to Object which will interpret general information to specialized.
    /// </summary>
    public class Command
    {
        public List<TextToken> Tokens { get; set; }

        public Command()
        {
            Tokens = new List<TextToken>();
        }
        public override string ToString()
        {
            return String.Join(" ", Tokens.Select(o => o.Text));
        }
    }

    /// <summary>
    /// Abstract class that is repsonsible for drawing object.
    /// </summary>
    public abstract class DrawObject
    {
        public abstract void Draw(PaintEventArgs e, float dx, float dy, float scalePercent);
    }

    /// <summary>
    /// Polyline object. 
    /// Stores points information and can draw polyline.
    /// </summary>
    public class PolylineObject : DrawObject
    {
        public int LineType { get; set; } // 1 solid  4 dash-dot  5 dash-dot-dot
        public float LineWidth { get; set; }
        public int LineColor { get; set; }
        public List<PointF> Points { get; set; }

        public PolylineObject(Command command, int lineType, float lineWidth, int lineColor)
        {
            LineType = lineType;
            LineWidth = lineWidth;
            LineColor = lineColor;
            Points = new List<PointF>();
            for (int i = 0; i < command.Tokens.Count; i++)
            {
                if (command.Tokens[i].Type == TextTokenType.Symbol && command.Tokens[i].Text == "(")
                {
                    //assume we have properly defined file and we dont have to check issues
                    // so in this case we can take values from constant positions
                    PointF point = new PointF((float)Convert.ToDouble(command.Tokens[i + 1].Text), (float)Convert.ToDouble(command.Tokens[i + 3].Text));
                    Points.Add(point);
                }
            }
        }

        public override void Draw(PaintEventArgs e, float dx, float dy, float scalePercent)
        {
            List<Color> colors = new List<Color>();
            colors.Add(Color.White);
            colors.Add(Color.Black);
            colors.Add(Color.Red);
            colors.Add(Color.Green);
            colors.Add(Color.Blue);
            //var color = Color.FromArgb(255, 0, 0, 0);
            var color = colors[LineColor];
            using (Pen pen = new Pen(color, LineWidth))
            {
                if (LineType == 4) pen.DashPattern = new float[2] { 10, 2 };
                if (LineType == 5) pen.DashPattern = new float[3] { 10, 2, 2 };
                for (int i = 0; i < Points.Count - 1; i++)
                {
                    var p1 = Points[i];
                    var p2 = Points[i + 1];
                    e.Graphics.DrawLine(pen, dx + p1.X * scalePercent, dy + p1.Y * scalePercent, dx + p2.X * scalePercent, dy + p2.Y * scalePercent);
                }
            }
        }
    }


    /// <summary>
    /// Main class that parses text CGM file and draw it.
    /// </summary>
    public class CGM
    {
        public List<DrawObject> Objects { get; private set; }

        public void Read(string filename)
        {
            // read file into lines
            string[] lines = File.ReadAllLines(filename);
            Read(lines);
        }

        /// <summary>
        /// Parse lines and generate Objects so we can later paint them in form
        /// </summary>
        /// <param name="lines"></param>
        public void Read(string[] lines)
        {
            // parse lines in parallel
            List<TextParser> parsers = lines.Select(o => new TextParser(o)).ToList();
            Parallel.ForEach(parsers, (parser, state) =>
                {
                    try
                    {
                        parser.Parse();
                    }
                    catch (Exception ex)
                    {
                        Debugger.Break();
                        parser.Tokens.Clear();
                    }
                });

            // merge tokens into one list to be able proceed them one by one
            List<TextToken> tokens = parsers.SelectMany(o => o.Tokens).ToList();

            // merge comments and strings ( simple version without optimization just to keep code simple)
            while (GroupCommentOrString(ref tokens))
            {
            }

            // mark text as number if there only numeric carachters in it
            Parallel.ForEach(tokens, (o) =>
            {
                if (o.IsNumber())
                {
                    o.Type = TextTokenType.Number;
                }
            });

            // get commands by grouping tokens
            List<Command> commands = new List<Command>();
            commands.Add(new Command()); // add command to be able to add tokens to it
            foreach (var token in tokens)
            {
                // skip spaces and comment
                if (token.Type == TextTokenType.Space || token.Type == TextTokenType.Comment) continue;
                //check if token ends the current command
                if (token.Type == TextTokenType.Symbol && token.Text == ";")
                {
                    // start new command
                    if (commands.Last().Tokens.Count != 0) commands.Add(new Command()); // add command to be able to add tokens to it
                }
                else
                {
                    // add token to current command
                    commands.Last().Tokens.Add(token);
                }
            }
            if (commands.Last().Tokens.Count == 0) commands.RemoveAt(commands.Count - 1); // remove last command if it is empty

            // create objects from commands
            Objects = new List<DrawObject>();
            int LineType = 1;
            float LineWidth = 2.0f;
            int LineColor = 1;
            foreach (var command in commands)
            {
                var head = command.Tokens.First();
                if (head.Type == TextTokenType.Text)
                {
                    if (head.Text == "LineType")
                    {
                        int lineType;
                        if (Int32.TryParse(command.Tokens[1].Text, out lineType)) LineType = lineType;
                    }
                    if (head.Text == "LineWidth")
                    {
                        float lineWidth;
                        if (float.TryParse(command.Tokens[1].Text, out lineWidth)) LineWidth = lineWidth;
                    }
                    if (head.Text == "LineColr") // yes property name is 'LineColr' (without letter 'o')
                    {
                        int lineColor;
                        if (Int32.TryParse(command.Tokens[1].Text, out lineColor)) LineColor = lineColor;
                    }
                    if (head.Text == "Line")
                    {
                        Objects.Add(new PolylineObject(command, LineType, LineWidth, LineColor));
                    }
                }
            }
        }

        /// <summary>
        /// Draw polilines on form just to show what we have read from file
        /// </summary>
        /// <param name="e"></param>
        /// <param name="dx">coordinate shift by x</param>
        /// <param name="dy">coordinate shift by y</param>
        /// <param name="scalePercent">value from 0 to 1</param>
        public void Draw(PaintEventArgs e, float dx, float dy, float scalePercent)
        {
            foreach (var obj in Objects)
            {
                obj.Draw(e, dx, dy, scalePercent);
            }
        }


        /// <summary>
        /// grous tokens into one token and removes merged tokens, so we can get comments and strings
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private static bool GroupCommentOrString(ref List<TextToken> tokens)
        {
            // try to find start token
            var token = tokens.FirstOrDefault(o => o.Type == TextTokenType.Symbol && o.Text == "%" || o.Text == "\"");
            if (token == null)
            {
                // no token found - there are nothing to merge
                return false;
            }

            // try to find closing token
            int index = tokens.IndexOf(token);
            int indexEnd = -1;
            for (int i = index + 1; i < tokens.Count; i++)
            {
                if (tokens[i].Type == token.Type && tokens[i].Text == token.Text)
                {
                    indexEnd = i;
                    break;
                }
            }

            if (indexEnd == -1)
            {
                // no closing token found - there are nothing to merge
                // here we have to handle error - since every open token must have closing token
                return false;
            }

            // merge tokens into one
            string text = "";
            for (int i = index; i <= indexEnd; i++)
            {
                text += tokens[i].Text;
            }
            token.Type = token.Text == "%" ? TextTokenType.Comment : TextTokenType.String; // we have only 2 possible cases so we can use such condition
            token.Text = text;
            // delete merged tokens
            tokens.RemoveRange(index + 1, indexEnd - index);
            // return succeed status
            return true;
        }

    }
}
