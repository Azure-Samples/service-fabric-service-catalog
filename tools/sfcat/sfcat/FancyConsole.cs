using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    internal class FancyConsole
    {
        public static void DumpException(string prefix, Exception exp)
        {
            if (exp == null)
                return;
            FancyConsole.WriteLine("#r" + prefix + exp.Message);
            if (exp is AggregateException)
            {
                foreach (var e in ((AggregateException)exp).InnerExceptions)
                    DumpException("    " + prefix, e);
            }
            else if (exp.InnerException != null)
                DumpException("    " + prefix, exp.InnerException);
        }
        public static void WriteLine(string text, ConsoleColor forground = ConsoleColor.White, 
                                                  ConsoleColor background = ConsoleColor.Black, 
                                                  int paddingTop = 0, 
                                                  int paddingBottom = 0, 
                                                  bool centered = false,
                                                  char borderBottom = '\0')
        {
            var vFront = Console.ForegroundColor;
            var vBack = Console.BackgroundColor;
            Console.ForegroundColor = forground;
            Console.BackgroundColor = background;
            for (int i = 1; i <= paddingTop; i++)
                Console.WriteLine();
            WriteColorCodedLine((centered? new string(' ', Math.Max(0, (Console.WindowWidth - text.Length)/2)):"") + text);
            if (borderBottom != '\0')
                Console.WriteLine((centered ? new string(' ', Math.Max(0, (Console.WindowWidth - text.Length) / 2)) : "") + new string(borderBottom, text.Length));
            for (int i = 1; i <= paddingBottom; i++)
                Console.WriteLine();
            Console.ForegroundColor = vFront;
            Console.BackgroundColor = vBack;
        }
        public static void WriteColorCodedLine(string text)
        {
            var color = Console.ForegroundColor;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '#' && i < text.Length - 1)
                {
                    
                    switch (text[i + 1])
                    {
                        case 'g':
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case '~':
                            Console.ForegroundColor = color;
                            break;
                        case 'y':
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case 'c':
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case 'b':
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case 'w':
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case 'a':
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case 'r':
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                    }
                    i++;
                }
                else
                    Console.Write(text[i]);
            }
            Console.WriteLine();
            Console.ForegroundColor = color;
        }
    }
}
