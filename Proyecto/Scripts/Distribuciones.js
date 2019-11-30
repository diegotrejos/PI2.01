function getProvincia(id) {
    $.ajax({
        dataType: "json",
        url: "https://ubicaciones.paginasweb.cr/provincias.json",
        data: {},
        success: function (data) {
            var html = "";
            for (key in data) {
                html += '<option data-name="' + data[key] + '"value="' + key + '">' + data[key] + '</option>';
            }
            $("#province").append(html);

            if (id != null) { // para cuando es create
                $("#province").val(id);
            }
        }
    });
}


function getCanton(id_province, id_canton) { // Parámetros

    id_province = (id_province == null) ? $("#province").val() : id_province;

    $.ajax({
        dataType: "json",
        url: "https://ubicaciones.paginasweb.cr/provincia/" + id_province + "/cantones.json",
        data: {},
        success: function (data) {
            var html = "";
            for (key in data) {
                html += '<option data-name="' + data[key] + '"value="' + key + '">' + data[key] + '</option>';
            }
            $("#canton").empty();
            $("#canton").append(html);

            if (id_canton != null) {// para cuando es create
                $("#canton").val(id_canton);
            }
        }
    });
}

function getDistrict(id_province, id_canton, id_district) { // Parámetros

    id_province = (id_province == null) ? $("#province").val() : id_province;
    id_canton = (id_canton == null) ? $("#canton").val() : id_canton;

    $.ajax({
        dataType: "json",
        url: "https://ubicaciones.paginasweb.cr/provincia/" + id_province + "/canton/" + id_canton + "/distritos.json",
        data: {},
        success: function (data) {
            var html = "";
            for (key in data) {
                html += '<option data-name="' + data[key] + '"value="' + key + '">' + data[key] + '</option>';
            }
            $("#district").empty();
            $("#district").append(html);
            if (id_district != null) {// para cuando es create
                $("district").val(id_district);
            }
        }
    });
}