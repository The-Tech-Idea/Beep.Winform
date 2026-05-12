using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Helpers
{
    /// <summary>
    /// Helper class for asynchronously loading node images from URLs or local paths.
    /// Provides caching and automatic tree invalidation when images load.
    /// </summary>
    public class BeepTreeAsyncImageLoader : IDisposable
    {
        private readonly BeepTree _owner;
        private readonly ConcurrentDictionary<string, Image> _urlImageCache = new ConcurrentDictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, bool> _loadingUrls = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        private readonly HttpClient _httpClient;
        private bool _disposed;

        public BeepTreeAsyncImageLoader(BeepTree owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Gets an image synchronously if cached, or starts async loading if not.
        /// Returns a placeholder if the image is still loading.
        /// </summary>
        public Image GetImage(string imagePath, out bool isLoading)
        {
            isLoading = false;

            if (string.IsNullOrEmpty(imagePath))
                return null;

            // Check if it's a URL
            if (IsUrl(imagePath))
            {
                // Check cache first
                if (_urlImageCache.TryGetValue(imagePath, out var cachedImage))
                    return cachedImage;

                // Start async loading if not already loading
                if (!_loadingUrls.ContainsKey(imagePath))
                {
                    isLoading = true;
                    _ = LoadImageAsync(imagePath);
                }
                else
                {
                    isLoading = true;
                }

                return null; // Return null to indicate loading - painter should draw placeholder
            }

            // For local paths, use the existing StyledImagePainter mechanism
            return null;
        }

        /// <summary>
        /// Checks if an image is currently being loaded.
        /// </summary>
        public bool IsLoading(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;
            return IsUrl(imagePath) && _loadingUrls.ContainsKey(imagePath);
        }

        /// <summary>
        /// Checks if an image is cached.
        /// </summary>
        public bool IsCached(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;
            return IsUrl(imagePath) && _urlImageCache.ContainsKey(imagePath);
        }

        /// <summary>
        /// Preloads an image asynchronously.
        /// </summary>
        public async Task PreloadImageAsync(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !IsUrl(imagePath))
                return;

            if (_urlImageCache.ContainsKey(imagePath) || _loadingUrls.ContainsKey(imagePath))
                return;

            await LoadImageAsync(imagePath);
        }

        /// <summary>
        /// Clears the URL image cache.
        /// </summary>
        public void ClearCache()
        {
            foreach (var kvp in _urlImageCache)
            {
                try { kvp.Value?.Dispose(); } catch { }
            }
            _urlImageCache.Clear();
            _loadingUrls.Clear();
        }

        private async Task LoadImageAsync(string url)
        {
            if (_loadingUrls.TryAdd(url, true))
            {
                try
                {
                    byte[] imageData = await _httpClient.GetByteArrayAsync(url);
                    using (var ms = new MemoryStream(imageData))
                    {
                        var image = Image.FromStream(ms);
                        _urlImageCache.TryAdd(url, image);
                    }

                    // Invalidate the tree on the UI thread
                    _owner?.Invoke(new Action(() =>
                    {
                        _owner?.Invalidate();
                    }));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[BeepTreeAsyncImageLoader] Failed to load image from {url}: {ex.Message}");
                }
                finally
                {
                    _loadingUrls.TryRemove(url, out _);
                }
            }
        }

        private bool IsUrl(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            return path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   path.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                ClearCache();
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }
}
