$(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal]").on("click", function (e) {        
        $('#myModalContent').load(this.href, function () {  
            var bsOffcanvas = new bootstrap.Offcanvas($('#myModal'));
            bsOffcanvas.show();          
            //$('#myModal').modal('show');
            bindForm(this);
        });
        return false;
    });
    $("a[data-winmodal]").on("click", function (e) {
        $('#winModalContent').load(this.href, function () {      
            var myModal = new bootstrap.Modal(document.getElementById('winModal'), {
                keyboard: false
            })
            myModal.show();
            bindWinForm(this);
        });
        return false;
    });
});

function bindWinForm(dialog) {
    $('form', dialog).submit(function () {
        $('#progress').show();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    $('#winModal').modal('hide');
                    $('#progress').hide();
                    location.reload();
                } else {
                    $('#progress').hide();
                    $('#myModalContent').html(result);
                    bindForm();
                }
            }
        });
        return false;
    });
}

function bindForm(dialog) {
    $('form', dialog).submit(function () {
        alert('ji');
        $('#progress').show();
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    var bsOffcanvas = new bootstrap.Offcanvas($('#myModal'));
                    bsOffcanvas.hide();          
                    $('#progress').hide();
                    location.reload();
                } else {                    
                    $('#progress').hide();
                    $('#myModalContent').html(result);
                    bindForm();
                }
            }
        });
        return false;
    });
}