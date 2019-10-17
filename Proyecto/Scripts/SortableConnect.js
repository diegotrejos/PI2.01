//ocupo cambiar este metodo para que se llame con un boton y en ese momento realizar los cambios 
$(document).ready(function () {
    $("#disponibilidad").sortable({
        connectWith: "#asignacion",
        update: function (event, ui) {
            var itemIds = "";
            $("disponibilidad").find(".taskSingleInline").each(function () {
                var itemId = $(this).attr("data-taskid");
                itemIds = itemIds + itemId + ",";
            });
            $.ajax({
                url: '@Url.Action("UpdateItem", "Equipo")',
                data: { itemIds: itemIds },
                type: 'POST',
                success: function (data) {
                },
                error: function (xhr, status, error) {
                }
            });
        }
    });
})