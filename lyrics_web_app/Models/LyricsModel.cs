using System;
using System.ComponentModel.DataAnnotations;

namespace lyrics_web_app.Models
{
    public class LyricsModel
    {
        [Required(ErrorMessage = "Sanatçı adı gereklidir")]
        public string? Artist { get; set; }

        [Required(ErrorMessage = "Şarkı adı gereklidir")]
        public string? Title { get; set; }

        public string? Lyrics { get; set; }
        public string? ErrorMessage { get; set; }
    }
} 