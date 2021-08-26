using System;
using System.IO;
using System.Reflection;
using Agrirouter.Impl.Service.Convenience;

namespace Agrirouter.Test.Data
{
    public class DataProvider
    {
        public static string ReadBase64EncodedImage()
        {
            var path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                throw new InvalidOperationException(),
                @"Data/Content/example.png");
            var allBytes = File.ReadAllBytes(path);
            return Encode.ToMessageContent(allBytes);
        }

        public static string ReadBase64EncodedLargeBmp()
        {
            var path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                throw new InvalidOperationException(),
                @"Data/Content/large_bmp.bmp");
            var allBytes = File.ReadAllBytes(path);
            return Encode.ToMessageContent(allBytes);
        }

        public static string ReadBase64EncodedSmallShape()
        {
            var path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                throw new InvalidOperationException(),
                @"Data/Content/small_shape.zip");
            var allBytes = File.ReadAllBytes(path);
            return Encode.ToMessageContent(allBytes);
        }

        public static string ReadBase64EncodedLargeShape()
        {
            var path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                throw new InvalidOperationException(),
                @"Data/Content/large_shape.zip");
            var allBytes = File.ReadAllBytes(path);
            return Encode.ToMessageContent(allBytes);
        }

        public static string ReadBase64EncodedSmallTaskData()
        {
            var path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                throw new InvalidOperationException(),
                @"Data/Content/small_taskdata.zip");
            var allBytes = File.ReadAllBytes(path);
            return Encode.ToMessageContent(allBytes);
        }
    }
}