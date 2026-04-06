using Svg;

namespace ReconcileTool.UI.Forms;

internal static class IconHelper
{
    // Icon dùng chung toàn app — load một lần ở Program.cs
    public static Icon? AppIcon { get; private set; }

    public static void Initialize(string svgPath)
    {
        AppIcon ??= LoadFromSvg(svgPath);
    }

    /// <summary>
    /// Tải file SVG và tạo Icon (dùng cho form title bar, taskbar, Alt+Tab).
    /// Trả về null nếu có lỗi.
    /// </summary>
    public static Icon? LoadFromSvg(string svgPath)
    {
        try
        {
            var svg = SvgDocument.Open(svgPath);

            // Render nhiều kích thước để icon sắc nét ở mọi độ phân giải
            int[] sizes = [16, 32, 48, 256];
            var pngData = new List<byte[]>();

            foreach (int sz in sizes)
            {
                using var bmp = svg.Draw(sz, sz);
                using var ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                pngData.Add(ms.ToArray());
            }

            // Tạo ICO format thủ công (header + entries + PNG data)
            using var icoStream = new MemoryStream();
            using var bw = new BinaryWriter(icoStream, System.Text.Encoding.Default, leaveOpen: true);

            // ICONDIR
            bw.Write((short)0);              // Reserved
            bw.Write((short)1);              // Type: Icon
            bw.Write((short)sizes.Length);   // Count

            // Tính offset data (sau header + tất cả ICONDIRENTRY)
            int dataOffset = 6 + 16 * sizes.Length;

            // ICONDIRENTRY cho mỗi size
            for (int i = 0; i < sizes.Length; i++)
            {
                int sz = sizes[i];
                bw.Write((byte)(sz >= 256 ? 0 : sz)); // Width (0 = 256)
                bw.Write((byte)(sz >= 256 ? 0 : sz)); // Height
                bw.Write((byte)0);   // ColorCount
                bw.Write((byte)0);   // Reserved
                bw.Write((short)1);  // Planes
                bw.Write((short)32); // BitCount
                bw.Write(pngData[i].Length);  // BytesInRes
                bw.Write(dataOffset);          // ImageOffset
                dataOffset += pngData[i].Length;
            }

            // Image data (PNG)
            foreach (var png in pngData)
                bw.Write(png);

            bw.Flush();
            icoStream.Position = 0;
            return new Icon(icoStream);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Tạo và lưu file .ico từ SVG để dùng làm ApplicationIcon.
    /// </summary>
    public static void SaveIcoFile(string svgPath, string icoPath)
    {
        try
        {
            var svg = SvgDocument.Open(svgPath);
            int[] sizes = [16, 32, 48, 256];
            var pngData = new List<byte[]>();

            foreach (int sz in sizes)
            {
                using var bmp = svg.Draw(sz, sz);
                using var ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                pngData.Add(ms.ToArray());
            }

            using var icoStream = new FileStream(icoPath, FileMode.Create);
            using var bw = new BinaryWriter(icoStream);

            bw.Write((short)0);
            bw.Write((short)1);
            bw.Write((short)sizes.Length);

            int dataOffset = 6 + 16 * sizes.Length;
            for (int i = 0; i < sizes.Length; i++)
            {
                int sz = sizes[i];
                bw.Write((byte)(sz >= 256 ? 0 : sz));
                bw.Write((byte)(sz >= 256 ? 0 : sz));
                bw.Write((byte)0);
                bw.Write((byte)0);
                bw.Write((short)1);
                bw.Write((short)32);
                bw.Write(pngData[i].Length);
                bw.Write(dataOffset);
                dataOffset += pngData[i].Length;
            }

            foreach (var png in pngData)
                bw.Write(png);
        }
        catch { /* bỏ qua nếu lỗi */ }
    }
}
