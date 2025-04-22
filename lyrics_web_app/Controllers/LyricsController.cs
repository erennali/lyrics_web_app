using Microsoft.AspNetCore.Mvc;
using lyrics_web_app.Models;
using System.Text.Json;

namespace lyrics_web_app.Controllers
{
    public class LyricsController : Controller
    {
        private readonly HttpClient _httpClient;

        public LyricsController()
        {
            _httpClient = new HttpClient();
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
                var response = await _httpClient.GetAsync($"https://api.lyrics.ovh/v1/{model.Artist}/{model.Title}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var lyricsData = JsonSerializer.Deserialize<LyricsResponse>(content);
                    model.Lyrics = lyricsData?.Lyrics;
                }
                else
                {
                    model.ErrorMessage = "Şarkı sözleri bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Bir hata oluştu: " + ex.Message;
            }

            return View("Index", model);
        }
    }

    public class LyricsResponse
    {
        public string? Lyrics { get; set; }
    }
} 