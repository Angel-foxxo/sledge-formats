using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Sledge.Formats.Texture.ImageSharp;
using Sledge.Formats.Texture.Vtf;
using System.IO;
using System.Linq;
using Image = SixLabors.ImageSharp.Image;

namespace Sledge.Formats.Texture.Tests.Vtf;

[TestClass]
public class TestImageSharp
{
    [DataTestMethod]
    [DataRow(VtfImageFormat.Rgba8888)]
    [DataRow(VtfImageFormat.Abgr8888)]
    [DataRow(VtfImageFormat.Rgb888)]
    [DataRow(VtfImageFormat.Bgr888)]
    [DataRow(VtfImageFormat.Rgb888Bluescreen)]
    [DataRow(VtfImageFormat.Bgr888Bluescreen)]
    [DataRow(VtfImageFormat.Argb8888)]
    [DataRow(VtfImageFormat.Bgra8888)]
    [DataRow(VtfImageFormat.Bgrx8888)]
    [DataRow(VtfImageFormat.Rgba16161616)]
    [DataRow(VtfImageFormat.Uvwq8888)]
    public void TestSimpleImageLosslessFormats(VtfImageFormat imageFormat)
    {
        var white = Color.ParseHex("FAFBFCFF").ToPixel<Rgba32>(); // almost white
        var blue = Color.ParseHex("0102FEFF").ToPixel<Rgba32>();  // almost blue

        using var source = new Image<Rgba32>(1024, 1024, white);
        source.Mutate(ctx => ctx.Paint(canvas =>
        {
            canvas.Fill(Brushes.Solid(Color.ParseHex("0102FEFF")), new Rectangle(300, 0, 100, 300));
        }));

        var builder = new VtfImageBuilder(new VtfImageBuilderOptions
        {
            AutoCreateMipmaps = false,
            AutoResizeToPowerOfTwo = true,
            ImageFormat = imageFormat,
        });

        var sourceVtf = new VtfFile();
        foreach (var img in builder.CreateImages(source))
        {
            sourceVtf.AddImage(img);
        }

        var beforeSaveVtfImage = sourceVtf.Images.MaxBy(x => x.Height);
        var beforeSaveImage = Image.LoadPixelData<Bgra32>(beforeSaveVtfImage.GetBgra32Data(), beforeSaveVtfImage.Width, beforeSaveVtfImage.Height);
        CheckImage(beforeSaveImage);

        using var ms = new MemoryStream();
        sourceVtf.Write(ms);
        ms.Seek(0, SeekOrigin.Begin);

        var destVtf = new VtfFile(ms);
        var afterSaveVtfImage = destVtf.Images.MaxBy(x => x.Height);
        var afterSaveImage = Image.LoadPixelData<Bgra32>(afterSaveVtfImage.GetBgra32Data(), afterSaveVtfImage.Width, afterSaveVtfImage.Height);
        CheckImage(afterSaveImage);

        return;

        void CheckImage<T>(Image<T> img) where T : unmanaged, IPixel<T>
        {
            img.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < accessor.Height; y++)
                {
                    var pixelRow = accessor.GetRowSpan(y);
                    for (var x = 0; x < pixelRow.Length; x++)
                    {
                        ref var pixel = ref pixelRow[x];
                        var convertedPixel = pixel.ToRgba32();
                        if (y is >= 0 and < 300 && x is >= 300 and < 400)
                        {
                            Assert.AreEqual(blue, convertedPixel, $"\nExpected {blue.ToHex()} for pixel at [{x},{y}], but got {convertedPixel.ToHex()} instead.");
                        }
                        else
                        {
                            Assert.AreEqual(white, convertedPixel, $"\nExpected {white.ToHex()} for pixel at [{x},{y}], but got {convertedPixel.ToHex()} instead.");
                        }
                    }
                }
            });
        }
    }

    [DataTestMethod]
    [DataRow(VtfImageFormat.Rgb565)]
    [DataRow(VtfImageFormat.Bgr565)]
    [DataRow(VtfImageFormat.Bgrx5551)]
    [DataRow(VtfImageFormat.Bgra4444)]
    [DataRow(VtfImageFormat.Dxt1)]
    [DataRow(VtfImageFormat.Dxt1Onebitalpha)]
    [DataRow(VtfImageFormat.Dxt3)]
    [DataRow(VtfImageFormat.Dxt5)]
    [DataRow(VtfImageFormat.Bgra5551)]
    public void TestSimpleImageLossyFormats(VtfImageFormat imageFormat)
    {
        var white = Color.ParseHex("224466FF").ToPixel<Rgba32>(); // almost white
        var blue = Color.ParseHex("EECCAAFF").ToPixel<Rgba32>();  // almost blue

        using var source = new Image<Rgba32>(1024, 1024, white);
        source.Mutate(ctx => ctx.Paint(canvas =>
        {
            canvas.Fill(Brushes.Solid(Color.ParseHex("EECCAAFF")), new Rectangle(300, 0, 100, 300));
        }));

        var builder = new VtfImageBuilder(new VtfImageBuilderOptions
        {
            AutoCreateMipmaps = false,
            AutoResizeToPowerOfTwo = true,
            ImageFormat = imageFormat,
        });

        var sourceVtf = new VtfFile();
        foreach (var img in builder.CreateImages(source))
        {
            sourceVtf.AddImage(img);
        }

        var beforeSaveVtfImage = sourceVtf.Images.MaxBy(x => x.Height);
        var beforeSaveImage = Image.LoadPixelData<Bgra32>(beforeSaveVtfImage.GetBgra32Data(), beforeSaveVtfImage.Width, beforeSaveVtfImage.Height);
        CheckImage(beforeSaveImage);

        using var ms = new MemoryStream();
        sourceVtf.Write(ms);
        ms.Seek(0, SeekOrigin.Begin);

        var destVtf = new VtfFile(ms);
        var afterSaveVtfImage = destVtf.Images.MaxBy(x => x.Height);
        var afterSaveImage = Image.LoadPixelData<Bgra32>(afterSaveVtfImage.GetBgra32Data(), afterSaveVtfImage.Width, afterSaveVtfImage.Height);
        CheckImage(afterSaveImage);

        return;

        void CheckImage<T>(Image<T> img) where T : unmanaged, IPixel<T>
        {
            img.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < accessor.Height; y++)
                {
                    var pixelRow = accessor.GetRowSpan(y);
                    for (var x = 0; x < pixelRow.Length; x++)
                    {
                        ref var pixel = ref pixelRow[x];
                        var convertedPixel = pixel.ToRgba32();
                        if (y is >= 0 and < 300 && x is >= 300 and < 400)
                        {
                            IsApproximatelyEqual(blue, convertedPixel, $"\nExpected approximately {blue.ToHex()} for pixel at [{x},{y}], but got {convertedPixel.ToHex()} instead.");
                        }
                        else
                        {
                            IsApproximatelyEqual(white, convertedPixel, $"\nExpected approximately {white.ToHex()} for pixel at [{x},{y}], but got {convertedPixel.ToHex()} instead.");
                        }
                    }
                }
            });
        }

        void IsApproximatelyEqual(Rgba32 expected, Rgba32 actual, string message)
        {
            const int variation = 8;
            Assert.IsTrue(actual.A >= expected.A - variation && actual.A <= expected.A + variation, message);
            Assert.IsTrue(actual.R >= expected.R - variation && actual.R <= expected.R + variation, message);
            Assert.IsTrue(actual.G >= expected.G - variation && actual.G <= expected.G + variation, message);
            Assert.IsTrue(actual.B >= expected.B - variation && actual.B <= expected.B + variation, message);
        }
    }
}