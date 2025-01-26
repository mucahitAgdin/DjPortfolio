using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace YourProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public AdminController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        // Admin giriş ekranı
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "123") // Sabit kullanıcı bilgisi
            {
                HttpContext.Session.SetString("AdminLoggedIn", "true");
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Geçersiz kullanıcı adı veya şifre.";
            return View();
        }

        // Admin oturumu kapatma
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLoggedIn");
            return RedirectToAction("Login");
        }

        // Admin paneli ana sayfası
        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            var contentPath = Path.Combine(_environment.WebRootPath, "data/content.json");
            var jsonData = System.IO.File.ReadAllText(contentPath);

            var content = JsonSerializer.Deserialize<Content>(jsonData);
            return View(content.songs); // Şarkıları gönder
        }

        // Şarkı yükleme ekranı
        [HttpGet]
        public IActionResult AddSong()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult UploadSong(IFormFile songFile)
        {
            if (songFile != null && songFile.Length > 0)
            {
                // Media klasörüne yükleme yolu
                var uploadPath = Path.Combine(_environment.WebRootPath, "media");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // JSON dosyasını okuma
                var contentPath = Path.Combine(_environment.WebRootPath, "data/content.json");
                var jsonData = System.IO.File.ReadAllText(contentPath);

                var content = JsonSerializer.Deserialize<Content>(jsonData);
                if (content != null)
                {
                    // Yeni şarkı için dosya adını belirle
                    int nextId = content.songs.Count + 1; // JSON'daki mevcut şarkı sayısına göre belirle
                    var fileName = $"song{nextId}{Path.GetExtension(songFile.FileName)}"; // song1.mp3, song2.mp3 gibi
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Dosyayı kaydet
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        songFile.CopyTo(stream);
                    }

                    // JSON'a yeni şarkıyı ekle
                    var newSong = new Song
                    {
                        Id = nextId,
                        Title = $"Song {nextId}", // Şarkı adı otomatik
                        path = $"/media/{fileName}" // Media yolunu kaydet
                    };
                    content.songs.Add(newSong);

                    // JSON'u güncelle
                    var updatedJson = JsonSerializer.Serialize(content, new JsonSerializerOptions { WriteIndented = true });
                    System.IO.File.WriteAllText(contentPath, updatedJson);

                    ViewBag.Message = "Şarkı başarıyla yüklendi!";
                }
                else
                {
                    ViewBag.Message = "JSON dosyası yüklenirken bir hata oluştu.";
                }
            }
            else
            {
                ViewBag.Message = "Lütfen bir dosya seçin.";
            }

            return View("AddSong");
        }


        // Şarkı yönetimi
        [HttpGet]
        public IActionResult ManageSongs()
        {
            var contentPath = Path.Combine(_environment.WebRootPath, "data/content.json");
            var jsonData = System.IO.File.ReadAllText(contentPath);

            var content = JsonSerializer.Deserialize<Content>(jsonData);
            return View(content?.songs);
        }

        [HttpPost]
        public IActionResult DeleteSong(int id)
        {
            var contentPath = Path.Combine(_environment.WebRootPath, "data/content.json");
            var jsonData = System.IO.File.ReadAllText(contentPath);

            var content = JsonSerializer.Deserialize<Content>(jsonData);

            if (content != null)
            {
                var songToRemove = content.songs.FirstOrDefault(s => s.Id == id);
                if (songToRemove != null)
                {
                    // Şarkıyı listeden kaldır
                    content.songs.Remove(songToRemove);

                    // Medya dosyasını sil
                    var songPath = Path.Combine(_environment.WebRootPath, songToRemove.path.TrimStart('/'));
                    if (System.IO.File.Exists(songPath))
                    {
                        System.IO.File.Delete(songPath);
                    }

                    // JSON dosyasını güncelle
                    var updatedJson = JsonSerializer.Serialize(content, new JsonSerializerOptions { WriteIndented = true });
                    System.IO.File.WriteAllText(contentPath, updatedJson);
                }
            }

            return RedirectToAction("ManageSongs");
        }

        public class Song
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string path { get; set; } // Filepath yerine Path kullanıldı
        }

        public class Content
        {
            public string Logo { get; set; }
            public List<Song> songs { get; set; }
        }
    }
}
