using UnityEngine;
using UnityEngine.Experimental.Rendering;

#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class GraphicsFormatExtensions
    {
        public static bool IsCrunchFormat(this TextureFormat format) =>
            GraphicsFormatUtility.IsCrunchFormat(format);

        public static GraphicsFormat ConvertToAlphaFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.ConvertToAlphaFormat(format);

        public static bool IsSRGBFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsSRGBFormat(format);

        public static int GetColorComponentCount(this GraphicsFormat format) =>
            (int) GraphicsFormatUtility.GetColorComponentCount(format);

        public static int GetAlphaComponentCount(this GraphicsFormat format) =>
            (int) GraphicsFormatUtility.GetAlphaComponentCount(format);

        public static int GetComponentCount(this GraphicsFormat format) =>
            (int) GraphicsFormatUtility.GetComponentCount(format);

        public static bool IsCompressedFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsCompressedFormat(format);

        public static bool IsPackedFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsPackedFormat(format);

        public static bool Is16BitPackedFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.Is16BitPackedFormat(format);

        public static bool IsAlphaOnlyFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsAlphaOnlyFormat(format);

        public static bool HasAlphaChannel(this GraphicsFormat format) =>
            GraphicsFormatUtility.HasAlphaChannel(format);

        public static bool IsDepthFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsDepthFormat(format);

        public static bool IsStencilFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsStencilFormat(format);

        public static bool IsIEEE754Format(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsIEEE754Format(format);

        public static bool IsFloatFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsFloatFormat(format);

        public static bool IsHalfFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsHalfFormat(format);

        public static bool IsUnsignedFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsUnsignedFormat(format);

        public static bool IsSignedFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsSignedFormat(format);

        public static bool IsNormFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsNormFormat(format);

        public static bool IsUNormFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsUNormFormat(format);

        public static bool IsSNormFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsSNormFormat(format);

        public static bool IsIntegerFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsIntegerFormat(format);

        public static bool IsUIntFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsUIntFormat(format);

        public static bool IsSIntFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsSIntFormat(format);

        public static bool IsDXTCFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsDXTCFormat(format);

        public static bool IsRGTCFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsRGTCFormat(format);

        public static bool IsBPTCFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsBPTCFormat(format);

        public static bool IsBCFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsBCFormat(format);

        public static bool IsPVRTCFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsPVRTCFormat(format);

        public static bool IsETCFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsETCFormat(format);

        public static bool IsASTCFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsASTCFormat(format);

        public static int GetBlockSize(this GraphicsFormat format) =>
            (int) GraphicsFormatUtility.GetBlockSize(format);

        public static int GetBlockWidth(this GraphicsFormat format) =>
            (int) GraphicsFormatUtility.GetBlockWidth(format);

        public static int GetBlockHeight(this GraphicsFormat format) =>
            (int) GraphicsFormatUtility.GetBlockHeight(format);

        public static int ComputeMipmapSize(this GraphicsFormat format, int width, int height) =>
            (int) GraphicsFormatUtility.ComputeMipmapSize(width, height, format);

        public static int ComputeMipmapSize(this GraphicsFormat format, int width, int height, int depth) =>
            (int) GraphicsFormatUtility.ComputeMipmapSize(width, height, depth, format);

        public static bool IsPowerOfTwo(this GraphicsFormat format, bool hasMipmaps) =>
            format.IsCompressedFormat() && (hasMipmaps || format.IsPVRTCFormat());

        public static bool IsSquare(this GraphicsFormat format) =>
            format.IsPVRTCFormat();

        public static int GetMinWidth(this GraphicsFormat format) =>
            format.IsPVRTCFormat() ? format.GetBlockWidth() * 2 : format.GetBlockWidth();

        public static int GetMinHeight(this GraphicsFormat format) =>
            format.IsPVRTCFormat() ? format.GetBlockHeight() * 2 : format.GetBlockHeight();

        public static int GetMinSize(this GraphicsFormat format) =>
            format.IsPVRTCFormat() ? format.GetBlockSize() * 2 : format.GetBlockSize();

#if UNITY_2019_2_OR_NEWER
        public static bool IsSwizzleFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsSwizzleFormat(format);

        public static GraphicsFormat GetSRGBFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.GetSRGBFormat(format);

        public static GraphicsFormat GetLinearFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.GetLinearFormat(format);

        public static string GetFormatString(this GraphicsFormat format) =>
            GraphicsFormatUtility.GetFormatString(format);

        public static bool IsAlphaTestFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsAlphaTestFormat(format);

        public static bool IsXRFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsXRFormat(format);

        public static bool IsEACFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.IsEACFormat(format);

        public static GraphicsFormat ToGraphicsFormat(this RenderTextureFormat format, RenderTextureReadWrite readWrite) =>
            GraphicsFormatUtility.GetGraphicsFormat(format, readWrite);
#endif

#if UNITY_2019_3_OR_NEWER
        public static FormatSwizzle GetSwizzleR(this GraphicsFormat format) =>
            GraphicsFormatUtility.GetSwizzleR(format);

        public static FormatSwizzle GetSwizzleG(this GraphicsFormat format) =>
            GraphicsFormatUtility.GetSwizzleG(format);

        public static FormatSwizzle GetSwizzleB(this GraphicsFormat format) =>
            GraphicsFormatUtility.GetSwizzleB(format);

        public static FormatSwizzle GetSwizzleA(this GraphicsFormat format) =>
            GraphicsFormatUtility.GetSwizzleA(format);
#endif

        public static GraphicsFormat ToGraphicsFormat(this TextureFormat format, bool sRGB) =>
            GraphicsFormatUtility.GetGraphicsFormat(format, sRGB);

        public static TextureFormat ToTextureFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.GetTextureFormat(format);

        public static RenderTextureFormat ToRenderTextureFormat(this TextureFormat format) =>
            format.ToGraphicsFormat(false).ToRenderTextureFormat();

        public static GraphicsFormat ToGraphicsFormat(this RenderTextureFormat format, bool sRGB) =>
            GraphicsFormatUtility.GetGraphicsFormat(format, sRGB);

        public static RenderTextureFormat ToRenderTextureFormat(this GraphicsFormat format) =>
            GraphicsFormatUtility.GetRenderTextureFormat(format);

        public static TextureFormat ToTextureFormat(this RenderTextureFormat format) =>
            format.ToGraphicsFormat(false).ToTextureFormat();

#if UNITY_EDITOR
        // TextureFormat and TextureImporterFormat are basically the same (TextureFormat also has BGRA32 and YUY2)
        public static GraphicsFormat ToGraphicsFormat(this TextureImporterFormat format, bool sRGB) =>
            GraphicsFormatUtility.GetGraphicsFormat((TextureFormat) GetConservativeDefault(format), sRGB);

        public static TextureImporterFormat ToTextureImporterFormat(this GraphicsFormat format) =>
            (TextureImporterFormat) GraphicsFormatUtility.GetTextureFormat(format);

        public static TextureFormat ToTextureFormat(this TextureImporterFormat format) =>
            (TextureFormat) GetConservativeDefault(format);

        public static TextureImporterFormat ToTextureImporterFormat(this TextureFormat format) =>
            (TextureImporterFormat) format;

        public static RenderTextureFormat ToRenderTextureFormat(this TextureImporterFormat format) =>
            format.ToGraphicsFormat(false).ToRenderTextureFormat();

        public static TextureImporterFormat ToTextureImporterFormat(this RenderTextureFormat format) =>
            format.ToGraphicsFormat(false).ToTextureImporterFormat();

        /// <remarks>TextureImporterFormat's automatic entries have negative values</remarks>
        private static TextureImporterFormat GetConservativeDefault(TextureImporterFormat format) =>
            format >= 0 ? format : TextureImporterFormat.PVRTC_RGBA4;
#endif
    }
}