/// <reference path="E:\Projects\HiMall\branches\himall2.1\src\Web\Himall.Web\Scripts/jquery-1.11.1.js" />
/*
{
  "Id": 1,
  "Title": "测试",
  "StartTime": "2015-09-02T11:31:49",
  "EndTime": "2015-09-30T00:00:00",
  "ShortDesc": "描述",
  "ShopId": 1,
  "CreateTime": "2015-09-02T11:33:47",
  "CollocationPoruducts": [
      {
          "Id": 1,
          "ProductId": 123,
          "ColloId": 1,
          "IsMain": true,
          "DisplaySequence": 1,
          "CollocationSkus": [
              {
                  "Id": 2,
                  "ProductId": 123,
                  "SkuID": "123_0_73_76",
                  "ColloProductId": 1,
                  "Price": 1,
                  "SkuPirce": 0.5
              },
              {
                  "Id": 3,
                  "ProductId": 123,
                  "SkuID": "123_0_73_77",
                  "ColloProductId": 1,
                  "Price": 100,
                  "SkuPirce": 95
              },
              {
                  "Id": 4,
                  "ProductId": 123,
                  "SkuID": "123_0_74_76",
                  "ColloProductId": 1,
                  "Price": 100,
                  "SkuPirce": 95
              },
              {
                  "Id": 5,
                  "ProductId": 123,
                  "SkuID": "123_0_74_77",
                  "ColloProductId": 1,
                  "Price": 100,
                  "SkuPirce": 90
              }
          ]
      },
      {
          "Id": 2,
          "ProductId": 124,
          "ColloId": 1,
          "IsMain": false,
          "DisplaySequence": 1,
          "CollocationSkus": [
              {
                  "Id": 1,
                  "ProductId": 124,
                  "SkuID": "124_0_0_0",
                  "ColloProductId": 2,
                  "Price": 10000,
                  "SkuPirce": 999
              }
          ]
      },
      {
          "Id": 3,
          "ProductId": 125,
          "ColloId": 1,
          "IsMain": false,
          "DisplaySequence": 2,
          "CollocationSkus": [
              {
                  "Id": 6,
                  "ProductId": 125,
                  "SkuID": "125_0_73_78",
                  "ColloProductId": 3,
                  "Price": 100,
                  "SkuPirce": 90
              },
              {
                  "Id": 7,
                  "ProductId": 125,
                  "SkuID": "125_0_74_78",
                  "ColloProductId": 3,
                  "Price": 100,
                  "SkuPirce": 90
              },
              {
                  "Id": 8,
                  "ProductId": 125,
                  "SkuID": "125_0_75_78",
                  "ColloProductId": 3,
                  "Price": 100,
                  "SkuPirce": 90
              }
          ]
      }
  ]
}

  /*/

//排序
$("#otherProducts").on("click", '.glyphicon-circle-arrow-up', function () {
    var p = $(this).parents('tr');
    var index = p.parent().find('tr').index(p);
    if (index == 0)
        return false;
    else
        p.prev().before(p);

});
$("#otherProducts").on("click", '.glyphicon-circle-arrow-down', function () {
    var p = $(this).parents('tr');
    var count = p.parent().find('tr').length;
    var index = p.parent().find('tr').index(p);
    if (index == (count - 1))
        return false;
    else
        p.next().after(p);
});
$("#otherProducts").on("click", '.glyphicon', function () {
    $(this).parents('tbody').find('.glyphicon').removeClass('disabled');
    $(this).parents('tbody').find('tr').first().find('.glyphicon-circle-arrow-up').addClass('disabled');
    $(this).parents('tbody').find('tr').last().find('.glyphicon-circle-arrow-down').addClass('disabled');
});
var siblingsVal;
//$('.skutd').each(function () {
//    $('input',this).eq(1).keyup(function () {
        
//    });
//})

$(document).on('keyup', '.skutd input', function () {
    siblingsVal = $(this).siblings().val();
    this.value = this.value.replace(/[^0-9]+/, '');
    if (this.value == '' || this.value <= 0) {
        this.value = '1';
    }
    if (this.value > parseFloat(siblingsVal)) {
        this.value = siblingsVal;
    }
})


function Remove(obj, proid) {
    $(obj).parents("tr").remove();
    if (proid) {
        otherIds.remove(proid);
    }
    else {
        $("#MainProductId").val("");
    }
}
var center = "<span class='glyphicon glyphicon-circle-arrow-up'></span> <span class='glyphicon glyphicon-circle-arrow-down'></span>";
function CreateMainSkuTable(selectedProducts) {

    var t = selectedProducts[0].id;

    var other = $("#OtherTable tr[data-pid='" + t + "']");
    if (other.length > 0)
    {
        $.dialog.errorTips('主商品不能和附商品相同！');
        return;
    }

    var table = "<table id='MainTable' class='table  table-bordered'>";
    var title = "<thead><tr><th>主商品</th><th>规格</th><th>原价格/组合价格</th></tr></thead><tbody>";
    var td = "<tr><td><a href='/product/detail/" + selectedProducts[0].id + "' target='_blank' title=" + selectedProducts[0].name + "><img src='" + selectedProducts[0].imgUrl + "'></img></a></td><td>";
    $(selectedProducts[0].skus).each(function (index, item) {
        td += "<p>" + item.Color + " " + item.Size + " " + item.Version + "</p>";
    });
    td += "</td>";
    td += "<td>";
    $(selectedProducts[0].skus).each(function (index, item) {
        td += "<p class='skutd' data-pid='" + selectedProducts[0].id + "' data-skuid=" + item.Id + "><input type='text' readonly value=" + item.SalePrice + "> <input type='text'   value=" + item.SalePrice + "></p>";
    });
    td += "</td></tr></tbody></table>";
    var htmlTable = table + title + td; //<td><span data=" + selectedProducts[0].id + " onclick='Remove(this)'>删除</span></td>一个就重新选不需要删除
    $("#mainProducts").html(htmlTable);
}

function CreateOtherSkuTable(selectedProducts) {
    otherIds = [];
    var mainpid = $("#MainProductId").val();
    if (selectedProducts.length == 0) { $("#otherProducts").html(""); return; };
    var table = "<table id='OtherTable' class='table table-bordered'>";
    var title = "<thead><tr><th>搭配商品</th><th>规格</th><th>原价格/组合价格</th><th>排序</th><th>操作</th></tr></thead><tbody>";
    var td = "";
    $(selectedProducts).each(function (i, pro) {
        if (pro.id == mainpid) { return true; }
        otherIds.push(pro.id);
        td += "<tr  class='otherTR' data-pid=" + pro.id + "><td><a href='/product/detail/" + pro.id + "' target='_blank' title=" + pro.name + "><img src='" + pro.imgUrl + "'></img></a></td><td>";
        $(pro.skus).each(function (index, item) {
            td += "<p>" + item.Color + " " + item.Size + " " + item.Version + "</p>";
        });
        td += "</td>";
        td += "<td>";
        $(pro.skus).each(function (index, item) {
            td += "<p class='skutd' data-skuid=" + item.Id + "><input type='text' value=" + item.SalePrice + " readonly> <input type='text' value=" + item.SalePrice + "></p>";
        });
        td += "<td class='swaptd'>" + center + "</td></td><td><span class='btn-a'><a onclick='Remove(this," + pro.id + ")'>删除</a></span></td></tr>";
    });
    td += "</tbody></table>";
    var htmlTable = table + title + td;


    $("#otherProducts").html(htmlTable);
    $("#OtherTable tbody tr").first().find('.glyphicon-circle-arrow-up').addClass('disabled');
    $("#OtherTable tbody tr").last().find('.glyphicon-circle-arrow-down').addClass('disabled');
}

function CollotionInfo() {
    var collocation = {
        Id: 0,
        Title: "测试",
        StartTime: "2015-09-02T11:31:49",
        EndTime: "2015-09-30T00:00:00",
        ShortDesc: "描述",
        ShopId: 1,
        CreateTime: "2015-09-02",
        CollocationPoruducts: []
    };
    function collocationProduct() {
        this.ProductId = 123;
        this.IsMain = true;
        this.DisplaySequence = 1;
        this.CollocationSkus = [];
    }
    function Sku() {
        this.ProductId = 123,
          this.SkuID = "123_0_73_76",
        //this.ColloProductId =0,
             this.Price = 1,
           this.SkuPirce = 0.5
    }
    collocation.Title = $("#Title").val();
    collocation.Id = $("#collocationId").val();
    collocation.StartTime = $("#StartTime").val();
    collocation.EndTime = $("#EndTime").val();
    collocation.ShortDesc = $("#ShortDesc").val();
    var mainProduct = new collocationProduct();

    mainProduct.ProductId = $("#MainTable").find(".skutd").eq(0).data("pid");
    mainProduct.IsMain = true;
    mainProduct.DisplaySequence = 0;
    $("#MainTable").find(".skutd").each(function (index, item) {
        var mainSku = new Sku();
        mainSku.ProductId = $(item).data("pid");
        mainSku.SkuID = $(item).data("skuid");
        mainSku.Price = $(item).find("input").eq(1).val();
        mainSku.SkuPirce = $(item).find("input").eq(0).val();
        mainProduct.CollocationSkus.push(mainSku);
    });
    collocation.CollocationPoruducts.push(mainProduct);
    $(".otherTR").each(function (index, item) {
        var otherProduct = new collocationProduct();
        otherProduct.ProductId = $(item).data("pid");
        otherProduct.IsMain = false;
        otherProduct.DisplaySequence = ++index;
        $(item).find(".skutd").each(function (i, skuItem) {
            var otherSku = new Sku();
            otherSku.ProductId = otherProduct.ProductId
            otherSku.SkuID = $(skuItem).data("skuid");
            otherSku.Price = $(skuItem).find("input").eq(1).val();
            otherSku.SkuPirce = $(skuItem).find("input").eq(0).val();
            otherProduct.CollocationSkus.push(otherSku);
        });
        collocation.CollocationPoruducts.push(otherProduct);
    });
    return collocation;
}

function CheckCollocation() {
    var title = $("#Title").val();
    var startTime = $("#StartTime").val();
    var endTime = $("#EndTime").val();
    var regDate = /^\d{4}(\-|\.)\d{2}(\-|\.)\d{2}$/;
    if (title.length < 2) {
        $.dialog.errorTips('组合购标题不能为空！');
        return false;
    }
    else if ($("#MainTable .skutd").length == 0) {
        $.dialog.errorTips('没有选择主商品！');
        return false;
    }
    else if ($(".otherTR").length == 0) {
        $.dialog.errorTips('没有选择附加商品！');
        return false;
    }
    else if ($(".otherTR").length > 9) {
        $.dialog.errorTips('附加商品最多允许9个！');
        return false;
    }
    else if (!regDate.test(startTime) || !regDate.test(endTime)) {
        $.dialog.errorTips('开始日期或者结束日期格式错误！');
        return false;
    }
    return true;
}


function PostCollocation() {
    if (!CheckCollocation()) return;
    var collocationjson = CollotionInfo();
    if (collocationjson == null) return;
    var objectString = JSON.stringify(collocationjson);
    var loading = showLoading();
    $.post('./AddCollocation', { collocationjson: objectString }, function (result) {
        loading.close();
        if (result.success) {
            $.dialog.tips('保存成功', function () { location.href = './management'; });
        }
        else
            $.dialog.tips('保存失败！' + result.msg);
    }, "json");
}
function EditCollocation() {
    if (!CheckCollocation()) return;
    var collocationjson = CollotionInfo();
    if (collocationjson == null) return;
    var objectString = JSON.stringify(collocationjson);
    var loading = showLoading();
    $.post('/SellerAdmin/Collocation/EditCollocation', { collocationjson: objectString }, function (result) {
        loading.close();
        if (result.success) {
            $.dialog.tips('保存成功', function () { location.href = '/SellerAdmin/Collocation/management'; });
        }
        else
            $.dialog.tips('保存失败！' + result.msg);
    }, "json");
}