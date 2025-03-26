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

        // Şarkı oynatma işlevleri
        let currentSongIndex = 0;
        const audio = new Audio();
        const playPauseButton = document.getElementById('play-pause');
        const prevButton = document.getElementById('prev');
        const nextButton = document.getElementById('next');

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

        // İlk şarkıyı yükle
        loadSong(currentSongIndex);
    })
    .catch(error => console.error('Error fetching data:', error));


fetch('/data/images.json')
    .then((response) => response.json())
    .then((data) => {
        const container = document.getElementById('image-container');

        data.images.forEach((image) => {
            const imgElement = document.createElement('img');
            imgElement.src = image.src;
            imgElement.alt = image.alt;
            container.appendChild(imgElement);
        });
    })
    .catch((error) => console.error('Error fetching images:', error));
