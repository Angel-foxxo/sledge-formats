using System.Numerics;
using SixLabors.ImageSharp.PixelFormats;

namespace Sledge.Formats.Texture.ImageSharp.PixelFormats;

public struct Rgb565 : IPixel<Rgb565>, IPackedVector<ushort>
{
    public ushort PackedValue { get; set; }

    public Rgb565(Vector3 vector)
    {
        PackedValue = Pack(vector);
    }

    private static ushort Pack(Vector3 vector)
    {
        vector = Vector3.Clamp(vector, Vector3.Zero, Vector3.One);

        return (ushort)(
            (((int)Math.Round(vector.Z * 31F) & 0x1F) << 11)
            | (((int)Math.Round(vector.Y * 63F) & 0x3F) << 5)
            | ((int)Math.Round(vector.X * 31F) & 0x1F)
        );
    }

    public readonly Vector3 ToVector3() => new(
        (PackedValue & 0x1F) * (1F / 31F),
       ((PackedValue >> 5) & 0x3F) * (1F / 63F),
       ((PackedValue >> 11) & 0x1F) * (1F / 31F)
    );

    static PixelOperations<Rgb565> IPixel<Rgb565>.CreatePixelOperations()
    {
        return new PixelOperations<Rgb565>();
    }

    public static Rgb565 FromScaledVector4(Vector4 vector)
    {
        return FromVector4(vector);
    }

    public readonly Vector4 ToScaledVector4()
    {
        return ToVector4();
    }

    public static Rgb565 FromVector4(Vector4 vector)
    {
        return new Rgb565(new Vector3(vector.X, vector.Y, vector.Z));
    }

    public readonly Vector4 ToVector4()
    {
        return new Vector4(ToVector3(), 1);
    }

    public readonly bool Equals(Rgb565 other)
    {
        return PackedValue == other.PackedValue;
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is Rgb565 other && Equals(other);
    }

    public readonly override int GetHashCode()
    {
        return PackedValue.GetHashCode();
    }

    public static bool operator ==(Rgb565 left, Rgb565 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Rgb565 left, Rgb565 right)
    {
        return !left.Equals(right);
    }

    public static PixelTypeInfo GetPixelTypeInfo()
    {
        return PixelTypeInfo.Create<Rgb565>(
            PixelComponentInfo.Create<Rgb565>(3, 5, 6, 5),
            PixelColorType.RGB,
            PixelAlphaRepresentation.None
        );
    }

    public Rgba32 ToRgba32() => throw new NotImplementedException();

    public readonly Vector4 ToUnassociatedScaledVector4() => ToScaledVector4();
    public readonly Vector4 ToAssociatedScaledVector4() => ToScaledVector4();
    public readonly Vector4 ToUnassociatedVector4() => ToVector4();
    public readonly Vector4 ToAssociatedVector4() => ToVector4();

    public static Rgb565 FromUnassociatedScaledVector4(Vector4 source) => FromScaledVector4(source);
    public static Rgb565 FromAssociatedScaledVector4(Vector4 source)
    {
        UnPremultiply(ref source);
        return FromScaledVector4(source);
    }

    public static Rgb565 FromUnassociatedVector4(Vector4 source) => FromVector4(source);
    public static Rgb565 FromAssociatedVector4(Vector4 source)
    {
        UnPremultiply(ref source);
        return FromVector4(source);
    }

    private static void UnPremultiply(ref Vector4 source)
    {
        var w = source.W;
        if (w != 0) source /= w;
        source.W = w;
    }

    public static Rgb565 FromArgb32(Argb32 source) => throw new NotImplementedException();
    public static Rgb565 FromBgra5551(Bgra5551 source) => throw new NotImplementedException();
    public static Rgb565 FromBgr24(Bgr24 source) => throw new NotImplementedException();
    public static Rgb565 FromBgra32(Bgra32 source) => throw new NotImplementedException();
    public static Rgb565 FromAbgr32(Abgr32 source) => throw new NotImplementedException();
    public static Rgb565 FromL8(L8 source) => throw new NotImplementedException();
    public static Rgb565 FromL16(L16 source) => throw new NotImplementedException();
    public static Rgb565 FromLa16(La16 source) => throw new NotImplementedException();
    public static Rgb565 FromLa32(La32 source) => throw new NotImplementedException();
    public static Rgb565 FromRgb24(Rgb24 source) => throw new NotImplementedException();
    public static Rgb565 FromRgba32(Rgba32 source) => throw new NotImplementedException();
    public static Rgb565 FromRgb48(Rgb48 source) => throw new NotImplementedException();
    public static Rgb565 FromRgba64(Rgba64 source) => throw new NotImplementedException();
}