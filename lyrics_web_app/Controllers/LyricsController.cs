using Microsoft.AspNetCore.Mvc;
using lyrics_web_app.Models;
using System.Text.Json;
using System.Diagnostics;

namespace lyrics_web_app.Controllers
{
    public class LyricsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LyricsController> _logger;

        public LyricsController(ILogger<LyricsController> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new LyricsModel());
        }

        [HttpPost]
        public async Task<IActionResult> Search(LyricsModel model)
        {
            try
            {
                var url = $"https://api.lyrics.ovh/v1/{Uri.EscapeDataString(model.Artist!)}/{Uri.EscapeDataString(model.Title!)}";
                _logger.LogInformation($"API Request URL: {url}");

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response Status: {response.StatusCode}");
                _logger.LogInformation($"API Response Content: {content}");
                
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    
                    var lyricsData = JsonSerializer.Deserialize<LyricsResponse>(content, options);
                    _logger.LogInformation($"Deserialized Lyrics: {lyricsData?.lyrics}");
                    
                    if (!string.IsNullOrEmpty(lyricsData?.lyrics))
                    {
                        model.Lyrics = lyricsData.lyrics;
                        TempData["SuccessMessage"] = "Şarkı sözleri başarıyla bulundu!";
                    }
                    else
                    {
                        model.ErrorMessage = "Şarkı sözleri bulunamadı.";
                    }
                }
                else
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content);
                    model.ErrorMessage = errorResponse?.Error ?? "Şarkı sözleri bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API isteği sırasında hata oluştu");
                model.ErrorMessage = "Bir hata oluştu: " + ex.Message;
            }

            return View("Index", model);
        }
    }

    public class LyricsResponse
    {
        public string? lyrics { get; set; }
    }

    public class ErrorResponse
    {
        public string? Error { get; set; }
    }
} 