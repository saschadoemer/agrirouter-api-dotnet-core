namespace com.dke.data.agrirouter.api.definitions
{
    /**
     * Technical messages types for the messages.
     */
    public static class TechnicalMessageTypes
    {
        public static string Empty => "";

        public static string DkeCapabilities => "dke:capabilities";

        public static string DkeListEndpoints => "dke:list_endpoints";

        public static string DkeListEndpointsUnfiltered => "dke:list_endpoints_unfiltered";

        public static string DkeFeedHeaderQuery => "dke:feed_header_query";

        public static string DkeFeedMessageQuery => "dke:feed_message_query";

        public static string DkeFeedConfirm => "dke:feed_confirm";

        public static string DkeSubscription => "dke:subscription";

        public static string Iso11783TaskdataZip => "iso:11783:-10:taskdata:zip";

        public static string Iso11783DeviceDescriptionProtobuf => "iso:11783:-10:device_description:protobuf";
       
        public static string Iso11783TimeLogProtobuf => "iso:11783:-10:time_log:protobuf";
        
        public static string ImgBmp => "img:bmp";
        
        public static string ImgJpeg => "img:jpeg";
        
        public static string ImgPng => "img:png";
        
        public static string ShpShapeZip => "shp:shape:zip";
        
        public static string DocPdf => "doc:pdf";
        
        public static string VidAvi => "vid:avi";
        
        public static string VidMp4 => "vid:mp4";
        
        public static string VidWmv => "vid:wmv";
    }
}