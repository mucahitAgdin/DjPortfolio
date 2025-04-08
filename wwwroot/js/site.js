fetch('/api/Content/data')
    .then(response => response.json())
    .then(data => {
        const logoPath = data.logo;
        const songs = data.songs;

        // Logoyu güncelle
        const logoElement = document.querySelector('.logo img');
        if (logoElement) {
            logoElement.src = logoPath;
        }

        // Müzik oynatma kontrolleri
        let currentSongIndex = 0;
        const audio = new Audio();
        const playPauseButton = document.getElementById('play-pause');
        const prevButton = document.getElementById('prev');
        const nextButton = document.getElementById('next');
        const seekBar = document.getElementById('seek-bar'); // Slider elementi
        const currentTimeDisplay = document.getElementById("current-time");
        const totalTimeDisplay = document.getElementById("total-time");

        function loadSong(index) {
            audio.src = songs[index].path;
            audio.load();
        }

        // Oynatma/Durdurma
        playPauseButton.addEventListener('click', () => {
            if (audio.paused) {
                audio.play();
            } else {
                audio.pause();
            }
        });

        // Önceki Şarkı
        prevButton.addEventListener('click', () => {
            currentSongIndex = (currentSongIndex - 1 + songs.length) % songs.length;
            loadSong(currentSongIndex);
            audio.play();
        });

        // Sonraki Şarkı
        nextButton.addEventListener('click', () => {
            currentSongIndex = (currentSongIndex + 1) % songs.length;
            loadSong(currentSongIndex);
            audio.play();
        });


        audio.addEventListener("loadedmetadata", () => {
            seekBar.max = audio.duration; // Maksimum süreyi güncelle
            totalTimeDisplay.textContent = formatTime(audio.duration); // Toplam süreyi göster
        });

        audio.addEventListener("timeupdate", () => {
            seekBar.value = audio.currentTime; // Kaydırma çubuğunu güncelle
            currentTimeDisplay.textContent = formatTime(audio.currentTime); // Anlık süreyi göster
        });

        // Seek bar ile süreyi değiştirme
        seekBar.addEventListener("input", () => {
            audio.currentTime = seekBar.value;
        });

        // Zaman formatını dakika:saniye olarak ayarlayan fonksiyon
        function formatTime(seconds) {
            const minutes = Math.floor(seconds / 60);
            const secs = Math.floor(seconds % 60);
            return `${minutes}:${secs < 10 ? "0" : ""}${secs}`;
        }
        // İlk şarkıyı yükle
        loadSong(currentSongIndex);
    })
    .catch(error => console.error('Error fetching data:', error));



fetch('/data/images.json')
    .then(response => response.json())
    .then(data => {
        const gifContainer = document.getElementById('gif-container');
        if (!gifContainer) {
            console.error("HATA: 'gif-container' ID'li div bulunamadı!");
            return;
        }

        // Tüm GIF'leri tek seferde HTML içine ekleyelim
        gifContainer.innerHTML = data.gifs.map((gifSrc, index) =>
            `<img src="${gifSrc}" class="gif-image ${index === 0 ? 'active' : ''}" />`
        ).join('');

        const gifElements = document.querySelectorAll('.gif-image');
        let currentIndex = 0;

        function changeGif() {
            gifElements[currentIndex].classList.remove("active");
            currentIndex = (currentIndex + 1) % gifElements.length;
            gifElements[currentIndex].classList.add("active");
        }

        setInterval(changeGif, 15000);
    })
    .catch(error => console.error('Error fetching gifs:', error));


// JSON'dan etkinlikleri çekip sidebar'a ekleyelim
fetch('/data/events.json')
    .then(response => response.json())
    .then(events => {
        const eventList = document.getElementById('events-list');
        if (!eventList) return;

        // Etkinlikleri listeye ekleyelim
        eventList.innerHTML = events.map(event => `
  <div class="event-item">
    <strong>${event.Title}</strong>
    <div class="event-date">${event.Date}</div>
    <p>${event.Description}</p>
  </div>
`).join('');


        // Sürekli kayan efekti oluşturuyor
        const cloneEvents = eventList.innerHTML; // Aynı etkinlikleri tekrar oluştur
        eventList.innerHTML += cloneEvents; // 2 katına çıkart, döngü için
    })
    .catch(error => console.error("Etkinlikleri çekerken hata oluştu:", error));
