var fileSize = 300000000;
showModalLogin = function (title, size) {
    $.ajax({
        url: '/Account/Login',
        type: 'Get',
        success: function (res) {
            $("#mainModal .modal-body").text(res);
            $("#mainModal .modal-title").text(title);
            $("#mainModal .modal-dialog").removeClass('modal-xl');
            $("#mainModal").modal('show');
        }
    })
}

NosubmitLoginHandler = function (form) {
    event.preventDefault();
    
    if ($(form).valid()) {
        $("#btnSubmit").attr("disabled", true);
        $("#btnSubmit").append('<span class="spinner-border spinner-border-sm ml-2"></span>');
        $.ajax({
            url: '/Account/Login',
            /*url: '/Account/Login',*/
            type: 'Post',
            data: $(form).serialize(),
            success: function (res) {
                if (res == null) {
                    $("#mainModal .modal-body").text('');
                    $("#mainModal").modal('hide');

                    $("#btnSubmit").attr("disabled", false);
                    $("#btnSubmit").children("span").remove();
                    location.href = '/';
                }
                else {
                    $("#mainModal .modal-body").text(res);
                    $("#mainModal").modal('show');

                    $("#btnSubmit").attr("disabled", false);
                    $("#btnSubmit").children("span").remove();
                }
            },
            error: function (err) {
                console.log(err);
            }
        
        })
    }
}


showModal = function (title, size,res) {
    
            $("#mainModal .modal-body").text(res);
            $("#mainModal .modal-title").text(title);
            $("#mainModal .modal-dialog").removeClass(size);
            $("#mainModal").modal('show');
}

hideModal = function () {

    $("#mainModal .modal-body").text('');
    $("#mainModal .modal-title").text('');
    $("#mainModal .modal-dialog").removeClass('modal-xl');
    $("#mainModal").modal('hide');
}



//load all division menu
//loadMenuDivision = function () {
//    $.ajax({
//        url: 'Attachment/GetDivisionPlatform',
//        type: 'Get',        
//        success: function (res) {
//            $("#division_list").html(res);
//        }
//    })
//}
//loadMenuDivision_ISO = function () {
//    $.ajax({
//        url: 'Documents_ISO/GetDivisionPlatform',
//        type: 'Get',
//        success: function (res) {
//            $("#division_list").html(res);
//        }
//    })
//}

//Start working froups

//addNewGroups = function (title, size) {
//    $.ajax({
//        url: 'Groups/Create',
//        type: 'Get',
//        success: function (res) {
//            $("#mainModal .modal-body").html(res);
//            $("#mainModal .modal-title").html(title);
//            $("#mainModal .modal-dialog").removeClass('modal-xl');
//            $("#mainModal .modal-dialog").addClass(size);
//            $("#mainModal").modal('show');
//        }
//    })
//}
//createGroupsHandler = function (form) {
//    event.preventDefault();

//    if ($(form).valid()) {
//        $("#btnSubmit").attr("disabled", true);
//        $("#btnSubmit").append('<span class="spinner-border spinner-border-sm ml-2"></span>');
//        $.ajax({
//            url: 'Groups/Create',
//            type: "Post",
//            data: $(form).serialize(),
//            success: function (res) {
//                if (res == null) {
//                    $("#mainModal .modal-body").html('');
//                    $("#mainModal").modal('hide');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                    location.href = '/Division';
//                }
//                else {
//                    $("#mainModal .modal-body").html(res);
//                    $("#mainModal").modal('show');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                }
//            },
//            error: function (err) {
//                console.log(err);
//            }

//        })
//    }
//}

//editGroups = function (id, title, size) {
//    $.ajax({
//        url: 'Groups/Update',
//        type: 'Get',
//        data: { groupsId: id },
//        success: function (res) {
//            $("#mainModal .modal-body").html(res);
//            $("#mainModal .modal-title").html(title);
//            $("#mainModal .modal-dialog").removeClass('modal-xl');
//            $("#mainModal").modal('show');
//        }
//    })
//}

//updateGroupsHandler = function (form) {
//    event.preventDefault();

//    if ($(form).valid()) {
//        $("#btnSubmit").attr("disabled", true);
//        $("#btnSubmit").append('<span class="spinner-border spinner-border-sm ml-2"></span>');
//        $.ajax({
//            url: 'Groups/Update',
//            type: "Post",
//            data: $(form).serialize(),
//            success: function (res) {
//                if (res == null) {
//                    $("#mainModal .modal-body").html('');
//                    $("#mainModal").modal('hide');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                    location.href = '/Division';
//                }
//                else {
//                    $("#mainModal .modal-body").html(res);
//                    $("#mainModal").modal('show');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                }
//            },
//            error: function (err) {
//                console.log(err);
//            }

//        })
//    }
//}

//deleteGroups = function (id) {
//    //var conf = confirm("Are you sure to delete this Item?");
//    Swal.fire({
//        toast: true,
//        text: 'Do you want to Delete?',
//        icon: 'error',
//        iconHtml: '<i class="fas fa-question"></i>',
//        showConfirmButton: true,
//        confirmButtonText: 'Yes',
//        cancelButtonText: 'No',
//        showCancelButton: true,
//        confirmButtonColor: '#3085d6',
//        cancelButtonColor: '#d33',

//    }).then((result) => {
//        if (result.isConfirmed) {
//            $.ajax({
//                url: "Groups/Delete",
//                type: 'Post',
//                data: { groupsId: id },
//                success: function (res) {
//                    if (res == null) {
//                        location.href = '/Groups'
//                    } else {
//                        $("#menublock").html(res);
//                    }
//                },
//                error: function (err) {
//                    console.log(err);
//                }
//            })
//        }
//    });


//}
////end
///*  START WORKING WITH DIVISION */
////add new division
//addNewDivision = function (title, size) {
//        $.ajax({
//            url: 'Division/Create',
//            type: 'Get',
//            success: function (res) {
//                $("#mainModal .modal-body").html(res);
//                $("#mainModal .modal-title").html(title);
//                $("#mainModal .modal-dialog").removeClass('modal-xl');
//                $("#mainModal .modal-dialog").addClass(size);
//                $("#mainModal").modal('show');
//            }
//        })
//}


//createDivisionHandler = function (form) {
//    event.preventDefault();

//    if ($(form).valid()) {
//        $("#btnSubmit").attr("disabled", true);
//        $("#btnSubmit").append('<span class="spinner-border spinner-border-sm ml-2"></span>');
//        $.ajax({
//            url: 'Division/Create',
//            type: "Post",
//            data: $(form).serialize(),
//            success: function (res) {
//                if (res == null) {
//                    $("#mainModal .modal-body").html('');
//                    $("#mainModal").modal('hide');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                    location.href = '/Division';
//                }
//                else {
//                    $("#mainModal .modal-body").html(res);
//                    $("#mainModal").modal('show');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                }
//            },
//            error: function (err) {
//                console.log(err);
//            }

//        })
//    }
//}

//editDivision = function (id, title, size) {
//    $.ajax({
//        url: 'Division/Update',
//        type: 'Get',
//        data: { divisionId: id },
//        success: function (res) {
//            $("#mainModal .modal-body").html(res);
//            $("#mainModal .modal-title").html(title);
//            $("#mainModal .modal-dialog").removeClass('modal-xl');
//            $("#mainModal").modal('show');
//        }
//    })
//}

//updateDivisionHandler = function (form) {
//    event.preventDefault();

//    if ($(form).valid()) {
//        $("#btnSubmit").attr("disabled", true);
//        $("#btnSubmit").append('<span class="spinner-border spinner-border-sm ml-2"></span>');
//        $.ajax({
//            url: 'Division/Update',
//            type: "Post",
//            data: $(form).serialize(),
//            success: function (res) {
//                if (res == null) {
//                    $("#mainModal .modal-body").html('');
//                    $("#mainModal").modal('hide');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                    location.href = '/Division';
//                }
//                else {
//                    $("#mainModal .modal-body").html(res);
//                    $("#mainModal").modal('show');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                }
//            },
//            error: function (err) {
//                console.log(err);
//            }

//        })
//    }
//}

//deleteDivision = function (id) {
//    //var conf = confirm("Are you sure to delete this Item?");
//    Swal.fire({
//        toast: true,
//        text: 'Do you want to Delete?',
//        icon: 'error',
//        iconHtml: '<i class="fas fa-question"></i>',
//        showConfirmButton: true,
//        confirmButtonText: 'Yes',
//        cancelButtonText: 'No',
//        showCancelButton: true,
//        confirmButtonColor: '#3085d6',
//        cancelButtonColor: '#d33',

//    }).then((result) => {
//        if (result.isConfirmed) {
//            $.ajax({
//                url: "Division/Delete",
//                type: 'Post',
//                data: { divisionId: id },
//                success: function (res) {
//                    if (res == null) {
//                        location.href='/Division'
//                    } else {
//                        $("#menublock").html(res);
//                    }
//                },
//                error: function (err) {
//                    console.log(err);
//                }
//            })
//        }
//    });


//}
///*--END WORKING WITH DIVISION---*/


///*  START WORKING WITH PLATFORM */

//addNewPlatform = function (title, size) {
//    $.ajax({
//        url: 'Platform/Create',
//        type: 'Get',
//        success: function (res) {
//            $("#mainModal .modal-body").html(res);
//            $("#mainModal .modal-title").html(title);
//            $("#mainModal .modal-dialog").removeClass('modal-xl');
//            $("#mainModal .modal-dialog").addClass(size);
//            $("#mainModal").modal('show');
//        }
//    })
//}

//createPlatformHandler = function (form) {
//    event.preventDefault();

//    if ($(form).valid()) {
//        $("#btnSubmit").attr("disabled", true);
//        $("#btnSubmit").append('<span class="spinner-border spinner-border-sm ml-2"></span>');
//        $.ajax({
//            url: 'Platform/Create',
//            type: "Post",
//            data: $(form).serialize(),
//            success: function (res) {
//                if (res == null) {
//                    $("#mainModal .modal-body").html('');
//                    $("#mainModal").modal('hide');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                    location.href = '/Platform';
//                }
//                else {
//                    $("#mainModal .modal-body").html(res);
//                    $("#mainModal").modal('show');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                }
//            },
//            error: function (err) {
//                console.log(err);
//            }

//        })
//    }
//}

//getPlatformApi = function () {
//    $.ajax({
//        url: 'Platform/GetPlatformApi',
//        type: 'Post',
//        success: function (res) {
//            location.href = "/Platform"
//        }
//    })
//}

//editPlatform = function (id, title, size) {
//    $.ajax({
//        url: 'Platform/Update',
//        type: 'Get',
//        data: { platFormId: id },
//        success: function (res) {
//            $("#mainModal .modal-body").html(res);
//            $("#mainModal .modal-title").html(title);
//            $("#mainModal .modal-dialog").removeClass('modal-xl');
//            $("#mainModal").modal('show');
//        }
//    })
//}

//updatePlatformHandler = function (form) {
//    event.preventDefault();

//    if ($(form).valid()) {
//        $("#btnSubmit").attr("disabled", true);
//        $("#btnSubmit").append('<span class="spinner-border spinner-border-sm ml-2"></span>');
//        $.ajax({
//            url: 'Platform/Update',
//            type: "Post",
//            data: $(form).serialize(),
//            success: function (res) {
//                if (res == null) {
//                    $("#mainModal .modal-body").html('');
//                    $("#mainModal").modal('hide');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                    location.href = '/Platform';
//                }
//                else {
//                    $("#mainModal .modal-body").html(res);
//                    $("#mainModal").modal('show');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                }
//            },
//            error: function (err) {
//                console.log(err);
//            }

//        })
//    }
//}

//deletePlatform = function (id) {
//    //var conf = confirm("Are you sure to delete this Item?");
//    Swal.fire({
//        toast: true,
//        text: 'Do you want to Delete?',
//        icon: 'error',
//        iconHtml: '<i class="fas fa-question"></i>',
//        showConfirmButton: true,
//        confirmButtonText: 'Yes',
//        cancelButtonText: 'No',
//        showCancelButton: true,
//        confirmButtonColor: '#3085d6',
//        cancelButtonColor: '#d33',

//    }).then((result) => {
//        if (result.isConfirmed) {
//            $.ajax({
//                url: "Platform/Delete",
//                type: 'Post',
//                data: { platFormId: id },
//                success: function (res) {
//                    if (res == null) {
//                        location.href = '/Platform'
//                    } else {
//                        $("#menublock").html(res);
//                    }
//                },
//                error: function (err) {
//                    console.log(err);
//                }
//            })
//        }
//    });


//}
///*--END WORKING WITH PLATFORM---*/


///*  START WORKING WITH INSPECTION */

//addNewInspection = function (title, size) {
//    $.ajax({
//        url: 'Inspection/Create',
//        type: 'Get',
//        success: function (res) {
//            $("#mainModal .modal-body").html(res);
//            $("#mainModal .modal-title").html(title);
//            $("#mainModal .modal-dialog").removeClass('modal-xl');
//            $("#mainModal .modal-dialog").addClass(size);
//            $("#mainModal").modal('show');
//        }
//    })
//}

//createInspectionHandler = function (form) {
//    event.preventDefault();

//    if ($(form).valid()) {
//        $("#btnSubmit").attr("disabled", true);
//        $("#btnSubmit").append('<span class="spinner-border spinner-border-sm ml-2"></span>');
//        $.ajax({
//            url: 'Inspection/Create',
//            type: "Post",
//            data: $(form).serialize(),
//            success: function (res) {
//                if (res == null) {
//                    $("#mainModal .modal-body").html('');
//                    $("#mainModal").modal('hide');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                    location.href = '/Inspection';
//                }
//                else {
//                    $("#mainModal .modal-body").html(res);
//                    $("#mainModal").modal('show');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                }
//            },
//            error: function (err) {
//                console.log(err);
//            }

//        })
//    }
//}

//editInspection = function (id, title, size) {
//    $.ajax({
//        url: 'Inspection/Update',
//        type: 'Get',
//        data: { inspectionId: id },
//        success: function (res) {
//            $("#mainModal .modal-body").html(res);
//            $("#mainModal .modal-title").html(title);
//            $("#mainModal .modal-dialog").removeClass('modal-xl');
//            $("#mainModal").modal('show');
//        }
//    })
//}

//updateInspectionHandler = function (form) {
//    event.preventDefault();

//    if ($(form).valid()) {
//        $("#btnSubmit").attr("disabled", true);
//        $("#btnSubmit").append('<span class="spinner-border spinner-border-sm ml-2"></span>');
//        $.ajax({
//            url: 'Inspection/Update',
//            type: "Post",
//            data: $(form).serialize(),
//            success: function (res) {
//                if (res == null) {
//                    $("#mainModal .modal-body").html('');
//                    $("#mainModal").modal('hide');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                    location.href = '/Inspection';
//                }
//                else {
//                    $("#mainModal .modal-body").html(res);
//                    $("#mainModal").modal('show');
//                    $("#btnSubmit").attr("disabled", false);
//                    $("#btnSubmit").children("span").remove();
//                }
//            },
//            error: function (err) {
//                console.log(err);
//            }

//        })
//    }
//}

//deleteInspection = function (id) {
//    //var conf = confirm("Are you sure to delete this Item?");
//    Swal.fire({
//        toast: true,
//        text: 'Do you want to Delete?',
//        icon: 'error',
//        iconHtml: '<i class="fas fa-question"></i>',
//        showConfirmButton: true,
//        confirmButtonText: 'Yes',
//        cancelButtonText: 'No',
//        showCancelButton: true,
//        confirmButtonColor: '#3085d6',
//        cancelButtonColor: '#d33',

//    }).then((result) => {
//        if (result.isConfirmed) {
//            $.ajax({
//                url: "Inspection/Delete",
//                type: 'Post',
//                data: { inspectionId: id },
//                success: function (res) {
//                    if (res == null) {
//                        location.href = '/Inspection'
//                    } else {
//                        $("#menublock").html(res);
//                    }
//                },
//                error: function (err) {
//                    console.log(err);
//                }
//            })
//        }
//    });


//}
///*--END WORKING WITH INSPECTION---*/

///*----START WORKING WITH ATTACHMENT-------*/

////create new attachment
//showModalCreateNewAttachment = function (title, size) {
//    $.ajax({
//        url: 'Documents_ISO/Create',
//        type: 'Get',
//        success: function (res) {
//            $("#mainModal .modal-body").html(res);
//            $("#mainModal .modal-title").html(title);
//            $("#mainModal .modal-dialog").addClass(size);
//            $("#mainModal").modal('show');
//        }
//    })
//}

//// Load documents ISO
//showDocuments_ISOList = function (groupsid, documenttypeid) {
//    $.ajax({
//        url: 'Documents_ISO/GetAllFiles',
//        type: 'Get',
//        data: { groupsId: groupsid, documenttypeid: documenttypeid },
//        success: function (res)
//        {
//            $("#inspection_list").html(res);
//        },
//        error: function (err) {
//            console.log(err);
//        }
//    })

//}
//showDocuments_ISOListSearch = function () {
   
//    var group = $("#group").val();
//    var documenttype = $("#documenttype").val();
//   // alert(group);
//   // alert(documenttype);

//    $.ajax({
//        url: 'Documents_ISO/GetAllFiles',
//        type: 'Get',
//        data: { groupsId: group, documenttypeid: documenttype },
//        success: function (res) {
//            $("#inspection_list").html(res);
//        },
//        error: function (err) {
//            console.log(err);
//        }
//    })

//}
////end
///*----END WORKING WITH ATTACHMENT-------*/

//showDocumentInspectionList = function (divisionId, platFormId, type,loadMore, currentIndex, numerOfPage) {
//    localStorage.setItem("type", type ?? '');
//    localStorage.setItem("divisionId", divisionId ?? 0);
//    localStorage.setItem("platFormId", platFormId ?? 0);
//    if (type == 'child') {
//       //event.stopPropagation();
//        $(".accordion-body").removeClass("active");
//        $(".accordion-menu").removeClass("active");
//        $("#accordion-body_" + divisionId + "_" + platFormId).addClass("active");

//    }
//    else if (type == 'parent') {
//        $(".accordion-menu").removeClass("active");
//        $(".accordion-body").removeClass("active");
//        $("#accordion-menu_" + divisionId).addClass("active");
//    }
//    if (divisionId == 0 && platFormId == 0) {
//        $(".accordion-body").removeClass("active");
//        $(".accordion-menu").removeClass("active");
//        $("#allDoc").addClass("active");

//        }
// //get all inspection by divisionId and platFormId
//    let fromDate = $("#fromdate").val();
//    let toDate = $("#todate").val();
//    let parentInspection = $("#parentInspection").val();
//    let inspection = $("#inspection").val();
//    currentIndex = currentIndex ?? 0;
//    //set number of page for paging inspection list
//    numerOfPage = numerOfPage ?? 100;
//    $("#loading-inspection").css("display", "block");
    
//    $.ajax({
//        url: 'Attachment/GetAllFiles',
//        type: 'Get',
//        data: { divisionId: divisionId, platFormId: platFormId, parentInspection: parentInspection, inspection: inspection, fromDate, toDate, currentIndex:currentIndex, numerOfPage:numerOfPage },
//        success: function (res) {

//            if (loadMore === false) {
//                $("#loading-inspection").css("display", "none");
//                $("#inspection_list").html(res);                   
//            }
//            else {
//                $("#loading-inspection").css("display", "none");
//                $("#loadmore").remove();
//                $("#inspection_list").append(res)
//            }            
//        },
//        error: function (err) {
//            console.log(err);
//        }
//    })
    
//}

//loadMore = function (currentPage) {
//    let divisionId = localStorage.getItem("divisionId") === 'undefined' ? 0 : localStorage.getItem("divisionId");
//    let platFormId = localStorage.getItem("platFormId") === 'undefined' ? 0 : localStorage.getItem("platFormId");
//    let type = localStorage.getItem("type") === 'undefined' ? '' : localStorage.getItem("type");
//    showDocumentInspectionList(divisionId, platFormId, type, true, currentPage,null);
//}

//showFileAttachmentList = function (attachmentId) {
    
//    $.ajax({
//        url: 'Attachment/GetAttachment',
//        type: 'Get',
//        data: {
//            attachmentId:attachmentId
//        },
//        success: function (res) {
//            $("#offcanvas-attachment").html('');
//            $("#offcanvas-attachment").html(res)
//        },
//        error: function (err) {
//            console.log(err);
//        }
//    })

//}
//showDocISOFileAttachmentList = function (document_ISOId) {

//    $.ajax({
//        url: 'Documents_ISO/GetDocuments_ISO',
//        type: 'Get',
//        data: {
//            document_ISOId: document_ISOId
//        },
//        success: function (res) {
//            $("#offcanvas-attachment").html('');
//            $("#offcanvas-attachment").html(res);
//            $('#offcanvasRight').collapse('show');
//            $(".offcanvas-backdrop").show();
//        },
//        error: function (err) {
//            console.log(err);
//        }
//    });
//    $("#offcanvasRight btn-close, .offcanvas-backdrop").on("click", function () {
//        $('#offcanvasRight').collapse('hide');
//        $(".offcanvas-backdrop").hide();
//    });
//}
//reloadInspectionList = function () {
//    let divisionId = localStorage.getItem("divisionId") === 'undefined' ? 0 : localStorage.getItem("divisionId");
//    let platFormId = localStorage.getItem("platFormId") === 'undefined' ? 0 : localStorage.getItem("platFormId");
//    let type = localStorage.getItem("type") === 'undefined' ? '' : localStorage.getItem("type");

//    showDocumentInspectionList(divisionId, platFormId, type, false,null,null);
//}


//filterAttachment = function () {

//    let fromDate = $("#fromdate").val();
//    let toDate = $("#todate").val();
//    showDocumentInspectionList(0, 0,'',false,null,null);
//}

//clearFilter = function () {
//    $("#fromdate").val('');
//    $("#todate").val('');
//    $("#parentInspection").val('');
//    $("#inspection").val('');
//    //showDocumentInspectionList(0, 0, '',false,null,null);
//    showDocuments_ISOList(null, null);
//}

///*----working in upload files panel thirth--------*/


//$("#searchAttachment").on("keyup", function () {
    
//    var value = $(this).val().toLowerCase();
//    console.log(value);
//        $(".table-filter tbody tr").filter(function () {
//            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
//        });
//    });



//function extractContent(s) {
//    var span = document.createElement('span');
//    span.innerHTML = s;
//    return span.textContent || span.innerText;
//};

///*----end working in upload files panel thirth--------*/


////viewPdf = function (filePath,id) {
    
////    $("#preview_"+id).html(
////        `<div class="spinner-grow text-warning" role="status" style="width: 3rem; height: 3rem;">
////		<span class="sr-only">Loading...</span>
////	</div>`);
////    $.ajax({
////        url: 'Attachment/GetFileConvertToPDF',
////        type: 'Post',
////        data: { originalFile: filePath },
////        success: function (res) {
////            $("#preview_" + id).html(`
////                        <iframe id="iframe" src='${res}' width='100%' height='800'></iframe>
////                    `)
////            $('iframe').on('load', function () {  
////               $.ajax({
////                    url: 'Attachment/DeleteTempFileConvertToPDF',
////                    type: 'Post',
////                    data: { originalFile: filePath },
////                    success: function (res2) {
////                        console.log(res2);
////                    }
////               });
////            });
////        }
////    })
////}

////viewFile = function (filePath,id,fileType) {
    
////    $("#preview_"+id).html(
////        `
////    <div class="spinner-grow text-warning" role="status" style="width: 3rem; height: 3rem;">
////		<span class="sr-only">Loading...</span>
////	</div>
////    `);
////    if (fileType === "pdf") {
////        $("#preview_"+id).html(`
////         <iframe src='${'Documents/' + filePath}' width='100%' height='800'></iframe>
////     `);
////    }
////    else if (fileType === "video") {
////        $("#preview_" + id).html(`
////         <video width="90%" controls>
////                <source src="Documents/${filePath}">
////            </video>
////     `);
////    }
////    else if (fileType === "image") {          
////        $("#preview_" + id).html(`
////         <img src='${'Documents/' + filePath}' width="90%"/>
////     `);
////    }
////    else {
////        return false;
////    }
////}

//editDocuments_ISO = function (title, id) {
//  //  event.stopPropagation();
//    $.ajax({
//        url: 'Documents_ISO/Edit',
//        type: 'Get',
//        data: { attachmentId: id },
//        success: function (res) {
//            $("#mainModal .modal-body").html(res);
//            $("#mainModal .modal-title").html(title);
//            $("#mainModal .modal-dialog").addClass('modal-lg');
//            $("#mainModal").modal("show");
//        }
//    })
//}

//deleteDocumentISO = function (id) {
    
//    Swal.fire({
//        toast: true,
//        text: 'Do you want to Delete?',
//        icon: 'error',
//        iconHtml: '<i class="fas fa-question"></i>',
//        showConfirmButton: true,
//        confirmButtonText: 'Yes',
//        cancelButtonText: 'No',
//        showCancelButton: true,
//        confirmButtonColor: '#3085d6',
//        cancelButtonColor: '#d33',

//    }).then((result) => {
//        if (result.isConfirmed) {
//            $.ajax({
//                url: 'Documents_ISO/Delete',
//                type: 'Post',
//                data: { docId: id },
//                success: function (res) {
//                    if (res.isDeleted == true) {
//                        $.notify("Delete Attachment Successful", { "className": "success", "globalPosition": "top center" });
//                        //reloadInspectionList();
//                        showDocuments_ISOList(null, null);
//                    }
//                    else {
//                        $.notify("Can not Delete Attachment", { "className": "error" })
//                    }
//                }
//            })
//        }
//    })
//}

//editFileDescFunc = function (fileId,oldFileDescVn,oldFileDescRu, oldVersion) {
//    var parentElement = $("#filerow_" + fileId);
//    var fileDescVn = $(".filedescvn_" + fileId).val();
//    var fileDescRu = $(".filedescru_" + fileId).val();
//    var version = $(".version_" + fileId).val();
//    if (oldFileDescVn !== fileDescVn || oldFileDescRu !== fileDescRu || oldVersion !== version) {
//        $.ajax({
//            url: 'Documents_ISO/EditFile',
//            type: 'Post',
//            data: { id: fileId, fileDescVn: fileDescVn, fileDescRu: fileDescRu, version:version },
//            success: function (res) {
//                if (res.isEdited) {
//                    oldFileDescVn !== fileDescVn ? $(".filedescvn_" + fileId).addClass("bg-success") : "";
//                    oldFileDescRu !== fileDescRu ? $(".filedescru_" + fileId).addClass("bg-success") : "";
//                    oldVersion !== version ? $(".version_" + fileId).addClass("bg-success") : "";
//                    $.notify("Edited successful", { "className": "success", "globalPosition": "top center" });
//                }
//            },
//            error: function (err) {
//                console.log("edit file error...");
//            }
//        })
//    }
//}

//deleteFileFunc = function (fileId, fileName) {
//    var parentElement = $("#filerow_" + fileId);
    
//    Swal.fire({
//        toast: true,
//        text: 'Do you want to Delete?',
//        icon: 'error',
//        iconHtml: '<i class="fas fa-question"></i>',
//        showConfirmButton: true,
//        confirmButtonText: 'Yes',
//        cancelButtonText: 'No',
//        showCancelButton: true,
//        confirmButtonColor: '#3085d6',
//        cancelButtonColor: '#d33',

//    }).then((result) => {
//        if (result.isConfirmed) {
//            $.ajax({
//                url: 'Documents_ISO/DeleteFile',
//                type: 'Post',
//                data: { id: fileId, fileName: fileName },
//                success: function (res) {
//                    if (res.isDelete) {
//                        $.notify("Deleted", { "className": "success", "globalPosition": "top center" });
//                        $(parentElement).remove();
//                    }
//                },
//                error: function (err) {
//                    console.log("delete file error...");
//                }

//            })
//        }
//    })
//}

///*----working in comment panel ----*/
//sendComment = function (attachmentId) {
//    var userName = $("#UserName").val();
//    var description = $("#Description_"+attachmentId).val();
//    var photo = $("#Photo").val();
//    $.ajax({
//        url: 'Comment/CreateComment',
//        type: 'Post',
//        data: { AttachmentId: attachmentId, Description: description, UserName: userName,Photo:photo },
//        success: function (res) {
//            $("#Description_"+attachmentId).val('');
//            loadComments(attachmentId);
//        },
//        error: function (err) {
//            console.log(err);
//        }
//    })
//}

//deleteComment = function (id, attachmentId) {
//    $.ajax({
//        url: 'Comment/Delete',
//        type: 'Post',
//        data: { id: id },
//        success: function (res) {
//            loadComments(attachmentId);
//        },
//        error: function (err) {
//            console.log(err);
//        }

//    })
//}

//loadComments = function (attachmentId) {
//    $.ajax({
//        url: 'Comment/GetComments',
//        type: 'Get',
//        data: { attachmentId: attachmentId},
//        success: function (res) {
//            $("#commentlist_"+attachmentId).html(res);
//        },
//        error: function (err) {
//            console.log(err);
//        }
//    })
//}
///*----working in comment panel ----*/
///*--check file before upload--*/
//upload_check = function (thisFile) {

//    if (thisFile.files[0].size > fileSize) {
//        $(thisFile).next().html(`please chosse file less than ${fileSize/1000000} MB`);
//        $(thisFile).val('');
//    }
//    else {
//        $(thisFile).next().html('');
//    }
//}
///*--end check file before upload--*/

//removeRow = function (element) {
//    var parentElem = $(element).parents(".row");
//    $(parentElem).remove();
//}

////$("textarea.note").summernote({
////    height: 100,
////});

////$("#fromdate").daterangepicker({
////    singleDatePicker: true,
////    autoApply: true,
////    autoUpdateInput: false,
////    locale: {
////        format: 'DD/MM/YYYY'
////    }
////});
////$("#todate").daterangepicker({
////    singleDatePicker: true,
////    autoApply: true,
////    autoUpdateInput: false,
////    locale: {
////        format: 'DD/MM/YYYY'
////    }
////});
////$('#fromdate').on('apply.daterangepicker', function (ev, picker) {
////    $(this).val(picker.startDate.format('DD/MM/YYYY'));
////});

////$('#todate').on('apply.daterangepicker', function (ev, picker) {
////    $(this).val(picker.startDate.format('DD/MM/YYYY'));
////});

////$(".datepicker").daterangepicker({
////    singleDatePicker: true,
////    autoApply: true,
////    autoUpdateInput: true,
////    locale: {
////        format: 'DD/MM/YYYY'
////    }
////});
