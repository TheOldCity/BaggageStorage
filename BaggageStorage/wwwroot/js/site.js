$(function () {
    $(window).resize(function (e) {
        $(".height-as-parent").css('min-height', $(window).height() - 115);
        $(".page-sidebar").css('min-height', $(window).height() - 115);
    });

    $('.menu-toggler').click(function () {
        if ($('body').hasClass('page-sidebar-closed')) {
            Cookies.set('sidebar-closed', 'false', { expires: 365 * 10 });
        }
        else {
            Cookies.set('sidebar-closed', 'true', { expires: 365 * 10 });
        }
    });

    $(window).resize();
   
});

function closeBadgeClick(obj) {    
    var el = $(obj).parent().attr("href");
    eval($(el+" .portlet-title .actions a[href*=closePortlet]").attr('href'))
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function ShowView(actionURI, id, menuId) {
    
    $(".page-sidebar li").removeClass('active');    
    
    if ($("#a_portlet_" + id).size() == 0) {

        $tabTitle = $("#menu-item-" + menuId + " .title").text();
        $tab = '<li id="tab-opened-portlet-' + id + '" ><a id="a_portlet_' + id + '" href="#portlet_' + id + '" data-toggle="tab" aria-expanded="false" class="a-tab-opened-portlet">' + $tabTitle + '<span class="badge tab-badge" onclick="closeBadgeClick(this)" >x</span> </a></li>';
        $('#tab-opened-portlets').append($tab);

        var $element = $('<div class="tab-pane" id="portlet_' + id + '"></div>');
        $(".page-content #all-portlets").append($element);

        $('#a_portlet_' + id).click();

        $('#portlet_' + id).load(actionURI, null,
            function (response, status, xhr) {
                if (status == "error") {
                    var msg = "Ошибка в результате выполнения запроса.<br/>HTTP ";
                    toastr["error"](msg + xhr.status + " " + xhr.statusText);
                }
                else {
                    
                    $('#portlet_' + id + ' .portlet-header-icon').attr('class', $("#menu-item-" + menuId +' i').attr('class'));
                    
                    $(window).resize();
                }
            }
        );
    }
    else {
        $('#a_portlet_' + id).click();
    }
}

function table_cellPrepared(e) {

    if (e.rowType === "data" && e.column.command === "edit") {
        var isEditing = e.row.isEditing,
            $links = e.cellElement.find(".dx-link");

        $links.text("");

        if (isEditing) {
            $links.filter(".dx-link-save").addClass("dx-icon-save");
            $links.filter(".dx-link-cancel").addClass("dx-icon-revert");
        } else {
            $links.filter(".dx-link-edit").addClass("dx-icon-edit");
            $links.filter(".dx-link-delete").addClass("dx-icon-trash");
            $links.filter(".dx-link-add").addClass("dx-icon-add");
        }
    }

}

function btXReportClick() {    
    if (workPlace.IpAdressHardwareService == '' || workPlace.PortHardwareService == null || workPlace.PortHardwareService == undefined)
        toastr["error"]("IP адрес фискального сервиса не установлен");
    else {
        
        var result = DevExpress.ui.dialog.confirm("Вы уверены, что хотите распечатать отчет?", "Подтверждение");
        result.done(function (dialogResult) {
            if (dialogResult) {

                $.ajax({
                    method: "POST",
                        url: 'http://' + workPlace.IpAdressHardwareService + ':' + workPlace.PortHardwareService + '/xreport'
                    })
                    .error(function (e) {
                        toastr["error"]("Не удалось выполнить запрос к фискальному сервису");
                    });

            }
        });
    }
}

function btZReportClick() {
    if (workPlace.IpAdressHardwareService == '' || workPlace.PortHardwareService == null || workPlace.PortHardwareService == undefined)
        toastr["error"]($("#fiscalServiceNotSetted").val());
    else {


        var result = DevExpress.ui.dialog.confirm("Вы уверены, что хотите распечатать отчет?", "Подтверждение");
        result.done(function (dialogResult) {
            if (dialogResult) {

                $.ajax({
                    method: "POST",
                    url: 'http://' + workPlace.IpAdressHardwareService + ':' + workPlace.PortHardwareService + '/zreport'
                })
                    .error(function (e) {
                        toastr["error"]("Не удалось выполнить запрос к фискальному сервису");
                    })
                    .done(function (result) {
                        if (result.error != "")
                            toastr["error"](result.error);

                    });

                
            }
        });            
    }
}

function btAddMoneyClick(amount) {
    if (workPlace.IpAdressHardwareService == '' || workPlace.PortHardwareService == null || workPlace.PortHardwareService == undefined)
        toastr["error"]("IP адрес фискального сервиса не установлен");
    else {
        var data = { amount: amount };
        $.ajax({
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(data),
            url: 'http://' + workPlace.IpAdressHardwareService + ':' + workPlace.PortHardwareService + '/addmoney'
        })
            .done(function (result) {
                if (result.error == "") {

                }
                else
                    toastr["error"](result.error);

            });
    }
}

function btGiveMoneyClick(amount) {
    if (workPlace.IpAdressHardwareService == '' || workPlace.PortHardwareService == null || workPlace.PortHardwareService == undefined)
        toastr["error"]("IP адрес фискального сервиса не установлен");
    else {
        var data = { amount: amount };
        $.ajax({
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(data),
            url: 'http://' + workPlace.IpAdressHardwareService + ':' + workPlace.PortHardwareService +  '/givemoney'
        })
            .done(function (result) {
                if (result.error == "") {

                }
                else
                    toastr["error"](result.error);

            });
    }
}

function saleFiscalPrinter(pName, pCode, amount, docId) {
    if (workPlace.IpAdressHardwareService == '' || workPlace.PortHardwareService == null || workPlace.PortHardwareService == undefined)
        toastr["error"]("IP адрес фискального сервиса не установлен");
    else {
        var product = {
            productCode: pCode,
            productNameFromMemory: false,
            productName: pName,//'Plata p-u bagaj',
            //productSurchargeName: 'Plata p-u bagaj prelungit',
            productQuantity: 1,
            productPrice: amount,
            printProductBarcode: false,
            taxRateGroup: 0
        };

        var data = new Object();
        data.Rows = new Array();
        data.Rows.push(product);
        data.PrintQRCode = true;
        data.QRCodeValue = docId;
        data.PaymentAmountCard = 0;
        data.PaymentAmountCash = amount;

        $.ajax({
            method: "POST",
            url: 'http://' + workPlace.IpAdressHardwareService + ":" + workPlace.PortHardwareService + "/sale",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            data: JSON.stringify(data),
            error:
                function (XMLHttpRequest, textStatus, errorThrown) {
                    if (XMLHttpRequest.readyState == 4) {
                        // HTTP error (can be checked by XMLHttpRequest.status and XMLHttpRequest.statusText)
                        toastr["error"]('HTTP error. Status: ' + XMLHttpRequest.status + ', ' + XMLHttpRequest.statusText);
                    }
                    else if (XMLHttpRequest.readyState == 0) {
                        toastr["error"]('Network error');// Network error (i.e. connection refused, access denied due to CORS, etc.)
                    }
                    else {
                        toastr["error"]('Unknown error');
                    }
                }
        })
            .done(function (result) {
                if (result.error == "") {
                    
                }
                else
                    toastr["error"](result.error);
            });     

    }
}

function getFormattedDate(date) {
    var year = date.getFullYear();

    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;

    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;

    return year + "-" + month + '-' + day;
}

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}