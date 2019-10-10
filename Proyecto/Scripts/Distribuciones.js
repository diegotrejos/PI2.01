function getProvincia(){ 
    $.ajax({
        dataType: "json",
        url: "https://ubicaciones.paginasweb.cr/provincias.json",
        data: {},
        success: function (data) {
            var html = "";
            for (key in data) {
              html += "<option value='" + key + "'>" + data[key] + "</option>";
            }
            console.log(data);
            $("#provincia").empty();
            $("#provincia").append(html);
        }
    });
}

function getCanton() {

    var id_provincia = $("#provincia").val();

    $.ajax({
        dataType: "json",
        url: "https://ubicaciones.paginasweb.cr/provincia/"+id_provincia+"/cantones.json",
        data: {},
        success: function (data) {
            var html = "";
            for (key in data) {
                html += "<option value='" + key + "'>" + data[key] + "</option>";
            }
            console.log(data);
            $("#canton").empty();
            $("#canton").append(html);
        }
    });
}

function getDistrict() {

    var id_provincia = $("#provincia").val();
    var id_canton = $("#canton").val();

    $.ajax({
        dataType: "json",
        url: "https://ubicaciones.paginasweb.cr/provincia/" + id_provincia + "/canton/" + id_canton + "/distritos.json",
        data: {},
        success: function (data) {
            var html = "";
            for (key in data) {
                html += "<option value='" + key + "'>" + data[key] + "</option>";
            }
            console.log(data);
            $("#distrito").empty();
            $("#distrito").append(html);
        }
    });
}