// K4WDisclaimer/Program.cs
// Copyright (c) 2013 Kazuhiro Sasao <k.sasao@gmail.com>
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace K4WDisclaimer
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = "This is preliminary software and/or hardware and APIs are preliminary and subject to change.";

            if (args.Length > 0)
            {
                try
                {
                    string outputFilename = args[0] + ".png";
                    if (args.Length > 1) {
                        outputFilename = args[1];
                    }
                    if (args.Length > 2)
                    {
                        text = Regex.Unescape(args[2]);
                    }
                    Caption wm = new Caption {
                        Text = text
                        //,Position = Position.Top
                        ,FontName = "Impact"
                        ,Size = 15
                        //,Italic = true
                        ,Bold = true
                        //,Color = System.Drawing.Color.LightBlue
                        //,BackgroundColor = System.Drawing.Color.White
                        //,BorderThick = 10f
                    };
                    wm.Load(args[0]).Render().Save(outputFilename);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
            else
            {
                Console.WriteLine("Usage: K4WDisclaimer targetFilename [[outputFileName(jpg/png)] message]");
            }

        }
    }
}
