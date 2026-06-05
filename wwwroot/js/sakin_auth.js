window.showForm = function (type) {
    const loginForm = document.getElementById('sakinLoginForm');
    const registerForm = document.getElementById('sakinRegisterForm');
    const tabLogin = document.getElementById('tabLogin');
    const tabRegister = document.getElementById('tabRegister');
    const msg = document.getElementById('sakinMessage');

    msg.innerText = "";

    if (type === 'login') {
        loginForm.style.display = 'block';
        registerForm.style.display = 'none';
        tabLogin.style.background = '#4ade80';
        tabLogin.style.color = '#0f172a';
        tabRegister.style.background = 'none';
        tabRegister.style.color = 'white';
    } else {
        loginForm.style.display = 'none';
        registerForm.style.display = 'block';
        tabRegister.style.background = '#4ade80';
        tabRegister.style.color = '#0f172a';
        tabLogin.style.background = 'none';
        tabLogin.style.color = 'white';
    }
};

document.addEventListener("DOMContentLoaded", function () {
    
    const regForm = document.getElementById('sakinRegisterForm');
    if (regForm) {
        regForm.addEventListener('submit', function (e) {
            e.preventDefault();
            const formData = new FormData(this);

            fetch('/Home/SakinKayit', {
                method: 'POST',
                body: new URLSearchParams(formData)
            })
                .then(res => res.json())
                .then(data => {
                    const msg = document.getElementById('sakinMessage');
                    msg.style.color = data.success ? "#4ade80" : "#fb7185";
                    msg.innerText = data.message;
                    if (data.success) setTimeout(() => { showForm('login'); }, 2000);
                });
        });
    }

    const logForm = document.getElementById('sakinLoginForm');
    if (logForm) {
        logForm.addEventListener('submit', function (e) {
            e.preventDefault();
            const formData = new FormData(this);

            fetch('/Home/SakinLogin', {
                method: 'POST',
                body: new URLSearchParams(formData)
            })
                .then(res => res.json())
                .then(data => {
                    const msg = document.getElementById('sakinMessage');
                    if (data.success) {
                        msg.style.color = "#4ade80";
                        msg.innerText = data.message;

                        setTimeout(() => {
                            window.location.href = data.redirectUrl || "/SakinPanel/Dashboard";
                        }, 1000);
                    } else {
                        msg.style.color = "#fb7185";
                        msg.innerText = data.message;
                    }
                })
                .catch(err => {
                    const msg = document.getElementById('sakinMessage');
                    msg.style.color = "#fb7185";
                    msg.innerText = "Bağlantı hatası!";
                });
        });
    }
});