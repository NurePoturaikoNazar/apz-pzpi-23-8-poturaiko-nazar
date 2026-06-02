/* 
ЗАПИТИ ДО ШІ:
1. Поясни концепцію структурного патерна Proxy на прикладі кешування запитів до мережі;
2. Напиши приклад реалізації патерна Proxy мовою C#, що ілюструє завантаження та кешування відео, приховуючи мережеві виклики від клієнта;
3. Додай коментарі до коду українською мовою для кращого розуміння архітектури;
*/

using System;
using System.Collections.Generic;

namespace ProxyPatternExample
{
    // Спільний інтерфейс для реального сервісу та замісника
    public interface IYouTubeService
    {
        string GetVideo(string videoId);
    }

    // Реальний об'єкт, який імітує мережевий запит
    public class ThirdPartyYouTubeClass : IYouTubeService
    {
        public string GetVideo(string videoId)
        {
            Console.WriteLine($"Завантаження відео {videoId} з мережі...");
            return $"Відеодані для {videoId}";
        }
    }

    // Клас-замісник (Proxy), що додає логіку кешування
    public class CachedYouTubeClass : IYouTubeService
    {
        private ThirdPartyYouTubeClass _service;
        private Dictionary<string, string> _cache = new Dictionary<string, string>();

        public CachedYouTubeClass(ThirdPartyYouTubeClass service)
        {
            _service = service;
        }

        public string GetVideo(string videoId)
        {
            if (!_cache.ContainsKey(videoId))
            {
                _cache[videoId] = _service.GetVideo(videoId);
            }
            else
            {
                Console.WriteLine($"Отримання відео {videoId} з кешу...");
            }
            return _cache[videoId];
        }
    }

    // Клієнтський код
    public class YouTubeManager
    {
        private IYouTubeService _service;

        public YouTubeManager(IYouTubeService service)
        {
            _service = service;
        }

        public void RenderVideoPage(string videoId)
        {
            string video = _service.GetVideo(videoId);
            Console.WriteLine($"Відтворення: {video}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Ініціалізація залежностей
            ThirdPartyYouTubeClass realService = new ThirdPartyYouTubeClass();
            CachedYouTubeClass proxy = new CachedYouTubeClass(realService);
            YouTubeManager manager = new YouTubeManager(proxy);

            // Демонстрація роботи кешування
            manager.RenderVideoPage("watch?v=123"); // Виконає мережевий запит
            manager.RenderVideoPage("watch?v=123"); // Поверне результат із кешу
        }
    }
}