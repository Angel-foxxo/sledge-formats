using System.Numerics;
using SixLabors.ImageSharp.PixelFormats;

namespace Sledge.Formats.Texture.ImageSharp.PixelFormats;

public struct Rg88 : IPixel<Rg88>, IPackedVector<ushort>
{
    public ushort PackedValue { get; set; }

    public Rg88(Vector3 vector)
    {
        PackedValue = Pack(vector);
    }

    private static ushort Pack(Vector3 vector)
    {
        vector = Vector3.Clamp(vector, Vector3.Zero, Vector3.One);

        var r = (int)(vector.X * byte.MaxValue);
        var g = (int)(vector.Y * byte.MaxValue);

        return (ushort)(g << 8 | r);
    }

    static PixelOperations<Rg88> IPixel<Rg88>.CreatePixelOperations()
    {
        return new PixelOperations<Rg88>();
    }

    public static Rg88 FromScaledVector4(Vector4 vector)
    {
        return FromVector4(vector);
    }

    public readonly Vector4 ToScaledVector4()
    {
        return ToVector4();
    }

    public static Rg88 FromVector4(Vector4 vector)
    {
        return new Rg88(new Vector3(vector.X, vector.Y, vector.Z));
    }

    public readonly Vector4 ToVector4()
    {
        var r = (PackedValue & 0xFF);
        var g = (PackedValue & 0xFF00) >> 8;
        return new Vector4(r / (float) byte.MaxValue, g / (float) byte.MaxValue, 1, 1);
    }

    public readonly bool Equals(Rg88 other)
    {
        return PackedValue == other.PackedValue;
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is Rg88 other && Equals(other);
    }

    public readonly override int GetHashCode()
    {
        return PackedValue.GetHashCode();
    }

    public static bool operator ==(Rg88 left, Rg88 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Rg88 left, Rg88 right)
    {
        return !left.Equals(right);
    }

    public static PixelTypeInfo GetPixelTypeInfo()
    {
        return PixelTypeInfo.Create<Rg88>(
            PixelComponentInfo.Create<Rg88>(2, 8, 8),
            PixelColorType.Red | PixelColorType.Green,
            PixelAlphaRepresentation.None
        );
    }

    public Rgba32 ToRgba32() => throw new NotImplementedException();

    public readonly Vector4 ToUnassociatedScaledVector4() => ToScaledVector4();
    public readonly Vector4 ToAssociatedScaledVector4() => ToScaledVector4();
    public readonly Vector4 ToUnassociatedVector4() => ToVector4();
    public readonly Vector4 ToAssociatedVector4() => ToVector4();

    public static Rg88 FromUnassociatedScaledVector4(Vector4 source) => FromScaledVector4(source);

    public static Rg88 FromAssociatedScaledVector4(Vector4 source)
    {
        UnPremultiply(ref source);
        return FromScaledVector4(source);
    }

    public static Rg88 FromUnassociatedVector4(Vector4 source) => FromVector4(source);

    public static Rg88 FromAssociatedVector4(Vector4 source)
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

    public static Rg88 FromArgb32(Argb32 source) => throw new NotImplementedException();
    public static Rg88 FromBgra5551(Bgra5551 source) => throw new NotImplementedException();
    public static Rg88 FromBgr24(Bgr24 source) => throw new NotImplementedException();
    public static Rg88 FromBgra32(Bgra32 source) => throw new NotImplementedException();
    public static Rg88 FromAbgr32(Abgr32 source) => throw new NotImplementedException();
    public static Rg88 FromL8(L8 source) => throw new NotImplementedException();
    public static Rg88 FromL16(L16 source) => throw new NotImplementedException();
    public static Rg88 FromLa16(La16 source) => throw new NotImplementedException();
    public static Rg88 FromLa32(La32 source) => throw new NotImplementedException();
    public static Rg88 FromRgb24(Rgb24 source) => throw new NotImplementedException();
    public static Rg88 FromRgba32(Rgba32 source) => throw new NotImplementedException();
    public static Rg88 FromRgb48(Rgb48 source) => throw new NotImplementedException();
    public static Rg88 FromRgba64(Rgba64 source) => throw new NotImplementedException();
}