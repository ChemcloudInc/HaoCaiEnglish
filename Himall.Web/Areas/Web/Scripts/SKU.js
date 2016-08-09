// SKU组合查询
$(function () {
	var pId=$('#gid').val();
	if($('#choose').length>0)
    	setSKUGroup(pId,$('#choose'));
});


function setSKUGroup(pId,SkuObj,colloPid){

	var create = function (data) {
        var i = 0, len = data.length, result = {};
        for (; i < len; i++) {
            result[data[i].SKUId] = {};
            result[data[i].SKUId].price = data[i].Price;
            result[data[i].SKUId].count = data[i].Stock;
        }
        return result;
    },
    SKUDATA = null,
	_this=SkuObj,
	mProduct=_this[0].id,
	data={'pId': pId,'colloPid':colloPid };
	if(mProduct=="choose"){
		data={ 'pId': pId }
	}
	
    $.ajax({
        type: 'GET',
        url: '/Product/GetSKUInfo',
        cache: false,
        data: data,
        dataType: 'json',
        success: function (data) {
            var getSKU = ({
                SKU: {},
                init: function (data) {
                    var i = 0, j, l,
                        skuKeyArr = this.getKey(data),// 可以看成可行路径 取得所有可行路径组成集合
                        len = skuKeyArr.length,
                        skuKey,// 原始数据里面的一条SKU数据的键
                        skuValue,// 原始数据里面的一条SKU数据的值
                        skuKeyArrs,// 原始数据里面的一条SKU数据的键返回的新的数组[1,2,3,4]
                        combinationArr;// 排列组合返回的所有可行路径
                    for (; i < len; i++) {
                        skuKey = skuKeyArr[i];
                        skuValue = data[skuKey];
                        skuKeyArrs = skuKey.split(";");
                        skuKeyArrs.sort(function (value1, value2) {
                            return (value1 >> 0) - (value2 >> 0);
                        });
                        // 进入combination方法里面的参数[1,2,3,4]
                        // 对每个SKU信息key属性值进行拆分组合 返回 [ [1],[2],[3],[4], [1,2],[1,3],[1,4],[2,3],[2,4],[3,4], [1,2,4],[1,3,4],[2,3,4],[1,2,3] ]
                        combinationArr = this.combination(skuKeyArrs);
                        //console.log(combinationArr);
                        l = combinationArr.length;
                        for (j = 0; j < l; j++) {
                            this.add(combinationArr[j], skuValue);
                        }
                        //结果全组合放入SKU 由于在拆分组合的时候没有生成3个元素组合所以放到这一步了
                        this.SKU[skuKeyArrs.join(";")] = {
                            count: skuValue.count,
                            prices: [skuValue.price]
                        }
                    }
                    return this.SKU;
                },
                getKey: function (data) {
                    var data = data,
                        key,
                        arr = [];
                    for (key in data) {
                        if (Object.prototype.hasOwnProperty.call(data, key)) {
                            arr.push(key);
                        }
                    }
                    return arr;
                },
                // 返回生成的排列组合
                combination: function (arr) {
                    var a = this.create(arr, 1),
                        b = this.create(arr, 2),
                        result = a.concat(b);
                    return result;
                },
                // 核心方法 排列组合 组合中没有重复的组合 排列中有重复的组合(顺序不一样)
                create: function (arr, num) {
                    var result = [];
                    ; (function fn(newArr, arr, n) {
                        if (n == 0) {
                            return result.push(newArr);
                        }
                        for (var i = 0, len = arr.length; i <= len - n; i++) {
                            fn(newArr.concat(arr[i]), arr.slice(i + 1), n - 1);
                        }
                    }([], arr, num));
                    return result;
                },
                add: function (arr, sku) {
                    var key = arr.join(';');
                    if (this.SKU[key]) {
                        this.SKU[key].count += sku.count;
                        this.SKU[key].prices.push(sku.price);
                    } else {
                        this.SKU[key] = {
                            count: sku.count,
                            prices: [sku.price]
                        };
                    }
                }
            }).init(create(data.skuArray));
            SKUDATA = getSKU;
            //初始化用户选择事件
            $(function () {
				/*if(mProduct!="choose"){
					var first;
					if(_this.find('.enabled').length>0){
						first=_this.find('.enabled').attr('cid')
					}else{
						first=0
					}
					_this.find('.product-price').text(SKUDATA[first].prices[0]);;
				}
				*/
				
                _this.find('.enabled').each(function () {
                    var _that = $(this),
                        cid = _that.attr('cid');
                    if (!SKUDATA[cid]) {
                        _that.attr('disabled', 'disabled');
                    }
                }).click(function () {

                    var _that = $(this);
                    //选择自己 兄弟节点取消选中
                    _that.toggleClass('selected').siblings().removeClass('selected');
					if(mProduct=="choose"){
						chooseResult();
					}else{
						
						var len = _this.find('.selected').length,
							cSkuId=skuId+'pId',
							cSkuId = new Array(3);
						for (var i = 0; i < len; i++) {
							var index = parseInt(_this.find('.selected').eq(i).attr('st'));
							cSkuId[index] = _this.find('.selected').eq(i).attr('cid');
						}
						//请求Ajax获取价格
						if (len === _this.find(".choose-sku").length) {
							var newSKU = pId;
							for (var i = 0; i < 3; i++) {
								newSKU += ((cSkuId[i] == undefined ? '_' + 0 : '_' + cSkuId[i]));
							}
							_this.find('.group-skuId').val(newSKU);
							$.ajax({
								type: 'get',
								url: '../GetStock',
								data: { skuId: newSKU },
								dataType: 'json',
								cache: false,// 开启ajax缓存
								async: false,
								success: function (data) {
									_this.find(".group-stock").text(data.Stock);
									var stock = parseInt(data.Stock);
									var Status = parseInt(data.Status);
						
								},
								error: function (e) {
									//alert(e);
								}
							});
						}
					}
                                        
                    //已经选择的节点
                    var selectedObjs = _this.find('.selected');
                    if (selectedObjs.length) {
                        var selectedIds = [];//获得组合价格
                        selectedObjs.each(function () {
                            selectedIds.push($(this).attr('cid'));
                        });
                        selectedIds.sort(function (value1, value2) {
                            return parseInt(value1) - parseInt(value2);
                        });
                        var len = selectedIds.length;
                        var prices = SKUDATA[selectedIds.join(';')].prices;
                        var maxPrice = Math.max.apply(Math, prices);
                        var minPrice = Math.min.apply(Math, prices);
						if(mProduct=="choose"){
                        	maxPrice > minPrice ? minPrice + "-" + maxPrice : ($('#jd-price').text('￥ ' + maxPrice));
						}else{
							_this.find('.product-price').text(minPrice);
						}
						if(mProduct!="choose"){
							var groupPrice=0;
							for(var i=0;i<$('.group-item').length;i++){
								groupPrice+=parseFloat($('.product-price').eq(i).text());
							}
							$('.group-price span').data('groupprice',groupPrice).text(($('#groupCounts').val()*groupPrice).toFixed(2));
						}
						
                        //已选中的节点验证待测试节点
                        _this.find(".enabled").not(selectedObjs).not(_that).each(function () {
                            var siblingsSelectedObj = $(this).siblings('.selected'),
                                testAttrIds = [];//从选中节点中去掉选中的兄弟节点
                            if (siblingsSelectedObj.length) {
                                var siblingsSelectedObjId = siblingsSelectedObj.attr('cid');
                                for (var i = 0; i < len; i++) {
                                    (selectedIds[i] != siblingsSelectedObjId) && testAttrIds.push(selectedIds[i]);
                                }
                            } else {
                                testAttrIds = selectedIds.concat();
                            }
                            testAttrIds = testAttrIds.concat($(this).attr('cid'));
                            testAttrIds.sort(function (value1, value2) {
                                return parseInt(value1) - parseInt(value2);
                            });
                            if (!SKUDATA[testAttrIds.join(';')]) {
                                $(this).addClass('disabled').removeClass('selected');
                            } else {
                                $(this).removeClass('disabled');
                            }
                        });
                    } else {
                        _this.find('.enabled').each(function () {
                            SKUDATA[$(this).attr('cid')] ? $(this).removeClass('disabled') : $(this).addClass('disabled').removeClass('selected');
                        });
                    }
                });
            });
			
			
            //有规格的时候默认一条数据
			if(mProduct=="choose"){
			    if (_this.find(".choose-sku").length != 0) {
			        _this.find(".choose-sku").each(function () {
			            $(".dd", this).children("div:first").not(".disabled").find("a:first").trigger("click");
			        });
			    }

			}
			if (_this.find(".spec").length != 0)
                checkFirstSKUWhenHas();
        },
        error: function () { }
    });
}

