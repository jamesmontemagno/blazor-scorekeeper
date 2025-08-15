using Svg.Skia;
using SkiaSharp;

static void RenderSvgToPng(string svgPath, string outputPath, int size)
{
    var svg = new SKSvg();
    using var stream = File.OpenRead(svgPath);
    var picture = svg.Load(stream);
    if (picture is null)
        throw new InvalidOperationException($"Failed to load SVG: {svgPath}");

    var bounds = picture.CullRect; // SKRect
    var maxDim = Math.Max(bounds.Width, bounds.Height);
    var scale = size / maxDim;

    using var surface = SKSurface.Create(new SKImageInfo(size, size, SKColorType.Rgba8888, SKAlphaType.Premul));
    var canvas = surface.Canvas;
    canvas.Clear(SKColors.Transparent);

    // Center content
    canvas.Scale(scale);
    var tx = (size / scale - bounds.Width) / 2f - bounds.Left;
    var ty = (size / scale - bounds.Height) / 2f - bounds.Top;
    canvas.Translate(tx, ty);
    canvas.DrawPicture(picture);

    using var image = surface.Snapshot();
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
    using var fs = File.Open(outputPath, FileMode.Create, FileAccess.Write);
    data.SaveTo(fs);
}

var repoRoot = Directory.GetCurrentDirectory();
// Assume running from tools/IconGen; adjust path to app wwwroot
var wwwroot = Path.GetFullPath(Path.Combine(repoRoot, "..", "..", "MyScoreBoard", "wwwroot"));
var svgIcon = Path.Combine(wwwroot, "icon.svg");
if (!File.Exists(svgIcon))
{
    Console.Error.WriteLine($"SVG not found at {svgIcon}");
    return 1;
}

var outputs = new (string file, int size)[]
{
    ("icon-512.png", 512),
    ("icon-256.png", 256),
    ("icon-192.png", 192),
    ("apple-touch-icon.png", 180),
    ("favicon-32x32.png", 32),
    ("favicon-16x16.png", 16)
};

foreach (var (file, size) in outputs)
{
    var outPath = Path.Combine(wwwroot, file);
    Console.WriteLine($"Generating {file} ({size}x{size})...");
    RenderSvgToPng(svgIcon, outPath, size);
}

Console.WriteLine("Icon generation complete.");
return 0;
