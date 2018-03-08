//取消订单
function cancelOrder(oid, cancelReason) {
    $.post("/ucenter/cancelorder", { 'oid': oid, 'cancelReason': cancelReason }, cancelOrderResponse);
}

//处理取消订单的反馈信息
function cancelOrderResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        $("#orderState" + result.content).html("取消");
        $("#payOrderBut" + result.content).remove();
        $("#cancelOrderBut" + result.content).remove();
        alert("取消成功");
    }
    else {
        alert(result.content);
    }
}

//打开评价商品层
function openReviewProductBlock(recordId) {
    var reviewProductFrom = document.forms["reviewProductFrom"];
    reviewProductFrom.elements["recordId"].value = recordId;
    $("#reviewProductBlock").show();
}

//评价商品
function reviewProduct() {
    var reviewProductFrom = document.forms["reviewProductFrom"];

    var oid = reviewProductFrom.elements["oid"].value;
    var recordId = reviewProductFrom.elements["recordId"].value;
    var star = getSelectedRadio(reviewProductFrom.elements["star"]).value;
    var message = reviewProductFrom.elements["message"].value;

    if (!verifyReviewProduct(recordId, star, message)) {
        return;
    }
    $.post("/ucenter/reviewproduct?oid=" + oid + "&recordId=" + recordId, { 'star': star, 'message': message }, reviewProductResponse);
}

//验证评价商品
function verifyReviewProduct(recordId, star, message) {
    if (recordId < 1) {
        alert("请选择商品");
        return false;
    }
    if (star < 1 || star > 5) {
        alert("请选择正确的星星");
        return false;
    }
    if (message.length == 0) {
        alert("请输入评价内容");
        return false;
    }
    if (message.length > 100) {
        alert("评价内容最多输入100个字");
        return false;
    }
    return true;
}

//处理评价商品的反馈信息
function reviewProductResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        var reviewProductFrom = document.forms["reviewProductFrom"];
        reviewProductFrom.elements["recordId"].value = 0;
        reviewProductFrom.elements["message"].value = "";

        $("#reviewProductBlock").hide();

        $("#reviewState" + result.content).html("已评价");
        $("#reviewOperate" + result.content).html("");

        alert("评价成功");
    }
    else {
        alert(result.content);
    }
}

//评价店铺
function reviewStore() {
    var reviewStoreFrom = document.forms["reviewStoreFrom"];

    var oid = reviewStoreFrom.elements["oid"].value;
    var descriptionStar = getSelectedRadio(reviewStoreFrom.elements["descriptionStar"]).value;
    var serviceStar = getSelectedRadio(reviewStoreFrom.elements["serviceStar"]).value;
    var shipStar = getSelectedRadio(reviewStoreFrom.elements["shipStar"]).value;

    $.post("/ucenter/reviewstore?oid=" + oid, { 'descriptionStar': descriptionStar, 'serviceStar': serviceStar, 'shipStar': shipStar }, reviewStoreResponse);
}

//处理评价店铺的反馈信息
function reviewStoreResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        $("#reviewStoreBut").remove();
        alert("评价成功");
    }
    else {
        alert(result.content);
    }
}