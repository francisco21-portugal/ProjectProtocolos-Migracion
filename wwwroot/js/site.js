// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//funcion para que muesre la espera de caga de informacion de databla 

// CAMBIO: Se eliminó el tiempo mínimo fijo de espera (antes 1000 ms = 1 segundo).
// Ahora el overlay (logo de carga) se oculta únicamente cuando la data terminó de
// cargarse, mostrándose el tiempo REAL que demora la petición (puede ser 1 minuto o más).
function checkAndHideOverlay(startTime) {
    // El parámetro startTime se conserva por compatibilidad con las vistas.
    ocultarOverlay();
}

function mostrarOverlay() {
    if ($('#loaderOverlay').length === 0) {
        $('body').append(`
            <div id="loaderOverlay" style="
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background-color: rgba(0, 0, 0, 0.6);
                z-index: 9999;
                display: flex;
                justify-content: center;
                align-items: center;
                backdrop-filter: blur(3px);
            ">
                <div style="
                    background: white;
                    padding: 30px 40px;
                    border-radius: 15px;
                    text-align: center;
                    box-shadow: 0 10px 30px rgba(0,0,0,0.3);
                    animation: fadeInScale 0.3s ease;
                ">
                    <div class="spinner-border text-primary" role="status" style="width: 4rem; height: 4rem;">
                        <span class="visually-hidden">Cargando...</span>
                    </div>
                    <div class="mt-3">
                        <strong style="font-size: 1.2rem;">Cargando datos...</strong>
                    </div>
                    <div class="mt-2 text-muted small">
                        <span id="timerText">0</span> segundos
                    </div>
                </div>
            </div>
        `);

        let seconds = 0;
        const timerInterval = setInterval(() => {
            if ($('#loaderOverlay').length === 0) {
                clearInterval(timerInterval);
                return;
            }
            seconds++;
            const timerText = $('#timerText');
            if (timerText.length) { 
                timerText.text(seconds);
            }
        }, 1000);

        $('#loaderOverlay').data('timerInterval', timerInterval);
    }
}

function ocultarOverlay() {
    const overlay = $('#loaderOverlay');
    if (overlay.length) {
        const timerInterval = overlay.data('timerInterval');
        if (timerInterval) {
            clearInterval(timerInterval);
        }
        overlay.fadeOut(300, function () {
            $(this).remove();
        });
    }
}
