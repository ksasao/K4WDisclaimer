// Caption.cs
// Copyright (c) 2013 Kazuhiro Sasao <k.sasao@gmail.com>
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K4WDisclaimer
{
    /// <summary>
    /// Text Rendering Position
    /// </summary>
    public enum Position { Top, Bottom };
    /// <summary>
    /// Overlay Caption
    /// </summary>
    public class Caption
    {
        /// <summary>
        /// Font Color
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// Font Background Color
        /// </summary>
        public Color BackgroundColor { get; set; }
        /// <summary>
        /// Font Size
        /// </summary>
        public float Size { get; set; }
        /// <summary>
        /// Message
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Font Name
        /// </summary>
        public string FontName { get; set; }
        /// <summary>
        /// Bold
        /// </summary>
        public bool Bold { get; set; }
        /// <summary>
        /// Italic
        /// </summary>
        public bool Italic { get; set; }
        /// <summary>
        /// Render Position
        /// </summary>
        public Position Position { get; set; }
        /// <summary>
        /// Text border thickness
        /// </summary>
        public float BorderThick { get; set; }

        Bitmap bitmap = null;

        public Caption()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Color = Color.FromArgb(255, 255, 255, 255);
            this.BackgroundColor = Color.FromArgb(255, 0, 0, 0);
            this.Size = 25;
            this.Text = "(Caption)";
            this.FontName = "Arial";
            this.Bold = false;
            this.Italic = false;
            this.Position = Position.Top;
            this.BorderThick = 3.5f;
        }

        /// <summary>
        /// Load Image File
        /// </summary>
        /// <param name="filename">filename</param>
        public Caption Load(string filename)
        {
            using (Bitmap bmp = new Bitmap(filename))
            {
                if (this.bitmap != null)
                {
                    this.bitmap.Dispose();
                }
                // Convert To 32bit ARGB
                this.bitmap = new Bitmap(bmp);
            }
            return this;
        }
        /// <summary>
        /// Save Image File(.jpg or .png)
        /// </summary>
        /// <param name="filename">filename</param>
        public Caption Save(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();
            if (ext == ".jpg" || ext == ".jpeg")
            {
                long quality = 100;
                ImageCodecInfo jpgEncoder = (
                    from c in ImageCodecInfo.GetImageEncoders() 
                    where c.FormatID == ImageFormat.Jpeg.Guid 
                    select c).FirstOrDefault();
                EncoderParameters encParams = new EncoderParameters(1);
                encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                bitmap.Save(filename, jpgEncoder, encParams);
            }
            else
            {
                bitmap.Save(filename,System.Drawing.Imaging.ImageFormat.Png);
            }
            return this;
        }
        /// <summary>
        /// Render Message
        /// </summary>
        /// <returns>This object</returns>
        public Caption Render()
        {
            return Render(this.Text, Position.Bottom);
        }
        public Caption Render(string message)
        {
            return Render(message, Position.Bottom);
        }
        public Caption Render(string message, Position position)
        {
            return Render(message, position, this.FontName, this.Size, this.Bold, this.Italic);
        }
        public Caption Render(string message, Position position, string fontName, float fontSize, bool isBold, bool isItalic)
        {
            return Render(message, position, fontName, fontSize, isBold, isItalic, this.Color, this.BackgroundColor, this.BorderThick);
        }
        public Caption Render(string message, Position position, string fontName, float fontSize, bool isBold, bool isItalic, Color fontColor, Color bgColor, float bgBorder)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                float padding = bgBorder * g.DpiX / 72;
                float x = padding;
                float y = padding;

                int fontStyle = (int)System.Drawing.FontStyle.Regular;
                if (isBold) fontStyle |= (int)System.Drawing.FontStyle.Bold;
                if (isItalic) fontStyle |= (int)System.Drawing.FontStyle.Italic;

                Font font = new Font(fontName, fontSize);
                Brush brush = new SolidBrush(fontColor);
                Brush bbrush = new SolidBrush(bgColor);
                Pen pen = new Pen(bbrush, padding);

                pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // Get Render Bounds
                RectangleF rect;
                using (var gp = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    gp.AddString(message, new FontFamily(fontName), fontStyle, fontSize, new RectangleF(new PointF(padding, padding), new SizeF(bitmap.Width - padding, bitmap.Height - padding)), StringFormat.GenericTypographic);
                    
                    rect = gp.GetBounds();
                }

                // Render
                if (position == K4WDisclaimer.Position.Bottom)
                {
                    y = bitmap.Height - rect.Height - rect.Y - padding;
                }
                using (var gp = new System.Drawing.Drawing2D.GraphicsPath())
                {

                    gp.AddString(message, new FontFamily(fontName), fontStyle, fontSize, new RectangleF(new PointF(x, y), new SizeF(bitmap.Width - padding, bitmap.Height - padding)), StringFormat.GenericTypographic);
                    g.FillPath(bbrush, gp);
                    g.DrawPath(pen, gp);
                    g.FillPath(brush, gp);
                }

                brush.Dispose();
                bbrush.Dispose();
                pen.Dispose();
                font.Dispose();
            }
            return this;
        }

    }
}
