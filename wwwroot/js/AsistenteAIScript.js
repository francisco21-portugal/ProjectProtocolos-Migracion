$(document).on("click", ".ia-suggestion", function () {
    const pregunta = $(this).data("pregunta");

    $("#txtPreguntaIA")
        .val(pregunta)
        .focus();
});

$("#txtPreguntaIA").on("input", function () {
    $(this).removeClass("is-invalid");
});

$("#btnConsultarIA").on("click", function () {
    const pregunta = $("#txtPreguntaIA").val()?.trim() || "";
    const respuesta = $("#contenedorRespuestaIA");
    const boton = $(this);

    if (!pregunta) {
        $("#txtPreguntaIA")
            .addClass("is-invalid")
            .focus();

        return;
    }

    boton.prop("disabled", true);

    respuesta.html(`
        <div class="ia-response-icon">
            <span class="spinner-border spinner-border-sm"></span>
        </div>
        <div>
            <strong>Consultando información...</strong>
            <p class="mb-0">Espere un momento.</p>
        </div>
    `);

    $.ajax({
        url: "/Asistente/Consultar",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            pregunta: pregunta
        }),
        success: function (response) {
            respuesta.html(`
                <div class="ia-response-icon">
                    <i class="bi bi-check-circle"></i>
                </div>
                <div>
                    <strong>Respuesta</strong>
                    <p class="mb-0">${escapeHtml(response.respuesta || "--")}</p>
                </div>
            `);
        },
        error: function () {
            respuesta.html(`
                <div class="ia-response-icon text-danger">
                    <i class="bi bi-exclamation-circle"></i>
                </div>
                <div>
                    <strong>No se pudo completar la consulta</strong>
                    <p class="mb-0">Intente nuevamente.</p>
                </div>
            `);
        },
        complete: function () {
            boton.prop("disabled", false);
        }
    });
});

function escapeHtml(valor) {
    return $("<div>").text(valor ?? "--").html();
}