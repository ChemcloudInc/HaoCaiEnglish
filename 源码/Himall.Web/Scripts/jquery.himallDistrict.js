/*
author: Five(673921852)
description: 市级地区选择（适用于运费计算，收货地址）
2015.6.1
*/

(function($) {
	$.fn.himallDistrict = function(options) {
		var defaults = {
			// 设置id
			id : $(this).attr("id"),
			// 指定绑定源
			renderTo : $(this).parent(),
			// 地区数据源
			items : new Array(),
			// 默认选中项,优先从this的data-select属性获取默认选中值
			select : '',
			// 关闭时回调
			closeFn : function() {
			}
		};
		var params = $.extend(defaults, options);
		params.renderTo = (typeof params.renderTo == 'string' ? $(params.renderTo) : params.renderTo);
		/**
		 * 遍历
		 */
		this.each(function() {
			var _this = $(this);
			var district=_this.siblings('.himall-district');
			
			if (params.items.length < 0) {
				return false;
			}
			if (!(params.id.length > 0)) {
				return false;
			}
			var thisId = 'himallDistrict-' + params.id;
			var arrSelect = params.select.split(',');
			if (_this.data("select")) {
				arrSelect = _this.data("select").split(',');
			}

			var provinceStr='<ul class="district-ul province-ul cl">';
			for(var i=0; i<params.items.length;i++){
				provinceStr+='<li><a data-id="'+params.items[i].id+'" data-index="'+i+'">'+params.items[i].name+'</a></li>';
			}
			provinceStr+='</ul>';
			
			var himallDistrict = '<div class="himall-district" id="'+thisId+'"><div class="district-hd"><span>请选择省</span><span>请选择市</span></div>'+provinceStr+'<ul class="district-ul city-ul cl"></ul></div>';
			
			
			if(params.renderTo.find("#"+thisId).length>0){
				if($("#"+thisId).is(':visible')){
					$("#"+thisId).hide();
					_this.removeClass('active');
				}else{
					$("#"+thisId).show();
					_this.addClass('active');
				}
				
			}else{
				params.renderTo.append(himallDistrict);
				_this.addClass('active');
			}
			
			
			var headSelect=params.renderTo.find('.district-hd').children();
			
			
			//省级点击
			params.renderTo.on('click','.province-ul a',function() {
				var cityStr='';
				var parent=params.items[$(this).data('index')];
				$(this).parent().addClass('cur').siblings().removeClass();
				$(this).parents('.district-ul').hide().siblings('ul').show().siblings('.district-hd').children().eq(0).removeClass().text($(this).text()).siblings().addClass('cur').text('请选择市');
				for(var i=0; i<parent.city.length;i++){
					cityStr+='<li><a data-id="'+parent.city[i].id+'" data-province="'+parent.id+'">'+parent.city[i].name+'</a></li>';
				}
				params.renderTo.find('.city-ul').html(cityStr);
			});
			
			//市级点击
			params.renderTo.off('click','.city-ul a');
			params.renderTo.on('click','.city-ul a',function() {
				
				$(this).parents('.district-ul').hide().siblings('ul').show().siblings('.district-hd').children().eq(1).text($(this).text()).removeClass().siblings().addClass('cur');
				_this.removeClass('active').data('select',$(this).data('province')+','+$(this).data('id')).html(headSelect.eq(0).text()+' '+headSelect.eq(1).text());
				$(this).parent().addClass('cur').siblings().removeClass();
				$(this).parents('.himall-district').hide();
				params.closeFn();
				
			});
			
			//切换卡点击
			params.renderTo.on('click','.district-hd span',function() {
				$(this).addClass('cur').siblings().removeClass().parent().siblings('ul').hide().eq($(this).index()).show();
			});
			
			
			
			//初始化数据
			
			if(arrSelect){
				params.renderTo.find('.province-ul').find('a[data-id="'+arrSelect[0]+'"]').click();
				params.renderTo.find('.city-ul').find('a[data-id="'+arrSelect[1]+'"]').parent().addClass('cur');
				params.renderTo.find('.district-hd span').first().click();
				var arrSet = _this.text().split(' ');
				headSelect.eq(0).text(arrSet[0]).siblings().text(arrSet[1]);
				
			}
		});
		
		
			
	};
})(jQuery);