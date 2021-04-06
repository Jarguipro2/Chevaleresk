/////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Author: Nicolas Chourot
// February 2020
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// This script generate necessary html control in order to offer an image uploader.
// Also it include validation rules to avoid submission on empty file and excessive image size.
// 
// This script is dependant of jquery and jquery validation.
//
//  Any <div> written as follow will contain an image file uploader :
//
//  <div class='imageUploader' id='data_Id' controlId = 'controlId' imageSrc='image url'> </div>
//  <span class="field-validation-valid text-danger" data-valmsg-for="controlId" data-valmsg-replace="true"></span>
//
//  If data_Id = 0 the file not empty validation rule will be applied
//
//  Example:
//
//  With the following:
//  <div class='imageUploader' id='0' controlId = 'PhotoImageData' imageSrc='/Photos/No_image.png' waitingImage="/PhotosManager/Photos/Loading_icon.gif"> </div>
//  <span class="field-validation-valid text-danger" data-valmsg-for="PhotoImageData" data-valmsg-replace="true"></span>
//
//
//  We obtain:
//  <div class="imageUploader" id="0" 
//       controlid="PhotoImageData"
//       imagesrc="/Photos/No_image.png" 
//       waitingImage = "/PhotosManager/Photos/Loading_icon.gif" >
//
//      <!-- Image uploaded -->
//      <img id="PhotoImageData_UploadedImage"
//           name="PhotoImageData_UploadedImage"
//           class="UploadedImage"
//           src="/Photos/No_image.png">
//
//      <!-- hidden file uploader -->
//      <input id="PhotoImageData_ImageUploader"
//             type="file"
//             style="visibility:hidden; height:0px;"
//             accept="image/jpeg,image/gif,image/png,image/bmp">
//  
//      <!-- hidden input uploaded imageData container -->
//      <input style="visibility:hidden;height:0px;"
//             class="fileUploadedExistRule fileUploadedSizeRule input-validation-error"
//             id="PhotoImageData"
//             name="PhotoImageData"
//             createmode="true"
//             waitingImage="/PhotosManager/Photos/Loading_icon.gif">
//  </div>
//  <span class="field-validation-valid text-danger" data-valmsg-for="PhotoImageData" data-valmsg-replace="true"></span>
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////


// Error messages
//let missingFileErrorMessage = "You must select an image file.";
//let tooBigFileErrorMessage = "Image too big! Please choose another one.";
//let wrongFileFormatMessage = "It is not a valid image file. Please choose another one.";
let missingFileErrorMessage = "Vous devez sélectionner une image.";
let tooBigFileErrorMessage = "L'image est trop volumineuse.";
let wrongFileFormatMessage = "Ce format d'image n'est pas accepté.";

let maxImageSize = 15000000;
var currentId = 0;

// Accepted file formats
let acceptedFileFormat = "image/jpeg,image/jpg,image/gif,image/png,image/bmp";

$(document).ready(() => {
    /* you can have more than one file uploader */
    $('.imageUploader').each(function () {
        let id = $(this).attr('id');
        let controlId = $(this).attr('controlId');
        let waitingImage = $(this).attr('waitingImage');
        let createMode = parseInt(id) === 0;

        let defaultImage = $(this).attr('imageSrc');
        $(this).append('<img id="' + controlId + '_UploadedImage" name="' + controlId + '_UploadedImage" class="UploadedImage" src="' + defaultImage + '" waitingImage ="' + waitingImage + '">');

        $(this).append('<input  id="' + controlId + '_ImageUploader" type="file" style="visibility:hidden;height:0px;" ' +
            ' accept="' + acceptedFileFormat + '"> ');

        $(this).append('<input style="visibility:hidden;height:0px;" class="fileUploadedExistRule fileUploadedSizeRule" id="' +
            controlId + '" name="' + controlId + '" createMode = "' + createMode + '" waitingImage ="' + waitingImage + '">');

        ImageUploader_AttachEvent(controlId);
        AddCustomValidator();
    });
});

function ImageUploader_AttachEvent(controlId) {
    // one click will be transmitted to #ImageUploader
    document.querySelector('#' + controlId + '_UploadedImage').
        addEventListener('click', () => {
            document.querySelector('#' + controlId + '_ImageUploader').click();
        });
    document.querySelector('#' + controlId + '_ImageUploader').
        addEventListener('change', preLoadImage );
}


function AddCustomValidator() {
    $.validator.addMethod("fileUploadedExistRule", function (value, element) { return CheckPhoto(element); }, missingFileErrorMessage);
    $.validator.addMethod("fileUploadedSizeRule", function (value, element) { return CheckPhotoSize(element); }, tooBigFileErrorMessage);
}

// Check if an image has been uploaded
function CheckPhoto(element) {
    let createMode = $(element).attr('createMode') === "true";
    if (createMode)
        return $(element).val() !== "";
    else
        return true;
}

// Check if uploaded image exceed maximum sixe
function CheckPhotoSize(element) {
    var files = $("#" + $(element).attr('id') + "_ImageUploader").get(0).files;
    if (files.length > 0)
        return files[0].size < maxImageSize;
    else
        return true;
}

function validExtension(ext) {
    return acceptedFileFormat.indexOf("/" + ext) > 0;
}

function preLoadImage(event) {
    // extract the id of the event target
    let id = event.target.id.split('_')[0];
    let UploadedImage = document.querySelector('#' + id + '_UploadedImage');
    let waitingImage = UploadedImage.getAttribute("waitingImage");
    let ImageUploader = document.querySelector('#' + id + '_ImageUploader');
    let ImageData = document.querySelector('#' + id);
    // store the previous uploaded image in case the file selection is aborted
    UploadedImage.setAttribute("previousImage", UploadedImage.src);
    // is there a file selection
    if (ImageUploader.value.length > 0) {

        // set the waiting image
        if (waitingImage !== "") UploadedImage.src = waitingImage;
        /* take some delai before starting uploading process in order to let browser to update UploadedImage new source affectation */
        let t2 = setTimeout(function () {
            if (UploadedImage !== null) {
                let len = ImageUploader.value.length;

                if (len !== 0) {
                    let fname = ImageUploader.value;
                    let ext = fname.split('.').pop().toLowerCase();

                    if (!validExtension(ext)) {
                        alert(wrongFileFormatMessage);
                        UploadedImage.src = UploadedImage.getAttribute("previousImage");
                    }
                    else {
                        let fReader = new FileReader();
                        fReader.readAsDataURL(ImageUploader.files[0]);
                        fReader.onloadend = () => {
                            UploadedImage.src = fReader.result;
                            ImageData.value = UploadedImage.src;
                        };
                    }
                }
                else {
                    UploadedImage.src = null;
                }
            }
        }, 30);
    }
    return true;
}


function rotateImage90(img) {
    // TODO this function only return png image file format wich
    // remove image jpg compression.
    // you can use it on small images.

    var canvas = document.createElement("canvas");
    canvas.height = img.width;
    canvas.width = img.height;
    var ctx = canvas.getContext("2d");
    ctx.rotate(90 * Math.PI / 180);
    ctx.translate(0, -canvas.width);
    ctx.drawImage(img, 0, 0);
    var dataURL = canvas.toDataURL();
    return dataURL;
}

