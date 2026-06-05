document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById('loginForm');

    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const adSoyad = document.getElementById('A').value;
            const kullaniciAdi = document.getElementById('username').value;
            const sifre = document.getElementById('password').value;
            const msg = document.getElementById('message');

            const formData = new URLSearchParams();
            formData.append('AdSoyad', adSoyad);
            formData.append('KullaniciAdi', kullaniciAdi);
            formData.append('Sifre', sifre);

            fetch('/Home/SakinLogin', {
                method: 'POST',
                body: formData
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        msg.style.color = "#4ade80";
                        msg.innerText = data.message;

                        setTimeout(() => {
                            window.location.href = data.redirectUrl || "/Admin/Dashboard";
                        }, 1000);
                    } else {
                        msg.style.color = "#fb7185";
                        msg.innerText = data.message;
                    }
                })
                .catch(err => {
                    msg.style.color = "#fb7185";
                    msg.innerText = "Sunucu bağlantı hatası!";
                });
        });
    }
});