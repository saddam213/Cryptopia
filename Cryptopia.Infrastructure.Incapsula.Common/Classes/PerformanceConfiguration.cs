using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class PerformanceConfiguration
  {
    [JsonProperty(PropertyName = "advanced_caching_rules")]
    public AdvancedCachingRules AdvancedCachingRules { get; set; }

    [JsonProperty(PropertyName = "acceleration_level")]
    public string AccelerationLevel { get; set; }

    [JsonProperty(PropertyName = "async_validation")]
    public bool AsyncValidation { get; set; }

    [JsonProperty(PropertyName = "minify_javascript")]
    public bool MinifyJavascript { get; set; }

    [JsonProperty(PropertyName = "minify_css")]
    public bool MinifyCSS { get; set; }

    [JsonProperty(PropertyName = "minify_static_html")]
    public bool MinifyStaticHTML { get; set; }

    [JsonProperty(PropertyName = "compress_jepg")]
    public bool CompressJPEG { get; set; }

    [JsonProperty(PropertyName = "progressive_image_rendering")]
    public bool ProgressiveImageRendering { get; set; }

    [JsonProperty(PropertyName = "aggressive_compression")]
    public bool AggressiveCompression { get; set; }

    [JsonProperty(PropertyName = "compress_png")]
    public bool CompressPNG { get; set; }

    [JsonProperty(PropertyName = "on_the_fly_compression")]
    public bool OnTheFlyCompression { get; set; }

    [JsonProperty(PropertyName = "tcp_pre_pooling")]
    public bool TCPPrePooling { get; set; }

    [JsonProperty(PropertyName = "comply_no_cache")]
    public bool ComplyNoCache { get; set; }

    [JsonProperty(PropertyName = "comply_vary")]
    public bool ComplyVary { get; set; }

    [JsonProperty(PropertyName = "use_shortest_caching")]
    public bool UseShortestCaching { get; set; }

    [JsonProperty(PropertyName = "perfer_last_modified")]
    public bool PerferLastModified { get; set; }

    [JsonProperty(PropertyName = "accelerate_https")]
    public bool AccelerateHTTPS { get; set; }

    [JsonProperty(PropertyName = "disable_client_side_caching")]
    public bool DisableClientSideCaching { get; set; }

    [JsonProperty(PropertyName = "cache300x")]
    public bool Cache300x { get; set; }

    [JsonProperty(PropertyName = "cache_headers")]
    public List<object> CacheHeaders { get; set; }
  }
}
