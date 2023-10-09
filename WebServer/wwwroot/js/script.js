//небольшой скрипт для временной демонстрации(!!!) активного пункта меню.
$('.sidebar__menu a').each(function () {
	if (this.pathname == location.pathname) {
		if (!$(this).parent().hasClass('classifier_down')) {
			$(this).parent().addClass('active');
		}
	}
})
$('.classifier a').on('click', function (e) {
	e.preventDefault(); // этот код предотвращает стандартное поведение браузера по клику
	$(this).parent().toggleClass('active');
	$('.classifier_down').toggleClass('show');
});
//шапка
$('.menu-btn').click(function () {
	$(this).toggleClass('menu-btn--active');
	$('.sidebar').toggleClass('sidebar--open')
})
//кастомный "селект"
$('.pseudoselect__input').focus(function () {
	$('.pseudoselect__dropdown').hide();
	$(this).siblings('.pseudoselect__dropdown').show()
})
$(document).click(function (e) {
	if (!$(e.target).closest('.pseudoselect').length) {
		$('.pseudoselect__dropdown').hide();
	}
})
$('.pseudoselect__list li').click(function () {
	var value = $(this).text();
	if ($(this).hasClass('classifer')) {
		$(this).closest('.pseudoselect').find('.id_select').val($(this).data('id'));
	}
	$(this).closest('.pseudoselect').find('.id_mashine').val($(this).data('id_mashine'));
	$(this).closest('.pseudoselect').find('.pseudoselect__input').val(value);
	$(this).parent().siblings('.pseudoselect__current').text(value);
	$(this).closest('.pseudoselect__dropdown').hide();
})
$('.pseudoselect__input').on('input change', function () {
	if (this.value) {
		$(this).closest('.pseudoselect').find('.pseudoselect__current').text(this.value);
	} else {
		$(this).closest('.pseudoselect').find('.pseudoselect__current').text('Не выбрано');
	}
})
//сортировка таблиц

//прозрачность в контейнере для прокрутки таблиц
function toggleScrollShadow() {
	if (this.clientWidth + this.scrollLeft < this.scrollWidth) {
		this.classList.add('table-wrapper--scrollable')
	} else {
		this.classList.remove('table-wrapper--scrollable');
	}
}
$('.table-wrapper').scroll(toggleScrollShadow);
$(window).resize(function () {
	$('.table-wrapper').each(toggleScrollShadow);
})
//Установка столбцов графика(ов) по высоте в зависимости от значений
function setChartItemsHeight() {
	var values = [];
	$(this).find('.chart__value').each(function () {
		values.push(parseFloat($(this).data('sum')));
	})
	var max = Math.max.apply(null, values);

	for (var i = 0; i < values.length; i++) {
		$(this).find('.chart__item').eq(i).css('min-height', values[i] / max * 100 + '%');
	}
}
$(document).ready(function () {
	$('.chart').each(setChartItemsHeight);
	if ($('.active').hasClass('classifier')) {
		$('.classifier_down').toggleClass('show');
	}
})
$(window).resize(function () {
	$('.chart').each(setChartItemsHeight);
})