//ocupo cambiar este metodo para que se llame con un boton y en ese momento realizar los cambios 
$(document).ready(function () {
    $("#disponibilidad").sortable({
        connectWith: "#asignacion"
    })
});
