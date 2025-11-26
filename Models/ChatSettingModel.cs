using Newtonsoft.Json;
using Parmigiano.Services.Wpf;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Parmigiano.Models
{
    public class ChatSettingModel : INotifyPropertyChanged
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("created_at")]
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("chat_id")]
        [JsonProperty("chat_id")]
        public ulong ChatId { get; set; }

        private string _сustom_background;
        [JsonPropertyName("custom_background")]
        [JsonProperty("custom_background")]
        public string CustomBackground
        {
            get => _сustom_background;
            set
            {
                if (_сustom_background != value)
                {
                    _сustom_background = value;
                    OnPropertyChanged(nameof(CustomBackground));
                }
            }
        }

        private bool _blocked;
        [JsonPropertyName("blocked")]
        [JsonProperty("blocked")]
        public bool Blocked
        {
            get => _blocked;
            set
            {
                if (_blocked != value)
                {
                    _blocked = value;
                    OnPropertyChanged(nameof(Blocked));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
