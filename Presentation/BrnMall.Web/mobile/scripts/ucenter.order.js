﻿//获得订单列表
var orderListNextPageNumber = 2;
function getOrderList(startAddTime, endAddTime, orderState, page) {
    $("#loadBut").hide();
    $("#loadPrompt").show();
    $.get("/mob/ucenter/ajaxorderlist?startAddTime=" + startAddTime + "&endAddTime=" + endAddTime + "&orderState=" + orderState + "&page=" + page, function (data) {
        getOrderListResponse(data);
    })
}

//处理获得订单列表的反馈信息
function getOrderListResponse(data) {
    try {
        var result = eval("(" + data + ")");
        var element = document.createElement("div");
        element.className = "proItme";
        var innerHTML = "";
        for (var i = 0; i < result.OrderList.length; i++) {
            var oid = result.OrderList[i].oid;
            var orderState = result.OrderList[i].orderstate;
            var orderProductInfo = null;
            for (var j = 0; j < result.OrderProductList.length; j++) {
                if (result.OrderProductList[j].Oid == oid) {
                    if (orderProductInfo == null)
                        orderProductInfo = result.OrderProductList[j];
                }
            }
            innerHTML += "<div class='proItme'>";
            innerHTML += "<a href='/ucenter/orderinfo?oid=" + oid + "'>";
            innerHTML += "<img src='/upload/store/" + orderProductInfo.StoreId + "/product/show/thumb60_60/" + orderProductInfo.ShowImg + "'>";
            innerHTML += "<div class='order-msg'><p class='title'>" + orderProductInfo.Name + "</p><p class='price'>¥" + orderProductInfo.DiscountPrice + "<span></span></p><p class='order-data'>" + orderProductInfo.AddTime + "</p></div>";
            innerHTML += "</a>";
            innerHTML += "<div class='proBt'>";
            innerHTML += "<a class='redBt' href='/ucenter/orderaction?oid=" + oid + "' id='orderActionList" + oid + "'>订单跟踪</a>";
            if (orderState == 30 && result.OrderList[i].paymode == 1) {
                innerHTML += "<a class='redBt' href='/order/payshow?oidList=" + oid + "' id='payOrderBut" + oid + "'>在线支付</a>";
            }
            if (orderState == 140 && result.OrderList[i].isreview == 0) {
                innerHTML += "<a class='redBt' href='/ucenter/revieworder?oid=" + oid + "'>订单评价</a>";
            }
            if (orderState == 30 || (orderState == 50 && result.OrderList[i].paymode == 0)) {
                innerHTML += "<a class='redBt' href='javascript:cancelOrder(" + oid + ", 0)' id='cancelOrderBut" + oid + "'>取消订单</a>";
            }
            innerHTML += "</div>";
            innerHTML += "</div>";
        }
        element.innerHTML = innerHTML;
        document.getElementById("orderListBlock").appendChild(element);
        if (result.PageModel.HasNextPage) {
            $("#loadBut").show();
            $("#loadPrompt").hide();
            orderListNextPageNumber += 1;
        }
        else {
            $("#loadBut").hide();
            $("#loadPrompt").hide();
            $("#lastPagePrompt").show();
        }
    }
    catch (ex) {
        alert("加载错误");
    }
}

//取消订单
function cancelOrder(oid, cancelReason) {
    $.post("/mob/ucenter/cancelorder", { 'oid': oid, 'cancelReason': cancelReason }, cancelOrderResponse);
}

//处理取消订单的反馈信息
function cancelOrderResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        $("#cancelOrderBut" + result.content).parent().html("<a class='redBt' href='" + document.getElementById("orderActionList" + result.content).href + "'>订单跟踪</a>");
        alert("取消成功");
    }
    else {
        alert(result.content);
    }
}

//选择商品星星
function selectProductStar(i) {
    var list = document.getElementById("productStarBlock").getElementsByTagName("span");
    for (var j = 1; j <= 5; j++) {
        if (j <= i) {
            list[j - 1].className = "on";
        }
        else {
            list[j - 1].className = "";
        }
    }
    var reviewProductFrom = document.forms["reviewProductFrom"];
    reviewProductFrom.elements["star"].value = i;
}

//选择描述星星
function selectDescriptionStar(i) {
    var list = document.getElementById("descriptionBlock").getElementsByTagName("span");
    for (var j = 1; j <= 5; j++) {
        if (j <= i) {
            list[j - 1].className = "on";
        }
        else {
            list[j - 1].className = "";
        }
    }
    var reviewProductFrom = document.forms["reviewStoreFrom"];
    reviewProductFrom.elements["descriptionStar"].value = i;
}

//选择服务星星
function selectServiceStar(i) {
    var list = document.getElementById("serviceBlock").getElementsByTagName("span");
    for (var j = 1; j <= 5; j++) {
        if (j <= i) {
            list[j - 1].className = "on";
        }
        else {
            list[j - 1].className = "";
        }
    }
    var reviewProductFrom = document.forms["reviewStoreFrom"];
    reviewProductFrom.elements["serviceStar"].value = i;
}

//选择配送星星
function selectShipStar(i) {
    var list = document.getElementById("shipBlock").getElementsByTagName("span");
    for (var j = 1; j <= 5; j++) {
        if (j <= i) {
            list[j - 1].className = "on";
        }
        else {
            list[j - 1].className = "";
        }
    }
    var reviewProductFrom = document.forms["reviewStoreFrom"];
    reviewProductFrom.elements["shipStar"].value = i;
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
    var star = parseInt(reviewProductFrom.elements["star"].value);
    var message = reviewProductFrom.elements["message"].value;

    if (!verifyReviewProduct(recordId, star, message)) {
        return;
    }
    $.post("/mob/ucenter/reviewproduct?oid=" + oid + "&recordId=" + recordId, { 'star': star, 'message': message }, reviewProductResponse);
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
        selectProductStar(5);

        $("#reviewProductBlock").style.display = "none";

        $("#reviewOperate" + result.content).html("您已评价");
        $("#reviewOperate" + result.content).unbind();
        $("#reviewOperate" + result.content).addClassName("gayBt");
        $("#reviewOperate" + result.content).css("color", "#999");

        alert("评价成功");
    }
    else {
        alert(result.content);
    }
}

//打开评价店铺层
function openReviewStoreBlock() {
    $("#reviewStoreBlock").toggle();
}

//评价店铺
function reviewStore() {
    var reviewStoreFrom = document.forms["reviewStoreFrom"];

    var oid = reviewStoreFrom.elements["oid"].value;
    var descriptionStar = parseInt(reviewStoreFrom.elements["descriptionStar"].value);
    var serviceStar = parseInt(reviewStoreFrom.elements["serviceStar"].value);
    var shipStar = parseInt(reviewStoreFrom.elements["shipStar"].value);

    $.post("/mob/ucenter/reviewstore?oid=" + oid, { 'descriptionStar': descriptionStar, 'serviceStar': serviceStar, 'shipStar': shipStar }, reviewStoreResponse);
}

//处理评价店铺的反馈信息
function reviewStoreResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        $("#reviewStoreBlock").hide();
        $("#reviewStoreBut").remove();
        alert("评价成功");
    }
    else {
        alert(result.content);
    }
}