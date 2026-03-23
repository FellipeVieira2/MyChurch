using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;

namespace MyChurch.Application.Presentation.Services
{
    public static class SlideImageGenerator
    {
        public const int Width = 1280;
        public const int Height = 720;
        public const int Padding = 80;
        public const string FontFamilyName = "Arial";
        public const float FontSize = 30;
        public static int TextAreaWidth => Width - (Padding * 2);
        public static int TextAreaHeight => Height - (Padding * 2);

        // Logo base64 string (JPEG)
        private static readonly string MyChurchLogoBase64 = @"/9j/4AAQSkZJRgABAQAAAQABAAD/4gHYSUNDX1BST0ZJTEUAAQEAAAHIAAAAAAQwAABtbnRyUkdCIFhZWiAH4AABAAEAAAAAAABhY3NwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAA9tYAAQAAAADTLQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAlkZXNjAAAA8AAAACRyWFlaAAABFAAAABRnWFlaAAABKAAAABRiWFlaAAABPAAAABR3dHB0AAABUAAAABRyVFJDAAABZAAAAChnVFJDAAABZAAAAChiVFJDAAABZAAAAChjcHJ0AAABjAAAADxtbHVjAAAAAAAAAAEAAAAMZW5VUwAAAAgAAAAcAHMAUgBHAEJYWVogAAAAAAAAb6IAADj1AAADkFhZWiAAAAAAAABimQAAt4UAABjaWFlaIAAAAAAAACSgAAAPhAAAts9YWVogAAAAAAAA9tYAAQAAAADTLXBhcmEAAAAAAAQAAAACZmYAAPKnAAANWQAAE9AAAApbAAAAAAAAAABtbHVjAAAAAAAAAAEAAAAMZW5VUwAAACAAAAAcAEcAbwBvAGcAbABlACAASQBuAGMALgAgADIAMAAxADb/2wBDAAMCAgICAgMCAgIDAwMDBAYEBAQEBAgGBgUGCQgKCgkICQkKDA8MCgsOCwkJDRENDg8QEBEQCgwSExIQEw8QEBD/2wBDAQMDAwQDBAgEBAgQCwkLEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBD/wAARCAQABAADASIAAhEBAxEB/8QAHQABAAICAwEBAAAAAAAAAAAAAAEIBgcCBQkDBP/EAGgQAQABAgQEAwEGCw0RDwUBAQABAgMEBREhBgcxQQgSUWETIjJxgbEUGDdCUpGUobKz0xUXIzZicnN0dbTB4fAJFiQmJzNDU1RVVmRlk9HS8SU1REVGY4KDhIWSlaOkpRlHosLDNOL/xAAbAQEAAgMBAQAAAAAAAAAAAAAABgcBBAUDAv/EADwRAQABAgMDCAcIAwADAQEAAAABAgMEBREhcbEGMTRBUWGh0RITFTVTcpEWIjOBssHh8BRSgjJC8UMj/9oADAMBAAIRAxEAPwDyqAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" ;

        public static byte[] GenerateImage(string text)
        {
            using var bmp = new Bitmap(Width, Height);
            using var graphics = Graphics.FromImage(bmp);
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            // Fundo escuro
            graphics.Clear(Color.FromArgb(40, 40, 40));
            // Fonte branca, grande, negrito
            using var font = new Font(FontFamilyName, FontSize, FontStyle.Bold);
            using var brush = new SolidBrush(Color.White);
            // Sombra para melhor leitura
            using var shadowBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            var rect = new RectangleF(Padding, Padding, TextAreaWidth, TextAreaHeight);
            // Centralizar texto
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            // Desenha sombra
            graphics.DrawString(text, font, shadowBrush, new RectangleF(rect.X + 4, rect.Y + 4, rect.Width, rect.Height), format);
            // Desenha texto principal
            graphics.DrawString(text, font, brush, rect, format);

            // Desenhar logo no canto inferior esquerdo
            try
            {
                // Corrige: base64 pode não ter vírgula se não for data URI
                string base64 = MyChurchLogoBase64.Trim();
                // Remove possíveis espaços em branco e quebras de linha
                base64 = base64.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty);
                int commaIdx = base64.IndexOf(",");
                if (commaIdx >= 0)
                    base64 = base64[(commaIdx + 1)..];
                byte[] logoBytes = Convert.FromBase64String(base64);
                using var msLogo = new MemoryStream(logoBytes);
                using var logo = Image.FromStream(msLogo, true, true);
                int logoHeight = 80;
                int logoWidth = logo.Width * logoHeight / logo.Height;
                int margin = 32;
                graphics.DrawImage(logo, margin, Height - logoHeight - margin, logoWidth, logoHeight);
            }
            catch (Exception ex)
            {
                // Se falhar, ignora o logo
                // Opcional: logar ex.Message
            }

            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}
